using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Venta.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;
using SAMBHS.Windows.WinClient.UI.Reportes.Compras;
using SAMBHS.Windows.WinClient.UI.Requerimientos.Ablimatex.Printer;
using SAMBHS.Windows.WinClient.UI.Requerimientos.Ablimatex;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmOrdenCompra : Form
    {
        readonly DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ordendecompraDto _ordendecompraDto = new ordendecompraDto();
        readonly OrdenCompraBL _objOrdenCompraBL = new OrdenCompraBL();
        readonly VentaBL _objVentasBL = new VentaBL();
        readonly DocumentoBL _objDocumentoBL = new DocumentoBL();
        readonly ComprasBL _objComprasBL = new ComprasBL();
        readonly UltraCombo ucUnidadMedida = new UltraCombo();
        readonly UltraCombo ucAlmacen = new UltraCombo();
        string strModo = string.Empty;
        readonly string strIdOrdenCompra = string.Empty;
        string _Mode = string.Empty;
        ordendecompradetalleDto _ordendecompradetalleDto;

        #region Temporales
        List<ordendecompradetalleDto> _TempDetalle_AgregarDto = new List<ordendecompradetalleDto>();
        List<ordendecompradetalleDto> _TempDetalle_ModificarDto = new List<ordendecompradetalleDto>();
        List<ordendecompradetalleDto> _TempDetalle_EliminarDto = new List<ordendecompradetalleDto>();
        #endregion

        public frmOrdenCompra(string Modo, string IdOrdenCompra)
        {
            InitializeComponent();
            strModo = Modo;
            strIdOrdenCompra = IdOrdenCompra;

            // Solo para ablimatex
            if (Globals.ClientSession.v_RucEmpresa != Constants.RucHormiguita) return;
            btnPrintCodes.Visible = true;
            btnPrintCodes.Click += btnPrintLabel_Click;
            btnAvios.Visible = true;
            btnAvios.Click += btnPrintLabel_Click;
        }

        private void frmOrdenCompra_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            this.BackColor = new GlobalFormColors().FormColor;
            var _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);
            panel1.BackColor = new GlobalFormColors().BannerColor;
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboIGV, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 27, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAreaSolicita, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 90, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 92, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboFormaPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 91, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboTipoOrdenCompra , "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 179, null), DropDownListAction.Select);
            cboTipoOrdenCompra.Value = "1";
            cboAreaSolicita.Value = "-1";
            cboFormaPago.Value = "-1";
            CargarCombosDetalle();
            cboDocumento.Value = "404";
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboIGV.SelectedValue = Globals.ClientSession.i_IdIgv.ToString();
            FijarRegistro();
            CargarOrdenCompras();
            chkAfectoIgv.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita ? true : false;
        }

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            FijarRegistro();
            var objOperationResult = new OperationResult();
            var TipoCambio = new ComprasBL().DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
            txtTipoCambio.Text = TipoCambio;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
            grdData.Rows.Move(row, grdData.Rows.Count() - 1);
            this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
            row.Cells["i_RegistroEstado"].Value = "Agregado";
            row.Cells["i_RegistroTipo"].Value = "Temporal";
            row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            row.Cells["i_IdUnidadMedida"].Value = "-1";
        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProveedor")
            {
                var frm = new frmBuscarProveedor(null, "Nombre");
                frm.ShowDialog();
                if (!string.IsNullOrEmpty(frm._RazonSocial))
                {
                    txtProveedor.Tag = frm._CodigoProveedor.TrimEnd();
                    txtProveedor.Text = frm._RazonSocial;
                    txtRUCProveedor.Text = frm._NroDocumento;
                    _ordendecompraDto.v_IdProveedor = frm._IdProveedor;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid && ValidarValoresGrilla())
            {
                CalcularValores();
                OperationResult objOperationResult = new OperationResult();
                string IdResult = string.Empty;
                switch (_Mode)
                {
                    case "New":

                        while (!_objOrdenCompraBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text))
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }

                        while (!_objOrdenCompraBL.ExisteDocumento(txtSerieDoc.Text, txtCorrelativoDoc.Text))
                        {
                            txtCorrelativoDoc.Text = (int.Parse(txtCorrelativoDoc.Text) + 1).ToString("00000000");
                        }

                        _ordendecompraDto.v_DocumentoInterno = txtDocInterno.Text.Trim();
                        _ordendecompraDto.v_Correlativo = txtCorrelativo.Text;
                        _ordendecompraDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                        _ordendecompraDto.t_FechaEntrega = dtpFechaEntrega.Value.Date;
                        _ordendecompraDto.v_Mes = txtMes.Text;
                        _ordendecompraDto.v_Periodo = txtPeriodo.Text;
                        _ordendecompraDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                        _ordendecompraDto.v_SerieDocumento = txtSerieDoc.Text;
                        _ordendecompraDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text;
                        _ordendecompraDto.i_IdAreaSolicita = int.Parse(cboAreaSolicita.Value.ToString());
                        _ordendecompraDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                        _ordendecompraDto.i_IdFormaPago = int.Parse(cboFormaPago.Value.ToString());
                        _ordendecompraDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _ordendecompraDto.d_TipoCambio = !string.IsNullOrEmpty(txtTipoCambio.Text) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0;
                        _ordendecompraDto.d_IGV = !string.IsNullOrEmpty(txtIGV.Text) ? decimal.Parse(txtIGV.Text.Trim()) : 0;
                        _ordendecompraDto.d_SubTotal = !string.IsNullOrEmpty(txtSubTotal.Text) ? decimal.Parse(txtSubTotal.Text.Trim()) : 0;
                        _ordendecompraDto.d_Total = !string.IsNullOrEmpty(txtTotal.Text) ? decimal.Parse(txtTotal.Text.Trim()) : 0;
                        _ordendecompraDto.v_AdjuntarAnexo = txtAdjuntarAnexos.Text;
                        _ordendecompraDto.v_LugarEntrega = txtLugarEntrega.Text;
                        _ordendecompraDto.v_Importante = txtObservaciones.Text;
                        _ordendecompraDto.i_PreciosIncluyeIgv = chkIncluyeIgv.Checked ? 1 : 0;
                        _ordendecompraDto.i_PreciosAfectosIgv = chkAfectoIgv.Checked ? 1 : 0;
                        _ordendecompraDto.v_NroCheque = txtNroCheque.Text;
                        _ordendecompraDto.i_NroDias = !string.IsNullOrEmpty(txtNroDias.Text.Trim()) ? int.Parse(txtNroDias.Text.Trim()) : 0;
                        _ordendecompraDto.i_IdEntidadBancaria = txtEntidadBancaria_1.Tag != null ? int.Parse(txtEntidadBancaria_1.Tag.ToString()) : -1;
                        _ordendecompraDto.i_IdTipoOrdenCompra = int.Parse ( cboTipoOrdenCompra.Value.ToString());
                        LlenarTemporales();
                        IdResult = _objOrdenCompraBL.InsertarOrdenCompra(ref objOperationResult, _ordendecompraDto, _TempDetalle_AgregarDto, Globals.ClientSession.GetAsList());
                        break;

                    case "Edit":
                        _ordendecompraDto.v_DocumentoInterno = txtDocInterno.Text.Trim();
                        _ordendecompraDto.v_Correlativo = txtCorrelativo.Text;
                        _ordendecompraDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                        _ordendecompraDto.t_FechaEntrega = dtpFechaEntrega.Value.Date;
                        _ordendecompraDto.v_Mes = txtMes.Text;
                        _ordendecompraDto.v_Periodo = txtPeriodo.Text;
                        _ordendecompraDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                        _ordendecompraDto.v_SerieDocumento = txtSerieDoc.Text;
                        _ordendecompraDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text;
                        _ordendecompraDto.i_IdAreaSolicita = int.Parse(cboAreaSolicita.Value.ToString());
                        _ordendecompraDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                        _ordendecompraDto.i_IdFormaPago = int.Parse(cboFormaPago.Value.ToString());
                        _ordendecompraDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _ordendecompraDto.d_TipoCambio = !string.IsNullOrEmpty(txtTipoCambio.Text) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0;
                        _ordendecompraDto.d_IGV = !string.IsNullOrEmpty(txtIGV.Text) ? decimal.Parse(txtIGV.Text.Trim()) : 0;
                        _ordendecompraDto.d_SubTotal = !string.IsNullOrEmpty(txtSubTotal.Text) ? decimal.Parse(txtSubTotal.Text.Trim()) : 0;
                        _ordendecompraDto.d_Total = !string.IsNullOrEmpty(txtTotal.Text) ? decimal.Parse(txtTotal.Text.Trim()) : 0;
                        _ordendecompraDto.v_AdjuntarAnexo = txtAdjuntarAnexos.Text;
                        _ordendecompraDto.v_LugarEntrega = txtLugarEntrega.Text;
                        _ordendecompraDto.i_PreciosIncluyeIgv = chkIncluyeIgv.Checked ? 1 : 0;
                        _ordendecompraDto.i_PreciosAfectosIgv = chkAfectoIgv.Checked ? 1 : 0;
                        _ordendecompraDto.v_Importante = txtObservaciones.Text;
                        _ordendecompraDto.v_NroCheque = txtNroCheque.Text;
                        _ordendecompraDto.i_NroDias = !string.IsNullOrEmpty(txtNroDias.Text.Trim()) ? int.Parse(txtNroDias.Text.Trim()) : 0;
                        _ordendecompraDto.i_IdEntidadBancaria = txtEntidadBancaria_1.Tag != null ? int.Parse(txtEntidadBancaria_1.Tag.ToString()) : -1;
                        _ordendecompraDto.i_IdTipoOrdenCompra = int.Parse(cboTipoOrdenCompra.Value.ToString());
                        LlenarTemporales();
                        IdResult = _objOrdenCompraBL.ActualizarOrdenCompra(ref objOperationResult, _ordendecompraDto, _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto, Globals.ClientSession.GetAsList());
                        break;
                }

                if (objOperationResult.Success == 1)
                {
                    strModo = "Edicion";
                    CargarCabecera(IdResult);
                    BtnImprimir.Enabled = true;
                    UltraMessageBox.Show("Registro Guardado", "Sistema");
                    _TempDetalle_AgregarDto = new List<ordendecompradetalleDto>();
                    _TempDetalle_EliminarDto = new List<ordendecompradetalleDto>();
                    _TempDetalle_ModificarDto = new List<ordendecompradetalleDto>();
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            frmProductoRegistroRapido frm = new frmProductoRegistroRapido();
            frm.ShowDialog();
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {

            if (ultraExpandableGroupBox1.Expanded == true)
            {
                groupBox3.Location = new Point(groupBox3.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height);
                groupBox3.Height = this.Height - groupBox3.Location.Y - 77;
            }
            else
            {
                groupBox3.Location = new Point(groupBox3.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height);
                groupBox3.Height = this.Height - groupBox3.Location.Y - 77;
            }
        }

        private void cboEstado_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_ordendecompraDto.i_IdEstado.ToString() != "2" && cboEstado.Value != null && cboEstado.Value.ToString() == "2")
            {
                UltraMessageBox.Show("La Orden de Compra será Procesada cuando esta halla sido usada en una compra. \nNo se puede cambiar a Procesado manualmente.", "Error de Valdación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboEstado.Value = "-1";
            }
        }

        #region Eventos Grilla
        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            if (e.Cell.Column.Key != "CodProducto") return;
            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
            {
                if (grdData.ActiveRow.Cells["i_IdAlmacen"].Value == null) return;

                if (_objVentasBL.EsValidoCodProducto(e.Cell.Text))
                {
                    var producto = _objComprasBL.DevolverArticuloPorCodInterno(e.Cell.Text);

                    if ((grdData.Rows.All(f => f.Cells["v_IdProductoDetalle"].Value == null) && producto != null) || grdData.Rows.All(f => f.Cells["v_IdProductoDetalle"].Value.ToString() != producto.v_IdProductoDetalle))
                    {
                        grdData.ActiveRow.Cells["NombreProducto"].Value = producto.v_Descripcion;
                        grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = producto.v_IdProductoDetalle;
                        grdData.ActiveRow.Cells["CodProducto"].Value = producto.v_CodInterno;
                        grdData.ActiveRow.Cells["Empaque"].Value = producto.d_Empaque;
                        grdData.ActiveRow.Cells["EmpaqueUM"].Value = producto.EmpaqueUnidadMedida;
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = producto.i_IdUnidadMedida;
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = producto.i_IdUnidadMedida;
                        grdData.ActiveRow.Cells["d_PrecioUnitario"].Value =Globals.ClientSession.v_RucEmpresa ==Constants.RucHormiguita ? 0: producto.d_Precio;
                        grdData.ActiveRow.Cells["d_PrecioVenta"].Value = Globals.ClientSession.v_RucEmpresa ==Constants.RucHormiguita ?0:producto.PrecioVenta;  
                    }

                }
                else
                {
                    var frm = new frmBuscarProducto(int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()), null, null, grdData.ActiveRow.Cells["CodProducto"].Text ?? string.Empty);
                    frm.ShowDialog();
                    if (frm._NombreProducto != null)
                    {
                        grdData.ActiveRow.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        var idAlmacen = int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString());
                        foreach (var fila in grdData.Rows)
                        {
                            if (fila.Cells["v_IdProductoDetalle"].Value != null)
                            {
                                if (frm._IdProducto == fila.Cells["v_IdProductoDetalle"].Value.ToString() && idAlmacen == int.Parse(fila.Cells["i_IdAlmacen"].Value.ToString()))
                                {
                                    UltraMessageBox.Show("El producto ya existe en el detalle", "Sistema");
                                    return;
                                }
                            }
                        }
                        grdData.ActiveRow.Cells["NombreProducto"].Value = frm._NombreProducto;
                        grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = frm._IdProducto;
                        grdData.ActiveRow.Cells["CodProducto"].Value = frm._CodigoInternoProducto;
                        grdData.ActiveRow.Cells["Empaque"].Value = frm._Empaque != null ? frm._Empaque.ToString() : null;
                        grdData.ActiveRow.Cells["EmpaqueUM"].Value = frm._UnidadMedida ?? string.Empty;
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque;
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque;
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque;
                        grdData.ActiveRow.Cells["d_PrecioUnitario"].Value = frm._PrecioUnitario;
                        grdData.ActiveRow.Cells["d_PrecioVenta"].Value = frm._PrecioVenta;  
                    }
                }
                if (grdData.Rows.Any())
                {
                    var aCell = grdData.Rows[e.Cell.Row.Index].Cells["d_Cantidad"];
                    grdData.Rows[e.Cell.Row.Index].Activate();
                    grdData.ActiveCell = aCell;
                    grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    grdData.Focus();
                }
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell != null)
            {
                if (grdData.ActiveCell.Column.Key != "i_IdAlmacen" && grdData.ActiveCell.Column.Key != "i_IdUnidadMedida")
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
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell.Column.Key == "d_Cantidad" || grdData.ActiveCell.Column.Key == "d_PrecioUnitario")
                CalcularValoresFila(grdData.ActiveRow);
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].EditorComponent = ucUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].EditorComponent = ucAlmacen;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }
        #endregion

        #region Clases/Validaciones
        void LlenarTemporales()
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
                                _ordendecompradetalleDto = new ordendecompradetalleDto();
                                _ordendecompradetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _ordendecompradetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _ordendecompradetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _ordendecompradetalleDto.i_IdAlmacen = Fila.Cells["d_Cantidad"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _ordendecompradetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _ordendecompradetalleDto.d_IGV = Fila.Cells["d_IGV"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_IGV"].Value.ToString());
                                _ordendecompradetalleDto.d_PrecioTotal = Fila.Cells["d_PrecioTotal"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioTotal"].Value.ToString());
                                _ordendecompradetalleDto.d_PrecioUnitario = Fila.Cells["d_PrecioUnitario"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());
                                _ordendecompradetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _ordendecompradetalleDto.d_SubTotal = Fila.Cells["d_SubTotal"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_SubTotal"].Value.ToString());
                                _ordendecompradetalleDto.i_UsadoEnCompra = 0;
                                _TempDetalle_AgregarDto.Add(_ordendecompradetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _ordendecompradetalleDto = new ordendecompradetalleDto();
                                _ordendecompradetalleDto.v_IdOrdenCompraDetalle = Fila.Cells["v_IdOrdenCompraDetalle"].Value == null ? null : Fila.Cells["v_IdOrdenCompraDetalle"].Value.ToString();
                                _ordendecompradetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _ordendecompradetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _ordendecompradetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _ordendecompradetalleDto.i_IdAlmacen = Fila.Cells["d_Cantidad"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _ordendecompradetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _ordendecompradetalleDto.d_IGV = Fila.Cells["d_IGV"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_IGV"].Value.ToString());
                                _ordendecompradetalleDto.d_PrecioTotal = Fila.Cells["d_PrecioTotal"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioTotal"].Value.ToString());
                                _ordendecompradetalleDto.d_PrecioUnitario = Fila.Cells["d_PrecioUnitario"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());
                                _ordendecompradetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _ordendecompradetalleDto.d_SubTotal = Fila.Cells["d_SubTotal"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_SubTotal"].Value.ToString());
                                _TempDetalle_ModificarDto.Add(_ordendecompradetalleDto);
                            }
                            break;
                    }
                }
            }
        }

        void FijarRegistro()
        {
            if (strModo == "Nuevo")
            {
                OperationResult objOperationResult = new OperationResult();
                txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
                txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
                txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.OrdenCompra, _ordendecompraDto.t_FechaRegistro, dtpFechaRegistro.Value, _ordendecompraDto.v_Correlativo, 0);
            }

            #region Control de Acciones
            if (new CierreMensualBL().VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.Compras))
            {
                btnGuardar.Visible = false;
                this.Text = "Orden de Compra [MES CERRADO]";
            }
            else
            {

                btnGuardar.Visible = true;
                this.Text = "Orden de Compra";
            }
            #endregion
        }

        void CargarOrdenCompras()
        {
            OperationResult objOperationResult = new OperationResult();
            switch (strModo)
            {
                case "Edicion":
                    CargarCabecera(strIdOrdenCompra);
                    _Mode = "Edit";
                    BtnImprimir.Enabled = true;
                    break;

                case "Nuevo":
                    CargarDetalle("");
                    _Mode = "New";
                    _ordendecompraDto = new ordendecompraDto();
                    txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
                    cboEstado.Value = "1";
                    cboEstado.Enabled = false;
                    txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).Trim();
                    txtCorrelativoDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000").Trim();
                    break;

                case "Guardado":
                    CargarCabecera(strIdOrdenCompra);
                    _Mode = "Edit";
                    break;
            }
        }

        void CargarCabecera(string IdOrdenCompra)
        {
            OperationResult objOperationResult = new OperationResult();

            _ordendecompraDto = _objOrdenCompraBL.ObtenerOrdenCompra(ref objOperationResult, IdOrdenCompra);

            if (objOperationResult.Success == 1)
            {
                txtPeriodo.Text = _ordendecompraDto.v_Periodo;
                txtMes.Text = _ordendecompraDto.v_Mes;
                txtProveedor.Text = _ordendecompraDto.RazonSocialProveedor;
                txtRUCProveedor.Text = _ordendecompraDto.RUCProveedor;
                txtTipoCambio.Text = _ordendecompraDto.d_TipoCambio.Value.ToString();
                txtTotal.Text = _ordendecompraDto.d_Total.Value.ToString("0.00");
                txtIGV.Text = _ordendecompraDto.d_IGV.Value.ToString("0.00");
                txtSubTotal.Text = _ordendecompraDto.d_SubTotal.Value.ToString("0.00");
                txtDocInterno.Text = _ordendecompraDto.v_DocumentoInterno;
                cboAreaSolicita.Value = _ordendecompraDto.i_IdAreaSolicita.ToString();
                cboEstado.Value = _ordendecompraDto.i_IdEstado.ToString();
                cboFormaPago.Value = _ordendecompraDto.i_IdFormaPago.ToString();
                txtCorrelativo.Text = _ordendecompraDto.v_Correlativo;
                txtObservaciones.Text = _ordendecompraDto.v_Importante;
                txtAdjuntarAnexos.Text = _ordendecompraDto.v_AdjuntarAnexo;
                txtLugarEntrega.Text = _ordendecompraDto.v_LugarEntrega;
                cboDocumento.Value = _ordendecompraDto.i_IdTipoDocumento.ToString();
                txtSerieDoc.Text = _ordendecompraDto.v_SerieDocumento;
                txtCorrelativoDoc.Text = _ordendecompraDto.v_CorrelativoDocumento;
                chkAfectoIgv.Checked = _ordendecompraDto.i_PreciosAfectosIgv == 1;
                chkIncluyeIgv.Checked = _ordendecompraDto.i_PreciosIncluyeIgv == 1;
                txtNroDias.Text = _ordendecompraDto.i_NroDias.ToString();
                txtNroCheque.Text = _ordendecompraDto.v_NroCheque;
                txtEntidadBancaria_1.Tag = _ordendecompraDto.i_IdEntidadBancaria != null ? _ordendecompraDto.i_IdEntidadBancaria.Value.ToString() : "-1";
                txtEntidadBancaria_2.Tag = _ordendecompraDto.i_IdEntidadBancaria != null ? _ordendecompraDto.i_IdEntidadBancaria.Value.ToString() : "-1";
                txtEntidadBancaria_1.Text = _ordendecompraDto.EntidadBancaria;
                txtEntidadBancaria_2.Text = _ordendecompraDto.EntidadBancaria;
                dtpFechaRegistro.Value = _ordendecompraDto.t_FechaRegistro.Value;
                dtpFechaEntrega.Value = _ordendecompraDto.t_FechaEntrega.Value;
                cboTipoOrdenCompra.Value = _ordendecompraDto.i_IdTipoOrdenCompra.ToString();
                CargarDetalle(IdOrdenCompra);
                _Mode = "Edit";
                cboEstado.Enabled = cboEstado.Value.ToString() != "2";
            }
            else
            {
                UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CargarDetalle(string IdOrdenCompra)
        {
            try
            {
                OperationResult objOperationResult = new OperationResult();
                grdData.DataSource = _objOrdenCompraBL.CargarDetalle(ref objOperationResult, IdOrdenCompra);

                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                grdData.Rows.ToList().ForEach(p => p.Cells["i_RegistroTipo"].Value = "NoTemporal");
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }

        void CalcularValoresFila(UltraGridRow Fila)
        {
            try
            {
                if (chkAfectoIgv.Checked)
                {
                    if (chkIncluyeIgv.Checked)
                    {
                        if (Fila.Cells["d_Cantidad"].Value != null && Fila.Cells["d_PrecioUnitario"].Value != null && !string.IsNullOrEmpty(Fila.Cells["d_Cantidad"].Value.ToString()) && !string.IsNullOrEmpty(Fila.Cells["d_PrecioUnitario"].Value.ToString()))
                        {
                            decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                            decimal PrecioUnitario = decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());
                            decimal Total = Utils.Windows.DevuelveValorRedondeado((Cantidad * PrecioUnitario), 2);
                            decimal SubTotal = Utils.Windows.DevuelveValorRedondeado(Total / (1 + (decimal.Parse(cboIGV.Text) / 100)), 2);
                            decimal IGV = Utils.Windows.DevuelveValorRedondeado(Total - SubTotal, 2);

                            Fila.Cells["d_SubTotal"].Value = SubTotal.ToString();
                            Fila.Cells["d_IGV"].Value = IGV.ToString();
                            Fila.Cells["d_PrecioTotal"].Value = Total.ToString();
                            CalcularTotales();
                        }
                    }
                    else
                    {
                        if (Fila.Cells["d_Cantidad"].Value != null && Fila.Cells["d_PrecioUnitario"].Value != null && !string.IsNullOrEmpty(Fila.Cells["d_Cantidad"].Value.ToString()) && !string.IsNullOrEmpty(Fila.Cells["d_PrecioUnitario"].Value.ToString()))
                        {
                            decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                            decimal PrecioUnitario = decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());
                            decimal SubTotal = Utils.Windows.DevuelveValorRedondeado((Cantidad * PrecioUnitario), 2);
                            decimal IGV = Utils.Windows.DevuelveValorRedondeado(SubTotal * (decimal.Parse(cboIGV.Text) / 100), 2);
                            decimal Total = Utils.Windows.DevuelveValorRedondeado(SubTotal + IGV, 2);
                            Fila.Cells["d_SubTotal"].Value = SubTotal.ToString();
                            Fila.Cells["d_IGV"].Value = IGV.ToString();
                            Fila.Cells["d_PrecioTotal"].Value = Total.ToString();
                            CalcularTotales();
                        }
                    }
                }
                else
                {
                    if (Fila.Cells["d_Cantidad"].Value != null && Fila.Cells["d_PrecioUnitario"].Value != null && !string.IsNullOrEmpty(Fila.Cells["d_Cantidad"].Value.ToString()) && !string.IsNullOrEmpty(Fila.Cells["d_PrecioUnitario"].Value.ToString()))
                    {
                        var cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                        decimal PrecioUnitario = decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());
                        decimal SubTotal = Utils.Windows.DevuelveValorRedondeado((cantidad * PrecioUnitario), 2);
                        decimal IGV = 0;
                        decimal Total = Utils.Windows.DevuelveValorRedondeado(SubTotal + IGV, 2);
                        Fila.Cells["d_SubTotal"].Value = SubTotal.ToString();
                        Fila.Cells["d_IGV"].Value = IGV.ToString();
                        Fila.Cells["d_PrecioTotal"].Value = Total.ToString();
                        CalcularTotales();
                    }
                }

                Fila.Cells["i_RegistroEstado"].Value = "Modificado";

            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message, "Error");
                return;
            }

        }

        void CalcularValores()
        {
            grdData.Rows.ToList().ForEach(CalcularValoresFila);
        }

        void CalcularTotales()
        {

            txtTotal.Text = grdData.Rows.Where(p => p.Cells["d_PrecioTotal"].Value != null).Sum(o => decimal.Parse(o.Cells["d_PrecioTotal"].Value.ToString())).ToString("0.00");
            txtIGV.Text = grdData.Rows.Where(p => p.Cells["d_IGV"].Value != null).Sum(o => decimal.Parse(o.Cells["d_IGV"].Value.ToString())).ToString("0.00");
            txtSubTotal.Text = grdData.Rows.Where(p => p.Cells["d_SubTotal"].Value != null).Sum(o => decimal.Parse(o.Cells["d_SubTotal"].Value.ToString())).ToString("0.00");
        }

        void CargarCombosDetalle()
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

            ucUnidadMedida.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucUnidadMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select);
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
            ucAlmacen.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucAlmacen, "Id", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaComboGrid(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            #endregion
        }

        bool ValidarValoresGrilla()
        {
            if (grdData.Rows.Where(p => p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString()) == 0).ToList().Count() != 0)
            {
                UltraMessageBox.Show("Ingrese correctamente las cantidades", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Fila = grdData.Rows.Where(p => p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString()) == 0).ToList().FirstOrDefault();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                Fila.Activate();
                Fila.Cells["d_Cantidad"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                return false;
            }
            if(string.IsNullOrEmpty(txtDocInterno.Text))
            if (grdData.Rows.Where(p => p.Cells["d_PrecioUnitario"].Value == null || decimal.Parse(p.Cells["d_PrecioUnitario"].Value.ToString()) == 0).ToList().Count() != 0)
            {
                UltraMessageBox.Show("Ingrese correctamente los precios", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Fila = grdData.Rows.Where(p => p.Cells["d_PrecioUnitario"].Value == null || decimal.Parse(p.Cells["d_PrecioUnitario"].Value.ToString()) == 0).ToList().FirstOrDefault();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                Fila.Activate();
                Fila.Cells["d_PrecioUnitario"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                return false;
            }

            if (grdData.Rows.Where(p => p.Cells["i_IdUnidadMedida"].Value == null || p.Cells["i_IdUnidadMedida"].Value.ToString() == "-1").ToList().Count() != 0)
            {
                UltraMessageBox.Show("Ingrese correctamente las unidades de medida", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Fila = grdData.Rows.Where(p => p.Cells["i_IdUnidadMedida"].Value == null || p.Cells["i_IdUnidadMedida"].Value.ToString() == "-1").ToList().FirstOrDefault();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                Fila.Activate();
                Fila.Cells["i_IdUnidadMedida"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                return false;
            }

            return true;
        }

        public void RecibirEInsertarProductoCreado(productoDto Producto)
        {
            UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
            grdData.Rows.Move(row, grdData.Rows.Count() - 1);
            this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
            row.Cells["i_RegistroEstado"].Value = "Agregado";
            row.Cells["i_RegistroTipo"].Value = "Temporal";
            row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            row.Cells["i_IdUnidadMedida"].Value = "-1";
            row.Cells["CodProducto"].Value = Producto.v_CodInterno;
            row.Cells["CodProducto"].Activate();
            DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdData.ActiveCell);
            grdData_DoubleClickCell(grdData, eventos);
        }

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            var j = false;
            foreach (UltraGridRow r in Filas)
            {
                if (grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == r.Cells["v_IdProductoDetalle"].Value.ToString()) != 0)
                {
                    UltraMessageBox.Show("El producto '" + r.Cells["v_Descripcion"].Value + "' ya se encuentra en el detalle", "Sistema");
                }
                else
                {
                    if (j == false)
                    {
                        grdData.ActiveRow.Cells["NombreProducto"].Value = r.Cells["v_Descripcion"].Value.ToString();
                        grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = r.Cells["v_IdProductoDetalle"].Value.ToString();
                        grdData.ActiveRow.Cells["CodProducto"].Value = r.Cells["v_CodInterno"].Value.ToString();
                        grdData.ActiveRow.Cells["Empaque"].Value = r.Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(r.Cells["d_Empaque"].Value.ToString());
                        grdData.ActiveRow.Cells["EmpaqueUM"].Value = r.Cells["EmpaqueUnidadMedida"].Value;
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = r.Cells["i_IdUnidadMedida"].Value;
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = r.Cells["i_IdUnidadMedida"].Value;
                        grdData.ActiveRow.Cells["d_PrecioUnitario"].Value = Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita ? "0" : r.Cells["d_Precio"].Value;
                        grdData.ActiveRow.Cells["d_PrecioVenta"].Value = Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita ? "0" : r.Cells["PrecioVenta"].Value;
                        grdData.ActiveRow.Cells["d_Cantidad"].Value = r.Cells["_Cantidad"].Value == null ? 0 : decimal.Parse(r.Cells["_Cantidad"].Value.ToString());
                        j = true;
                    }
                    else
                    {
                        UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                        grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                        row.Cells["i_IdUnidadMedida"].Value = "-1";
                        row.Cells["d_Cantidad"].Value = "0";
                        row.Cells["d_PrecioUnitario"].Value = Globals.ClientSession.v_RucEmpresa ==Constants.RucHormiguita ? "0" :r.Cells["d_Precio"].Value;
                        row.Cells["d_PrecioVenta"].Value = Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita ? "0" : r.Cells["PrecioVenta"].Value;
                        row.Cells["i_IdUnidadMedida"].Value = "-1";
                        row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        row.Activate();
                        grdData.ActiveRow.Cells["NombreProducto"].Value = r.Cells["v_Descripcion"].Value.ToString();
                        grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = r.Cells["v_IdProductoDetalle"].Value.ToString();
                        grdData.ActiveRow.Cells["CodProducto"].Value = r.Cells["v_CodInterno"].Value.ToString();
                        grdData.ActiveRow.Cells["Empaque"].Value = r.Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(r.Cells["d_Empaque"].Value.ToString());
                        grdData.ActiveRow.Cells["EmpaqueUM"].Value = r.Cells["EmpaqueUnidadMedida"].Value == null ? null : r.Cells["EmpaqueUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = r.Cells["i_IdUnidadMedida"].Value == null ? null : r.Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = r.Cells["i_IdUnidadMedida"].Value == null ? null : r.Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["d_Cantidad"].Value = r.Cells["_Cantidad"].Value == null ? 0 : decimal.Parse(r.Cells["_Cantidad"].Value.ToString());
                    }
                }
            }
            CalcularTotales();
        }
        #endregion

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            string EstadoIgv = chkAfectoIgv.Checked ? chkIncluyeIgv.Checked ? string.Format("Precios Incluyen IGV: {0}%", cboIGV.Text) : string.Format("Precios No Incluyen IGV: {0}%", cboIGV.Text) : "Precios No Afectos a IGV";
            frmVistaPreviaOrdenCompra f = new frmVistaPreviaOrdenCompra(_ordendecompraDto.v_IdOrdenCompra, EstadoIgv, cboMoneda.Value.ToString() == "1" ? 1 : 2, txtTotal.Text);
            f.ShowDialog();
        }

        private void chkAfectoIgv_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAfectoIgv.Checked)
            {
                chkIncluyeIgv.Enabled = true;
            }
            else
            {
                chkIncluyeIgv.Enabled = false;
                chkIncluyeIgv.Checked = false;
            }

            CalcularValores();
        }

        private void chkIncluyeIgv_CheckedChanged(object sender, EventArgs e)
        {
            CalcularValores();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _ordendecompradetalleDto = new ordendecompradetalleDto();
                    _ordendecompradetalleDto.v_IdOrdenCompraDetalle = grdData.ActiveRow.Cells["v_IdOrdenCompraDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_ordendecompradetalleDto);

                    grdData.ActiveRow.Delete(false);
                }
            }
            else
            {
                grdData.ActiveRow.Delete(false);
            }
            CalcularValores();
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            btnEliminar.Enabled = grdData.ActiveRow != null;
        }

        private void cboFormaPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFormaPago.Value == null) return;
            switch (cboFormaPago.Value.ToString())
            {
                case "1":
                    pTransferencia.Visible = true;
                    pCredito.Visible = false;
                    pCheque.Visible = false;
                    break;

                case "2":
                    pTransferencia.Visible = false;
                    pCredito.Visible = true;
                    pCheque.Visible = false;
                    break;

                case "3":
                    pTransferencia.Visible = false;
                    pCredito.Visible = false;
                    pCheque.Visible = true;
                    break;

                default:
                    pTransferencia.Visible = false;
                    pCredito.Visible = false;
                    pCheque.Visible = false;
                    break;
            }

            txtNroDias.Clear();
            txtEntidadBancaria_1.Clear();
            txtEntidadBancaria_1.Tag = null;
            txtEntidadBancaria_2.Clear();
            txtEntidadBancaria_2.Tag = null;
            txtNroCheque.Clear();
        }

        private void txtNroDias_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroDias, e);
        }

        private void txtEntidadBancaria_1_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmBuscarDatahierarchy f = new frmBuscarDatahierarchy(111, "Entidades Bancarias");
            f.ShowDialog();
            if (f._itemId != null)
            {
                txtEntidadBancaria_1.Tag = f._itemId;
                txtEntidadBancaria_1.Text = f._value1;

                txtEntidadBancaria_2.Tag = f._itemId;
                txtEntidadBancaria_2.Text = f._value1;
            }
        }

        private void txtEntidadBancaria_2_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            using (var f = new frmBuscarDatahierarchy(111, "Entidades Bancarias"))
            {
                f.ShowDialog();
                if (f._itemId != null)
                {
                    txtEntidadBancaria_1.Tag = f._itemId;
                    txtEntidadBancaria_1.Text = f._value1;

                    txtEntidadBancaria_2.Tag = f._itemId;
                    txtEntidadBancaria_2.Text = f._value1;
                }
            }
        }

        private void btnMatriz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProveedor.Tag as string))
            {
                UltraMessageBox.Show("Requiere Proveedor para crear articulos");
                txtProveedor.Focus();
                return;
            }
            using (var frm = new frmGenerarProductoMatriz(true) {CodigoProveedor = (string) txtProveedor.Tag})
            {
                var idsTabla =  grdData.Rows.Where(r => r.Cells["v_IdProductoDetalle"].Value != null)
                                .Select(r => r.Cells["v_IdProductoDetalle"].Value.ToString())
                                .ToList();

                if (frm.ShowDialog() != DialogResult.OK) return;
                var result = frm.Resultado;
                foreach (var item in result)
                {
                    if (idsTabla.Contains(item["CodProd"]))
                    {
                        UltraMessageBox.Show("El producto ya existe en el detalle", "Sistema");
                        return;
                    }
                    var row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    row.Cells["NombreProducto"].Value = item["NomProd"];
                    row.Cells["v_IdProductoDetalle"].Value = item["IdProd"];
                    row.Cells["CodProducto"].Value = item["CodProd"];
                    row.Cells["d_PrecioUnitario"].Value = item["PrecioUnitario"];
                    row.Cells["d_PrecioVenta"].Value = item["PrecioVenta"];
                    row.Cells["Empaque"].Value = "1.000";
                    row.Cells["EmpaqueUM"].Value = "UNIDAD";
                    row.Cells["i_IdUnidadMedida"].Value = 15;
                    row.Cells["i_IdUnidadMedidaProducto"].Value = 15;
                    row.Cells["d_Cantidad"].Value = 0M;
                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
        /// <summary>
        /// Imprime etiquetas.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrintLabel_Click(object sender, EventArgs e)
        {
            var products = grdData.Rows.Select(r =>
            {
                var obj = (ordendecompradetalleDto) r.ListObject;
                return new Product
                {
                    Codigo = obj.CodProducto,
                    Descripcion = obj.NombreProducto,
                    Cantidad = (int)(obj.d_Cantidad ?? 1),
                    Precio = obj.d_PrecioVenta ?? 0
                };
            });

            if (sender == btnPrintCodes)
            {
                using (var frm = new frmListadeImpresionPopup())
                {
                    frm.LoadItems(products);
                    frm.ShowDialog();
                }
            }
            else
            {
                var document = txtSerieDoc.Text + "-" + txtCorrelativoDoc.Text;
                using (var frm = new frmOrdenDeAvios(document) { Glosa = txtObservaciones.Text })
                {
                    frm.LoadItems(products);
                    frm.ShowDialog();
                }
            }
        }
    }
}
