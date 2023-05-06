using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Requerimientos.NBS;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmOrdenTrabajo : Form
    {



        OrdenTrabajoBL _objOrdenTrabajoBL = new OrdenTrabajoBL();
        public string strModo = string.Empty;
        public string v_IdOrdenTrabajo = "";
        PedidoBL _objPedidoBL = new PedidoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        nbs_ordentrabajoDto _ordenTrabajoDto = new nbs_ordentrabajoDto();
        nbs_ordentrabajodetalleDto _ordenTrabajoDetalleDto = new nbs_ordentrabajodetalleDto();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<nbs_ordentrabajodetalleDto> _TempDetalle_AgregarDto = new List<nbs_ordentrabajodetalleDto>();
        List<nbs_ordentrabajodetalleDto> _TempDetalle_ModificarDto = new List<nbs_ordentrabajodetalleDto>();
        List<nbs_ordentrabajodetalleDto> _TempDetalle_EliminarDto = new List<nbs_ordentrabajodetalleDto>();
        ClienteBL _objClienteBL = new ClienteBL();
        public frmOrdenTrabajo(string Modo, string IdOrdenTrabajo)
        {
            InitializeComponent();
            strModo = Modo;
            v_IdOrdenTrabajo = IdOrdenTrabajo;
        }

        private void frmOrdenTrabajo_Load(object sender, EventArgs e)
        {
            BackColor = Color.White;
            CargarCombos();
            ultraPanel1.BackColor = Color.FromArgb(233, 233, 233);
            ObtenerListadoOrdenTrabajo(Globals.ClientSession.i_Periodo.Value.ToString("00"), DateTime.Now.Month.ToString("00"));
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_PrecioDecimales);
            ValidarFechas();
        }

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstados, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 155, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboResponsableInterno, "Value1", "Id", new ClienteBL().BuscarAnexoForCombo(ref objOperationResult, "R"), DropDownListAction.Select);
            cboEstados.Value = "1";
            cboResponsableInterno.Value = "-1";
        
        }

        private void ObtenerListadoOrdenTrabajo(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            List<KeyValueDTO> _ListadoOrdenTrabajo = new List<KeyValueDTO>();

            switch (strModo)
            {
                case "Edicion":
                    CargarCabecera(v_IdOrdenTrabajo);
                    break;

                case "Nuevo":
                    LimpiarCabecera();
                    CargarDetalle("");
                    break;
            }

        }

        private void LimpiarCabecera()
        {
            var xx = _ordenTrabajoDto.t_FechaRegistro;
            OperationResult objOperationResult = new OperationResult();
            txtMes.Text = DateTime.Now.Date.Month.ToString("00");
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            //txtResponsableInterno.Text = Globals.ClientSession.v_UserName;
            dtpFechaRegistro.Value = DateTime.Now;
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.OrdenTrabajo, null, DateTime.Parse(dtpFechaRegistro.Value.ToString()), _ordenTrabajoDto.v_Correlativo == null ? "" : _ordenTrabajoDto.v_Correlativo, 0);
            txtNroOrdenTrabajo.Text = _objDocumentoBL.CorrelativoxSerie((int)TiposDocumentos.OrdenTrabajo, Globals.ClientSession.i_Periodo.Value.ToString("0000"));

        }

        private void CargarCabecera(string pstrIdOrdenTrabajo)
        {
            OperationResult objOperationResult = new OperationResult();
            _ordenTrabajoDto = _objOrdenTrabajoBL.ObtenerOrdenTrabajoCabecera(ref objOperationResult, pstrIdOrdenTrabajo);
            if (_ordenTrabajoDto != null)
            {
                txtMes.Text = _ordenTrabajoDto.v_Mes;
                txtCorrelativo.Text = _ordenTrabajoDto.v_Correlativo;
                txtPeriodo.Text = _ordenTrabajoDto.v_Periodo;
                txtNroOrdenTrabajo.Text = _ordenTrabajoDto.v_NroOrdenTrabajo;
                txtRucCliente.Text = _ordenTrabajoDto.RucCliente.Trim();
                txtRazonSocialCliente.Text = _ordenTrabajoDto.RazonSocialCliente;
                txtDireccionCliente.Text = _ordenTrabajoDto.DireccionCliente;
                cboResponsableInterno.Value = _ordenTrabajoDto.v_IdResponsable;
                txtReferencia.Text = _ordenTrabajoDto.v_Referencia;
                txtGlosa.Text = _ordenTrabajoDto.v_Glosa;
                txtNroOrdenCompra.Text = _ordenTrabajoDto.v_NroOrdenCompra;
                txtTotal.Text = Utils.Windows.DevuelveValorRedondeado(_ordenTrabajoDto.d_Total.Value, 2).ToString();
                txtTotalRegistral.Text = Utils.Windows.DevuelveValorRedondeado(_ordenTrabajoDto.d_TotalRegistral.Value, 2).ToString();
                cboEstados.Value = _ordenTrabajoDto.i_IdEstado.Value.ToString();
                CargarDetalle(pstrIdOrdenTrabajo);

            }



        }

        private void CargarDetalle(string pstrOrdenTrabajo)
        {
            OperationResult objOperationResult = new OperationResult();
            grData.DataSource = _objOrdenTrabajoBL.ObtenerDetalle(ref objOperationResult, pstrOrdenTrabajo);
            for (int i = 0; i < grData.Rows.Count(); i++)
            {
                grData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                grData.Rows[i].Cells["i_RegistroEstado"].Value = "NoModificado";
            }
            CalcularTotales();
        }



        private void txtNroOrdenTrabajo_Validating(object sender, CancelEventArgs e)
        {

            if (strModo == "Nuevo")
            {

                txtNroOrdenTrabajo.Text = _objDocumentoBL.CorrelativoxSerie((int)TiposDocumentos.OrdenTrabajo, Globals.ClientSession.i_Periodo.Value.ToString("0000"));

            }
            else
            {
                //  txtNroOrdenTrabajo.Text = string.Empty;

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (ValidarFormulario.Validate(true, false).IsValid)
            {
                if (_ordenTrabajoDto.v_IdCliente == null)
                {
                    UltraMessageBox.Show("Por favor ingrese un Cliente Válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

                if (!grData.Rows.Any())
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                if (strModo == "Edicion")
                {
                    string fut = _objOrdenTrabajoBL.BuscarNumeroFormatoUnico(_ordenTrabajoDto.v_IdOrdenTrabajo);
                    if (fut != string.Empty)
                    {

                        UltraMessageBox.Show("Imposible Guardar Cambios , Orden Trabajo ya está asociado a FUT : " + fut, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                if (ValidaCamposNulosVacios())
                {
                    if (strModo == "Nuevo")
                    {

                        while (_objOrdenTrabajoBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }
                        while (_objOrdenTrabajoBL.ExisteNroDocumento(txtNroOrdenCompra.Text) == false)
                        {
                            txtNroOrdenCompra.Text = (int.Parse(txtNroOrdenCompra.Text) + 1).ToString("00000000");
                        }
                        _ordenTrabajoDto.v_Mes = txtMes.Text.Trim();
                        _ordenTrabajoDto.v_Correlativo = txtCorrelativo.Text.Trim();
                        _ordenTrabajoDto.v_Periodo = txtPeriodo.Text.Trim();
                        _ordenTrabajoDto.v_NroOrdenTrabajo = txtNroOrdenTrabajo.Text.Trim();
                        _ordenTrabajoDto.v_Referencia = txtReferencia.Text.Trim();
                        _ordenTrabajoDto.v_Glosa = txtGlosa.Text.Trim();
                        _ordenTrabajoDto.t_FechaRegistro = DateTime.Parse(dtpFechaRegistro.Value.ToString());
                        _ordenTrabajoDto.v_NroOrdenCompra = txtNroOrdenCompra.Text.Trim();
                        _ordenTrabajoDto.d_Total = decimal.Parse(txtTotal.Text);
                        _ordenTrabajoDto.d_TotalRegistral = decimal.Parse(txtTotalRegistral.Text);
                        _ordenTrabajoDto.i_IdEstado = int.Parse(cboEstados.Value.ToString());
                        _ordenTrabajoDto.v_IdResponsable = cboResponsableInterno.Value.ToString();
                        LlenarTemporales();
                        v_IdOrdenTrabajo = _objOrdenTrabajoBL.InsertarOrdenTrabajo(ref objOperationResult, _ordenTrabajoDto, _TempDetalle_AgregarDto, Globals.ClientSession.GetAsList());

                    }
                    else
                    {

                        _ordenTrabajoDto.v_Mes = txtMes.Text.Trim();
                        _ordenTrabajoDto.v_Correlativo = txtCorrelativo.Text.Trim();
                        _ordenTrabajoDto.v_Periodo = txtPeriodo.Text.Trim();
                        _ordenTrabajoDto.v_NroOrdenTrabajo = txtNroOrdenTrabajo.Text.Trim();
                        _ordenTrabajoDto.v_Referencia = txtReferencia.Text.Trim();
                        _ordenTrabajoDto.v_Glosa = txtGlosa.Text.Trim();
                        _ordenTrabajoDto.t_FechaRegistro = DateTime.Parse(dtpFechaRegistro.Value.ToString());
                        _ordenTrabajoDto.v_NroOrdenCompra = txtNroOrdenCompra.Text.Trim();
                        _ordenTrabajoDto.d_Total = decimal.Parse(txtTotal.Text);
                        _ordenTrabajoDto.d_TotalRegistral = decimal.Parse(txtTotalRegistral.Text);
                        _ordenTrabajoDto.i_IdEstado = int.Parse(cboEstados.Value.ToString());
                        _ordenTrabajoDto.v_IdResponsable = cboResponsableInterno.Value.ToString();
                        LlenarTemporales();
                        v_IdOrdenTrabajo = _objOrdenTrabajoBL.ActualizaOrdenTrabajo(ref objOperationResult, _ordenTrabajoDto, _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto, Globals.ClientSession.GetAsList());

                    }
                    if (objOperationResult.Success == 1)
                    {
                        UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        strModo = "Edicion";
                        ObtenerListadoOrdenTrabajo(txtPeriodo.Text, txtMes.Text.Trim());

                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al guardar ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);


                    }
                    _TempDetalle_AgregarDto = new List<nbs_ordentrabajodetalleDto>();
                    _TempDetalle_EliminarDto = new List<nbs_ordentrabajodetalleDto>();
                    _TempDetalle_ModificarDto = new List<nbs_ordentrabajodetalleDto>();
                }
            }
        }

        private bool ValidaCamposNulosVacios() //1
        {
            if (grData.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value == null || p.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todos los Servicios", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grData.Rows.Where(x => x.Cells["v_IdProductoDetalle"].Value == null || x.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grData.Selected.Cells.Add(Row.Cells["CodigoProducto"]);
                grData.Focus();
                Row.Activate();
                grData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grData.ActiveRow.Cells["CodigoProducto"];
                this.grData.ActiveCell = aCell;
                return false;
            }

            if (grData.Rows.Where(p => p.Cells["i_Cantidad"].Value == null || p.Cells["i_Cantidad"].Value.ToString().Trim() == string.Empty || int.Parse(p.Cells["i_Cantidad"].Value.ToString()) == 0).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todas las cantidades", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grData.Rows.Where(x => x.Cells["i_Cantidad"].Value == null || x.Cells["i_Cantidad"].Value.ToString().Trim() == string.Empty || int.Parse(x.Cells["i_Cantidad"].Value.ToString()) == 0).FirstOrDefault();
                grData.Selected.Cells.Add(Row.Cells["i_Cantidad"]);
                grData.Focus();
                Row.Activate();
                grData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grData.ActiveRow.Cells["i_Cantidad"];
                this.grData.ActiveCell = aCell;
                return false;
            }

            if (grData.Rows.Where(p => (p.Cells["d_Importe"].Value == null || p.Cells["d_Importe"].Value.ToString().Trim() == string.Empty || decimal.Parse(p.Cells["d_Importe"].Value.ToString()) == 0) && !p.Cells["CodigoProducto"].Value.ToString().StartsWith("I")).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todos los Importes Registrales", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grData.Rows.Where(x => (x.Cells["d_Importe"].Value == null || x.Cells["d_Importe"].Value.ToString().Trim() == string.Empty || decimal.Parse(x.Cells["d_Importe"].Value.ToString()) == 0) && !x.Cells["CodigoProducto"].Value.ToString().StartsWith("I")).FirstOrDefault();
                grData.Selected.Cells.Add(Row.Cells["d_Importe"]);
                grData.Focus();
                Row.Activate();
                grData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grData.ActiveRow.Cells["d_Importe"];
                this.grData.ActiveCell = aCell;
                return false;
            }



            if (grData.Rows.Where(p => (p.Cells["d_ImporteRegistral"].Value == null || p.Cells["d_ImporteRegistral"].Value.ToString().Trim() == string.Empty || decimal.Parse(p.Cells["d_ImporteRegistral"].Value.ToString()) == 0) && p.Cells["CodigoProducto"].Value.ToString().StartsWith("I")).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todos los Importes Notariales", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grData.Rows.Where(x => (x.Cells["d_ImporteRegistral"].Value == null || x.Cells["d_ImporteRegistral"].Value.ToString().Trim() == string.Empty || decimal.Parse(x.Cells["d_ImporteRegistral"].Value.ToString()) == 0) && x.Cells["CodigoProducto"].Value.ToString().StartsWith("I")).FirstOrDefault();
                grData.Selected.Cells.Add(Row.Cells["d_ImporteRegistral"]);
                grData.Focus();
                Row.Activate();
                grData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grData.ActiveRow.Cells["d_ImporteRegistral"];
                this.grData.ActiveCell = aCell;
                return false;
            }
            return true;
        }

        private void LlenarTemporales()
        {

            try
            {
                if (!grData.Rows.Any()) return;
                foreach (var fila in grData.Rows)
                {
                    switch (fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                var _objOrdenTrabajoDetalle = (nbs_ordentrabajodetalleDto)fila.ListObject;
                                _TempDetalle_AgregarDto.Add(_objOrdenTrabajoDetalle);
                            }
                            break;

                        case "NoTemporal":
                            if (fila.Cells["i_RegistroEstado"].Value != null && fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                var _objOrdenTrabajoDetalle = (nbs_ordentrabajodetalleDto)fila.ListObject;
                                _TempDetalle_ModificarDto.Add(_objOrdenTrabajoDetalle);

                            }
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "Linea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }


        private void btnAgregar_Click(object sender, EventArgs e)
        {

            if (ValidarCliente.Validate(true, false).IsValid)
            {

                if (_ordenTrabajoDto.v_IdCliente == null)
                {
                    UltraMessageBox.Show("Por favor ingrese un Cliente Válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var ultimaFila = grData.Rows.LastOrDefault();
                if (ultimaFila == null || ultimaFila.Cells["v_IdProductoDetalle"].Value != null)
                {

                    UltraGridRow row = grData.DisplayLayout.Bands[0].AddNew();
                    grData.Rows.Move(row, grData.Rows.Count() - 1);
                    this.grData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["d_Importe"].Value = "0";
                    row.Cells["d_Total"].Value = "0";

                    grData.Focus();
                    UltraGridCell aCell = this.grData.ActiveRow.Cells["CodigoProducto"];
                    this.grData.ActiveCell = aCell;
                    grData.ActiveRow = aCell.Row;
                    grData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    aCell.Activate();
                    grData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                }
            }
        }

        private void grData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            switch (e.Cell.Column.Key)
            {

                case ("CodigoProducto"):
                    if (_objPedidoBL.EsValidoCodProducto(e.Cell.Text) && (grData.Rows[grData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "Temporal"))
                    {

                        productoshortDto Producto = _objPedidoBL.DevolverArticuloPorCodInternoNuevo(ref objOperationResult, null, null, Globals.ClientSession.i_IdAlmacenPredeterminado.Value, _ordenTrabajoDto.v_IdCliente, e.Cell.Text.Trim().ToUpper()).FirstOrDefault();
                        if (Producto != null)
                        {
                            grData.ActiveRow.Cells["DescripcionProducto"].Value = Producto.v_Descripcion.Trim();
                            grData.ActiveRow.Cells["CodigoProducto"].Value = Producto.v_CodInterno;
                            grData.ActiveRow.Cells["v_IdProductoDetalle"].Value = Producto.v_IdProductoDetalle;
                            grData.ActiveRow.Cells["d_Importe"].Value = DevolverPrecioProducto(Producto.IdMoneda, Producto.d_Precio ?? 0, 1, 0);
                            grData.ActiveRow.Cells["i_Cantidad"].Value = "1";
                            grData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                            grData.ActiveRow.Cells["EsNombreEditable"].Value = Producto.i_NombreEditable;
                        }
                    }
                    else
                    {

                        Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(int.Parse(Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString()), "PedidoVenta", _ordenTrabajoDto.v_IdCliente, grData.Rows[grData.ActiveRow.Index].Cells["CodigoProducto"].Text == null ? string.Empty : grData.Rows[grData.ActiveRow.Index].Cells["CodigoProducto"].Text.ToString(), UserConfig.Default.appTipoBusquedaPredeterminadaProducto);
                        frm.ShowDialog();
                        if (frm._NombreProducto != null)
                        {
                            foreach (UltraGridRow Fila in grData.Rows)
                            {
                                if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                                {

                                    if (frm._IdProducto == Fila.Cells["v_IdProductoDetalle"].Value.ToString())
                                    {
                                        UltraMessageBox.Show("El producto ya existe en la Lista de Servicios ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                }
                            }

                            grData.Rows[grData.ActiveRow.Index].Cells["DescripcionProducto"].Value = frm._NombreProducto;
                            grData.Rows[grData.ActiveRow.Index].Cells["CodigoProducto"].Value = frm._CodigoInternoProducto;
                            grData.Rows[grData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = frm._IdProducto;
                            grData.Rows[grData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                            grData.Rows[grData.ActiveRow.Index].Cells["d_Importe"].Value = DevolverPrecioProducto(frm._IdMoneda, frm._PrecioUnitario, 1, 0);
                            grData.ActiveRow.Cells["i_Cantidad"].Value = "1";
                            grData.ActiveRow.Cells["EsNombreEditable"].Value = frm._NombreEditable;

                            //UltraGridCell aCell = grData.Rows[e.Cell.Row.Index].Cells["i_Cantidad"];
                            //grData.Rows[e.Cell.Row.Index].Activate();
                            //grData.ActiveCell = aCell;
                            //grData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                            //grData.Focus();
                        }


                    }
                    UltraGridCell aCell = grData.Rows[e.Cell.Row.Index].Cells["i_Cantidad"];
                    grData.Rows[e.Cell.Row.Index].Activate();
                    grData.ActiveCell = aCell;
                    grData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    grData.Focus();
                    // CalcularValoresDetalle();
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


        private void FormatoDecimalesGrilla(int DecimalesPrecio)
        {
            string FormatoPrecio;

            UltraGridColumn _dImporteNotarial = this.grData.DisplayLayout.Bands[0].Columns["d_Importe"];
            UltraGridColumn _dImporteRegistral = this.grData.DisplayLayout.Bands[0].Columns["d_ImporteRegistral"];

            _dImporteNotarial.MaskDataMode = MaskMode.IncludeLiterals;
            _dImporteRegistral.MaskDisplayMode = MaskMode.IncludeLiterals;

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
            _dImporteNotarial.MaskInput = "-" + FormatoPrecio;
            _dImporteRegistral.MaskInput = "-" + FormatoPrecio;
        }

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            bool ExistenciaGrilla = false, anteriorRegistro = false;
            bool Saldo0 = false;
            int mensajeEg = 0, cantMensajes = 0;

            for (int i = 0; i < Filas.Count; i++)
            {
                ExistenciaGrilla = false;
                Saldo0 = false;

                foreach (UltraGridRow Fila in grData.Rows)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (Filas[i].Cells["v_IdProductoDetalle"].Value.ToString() == Fila.Cells["v_IdProductoDetalle"].Value.ToString())
                        {
                            if (mensajeEg == 0)
                            {
                                UltraMessageBox.Show("Uno de los productos seleccionados ya existe en la Lista Servicios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ExistenciaGrilla = true;
                                mensajeEg = mensajeEg + 1;
                            }
                            else
                            {
                                ExistenciaGrilla = true;
                                mensajeEg = mensajeEg + 1;
                            }

                        }

                    }
                }
                if (i == 0)
                {
                    if (!Saldo0)
                    {
                        if (!ExistenciaGrilla)
                        {

                            grData.Rows[grData.ActiveRow.Index].Cells["DescripcionProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                            grData.Rows[grData.ActiveRow.Index].Cells["CodigoProducto"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                            grData.Rows[grData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                            grData.Rows[grData.ActiveRow.Index].Cells["d_Importe"].Value = DevolverPrecioProducto(1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), 1, 0);
                            grData.Rows[grData.ActiveRow.Index].Cells["i_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : int.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                            grData.Rows[grData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                            grData.Rows[grData.ActiveRow.Index].Cells["i_RegistroTipo"].Value = "Temporal";
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
                                grData.Rows[grData.ActiveRow.Index].Cells["DescripcionProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                                grData.Rows[grData.ActiveRow.Index].Cells["CodigoProducto"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                                grData.Rows[grData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                                grData.Rows[grData.ActiveRow.Index].Cells["d_Importe"].Value = DevolverPrecioProducto(1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), 1, 0);
                                grData.Rows[grData.ActiveRow.Index].Cells["i_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : int.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                                grData.Rows[grData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                grData.Rows[grData.ActiveRow.Index].Cells["i_RegistroTipo"].Value = "Temporal";
                                anteriorRegistro = false;

                            }
                            else
                            {
                                UltraGridRow row = grData.DisplayLayout.Bands[0].AddNew();
                                grData.Rows.Move(row, grData.Rows.Count() - 1);
                                this.grData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                row.Cells["i_RegistroEstado"].Value = "Agregado";
                                row.Cells["i_RegistroTipo"].Value = "Temporal";

                                row.Cells["i_Cantidad"].Value = "1";
                                row.Cells["d_Importe"].Value = "0";

                                row.Activate();
                                grData.Rows[grData.ActiveRow.Index].Cells["DescripcionProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                                grData.Rows[grData.ActiveRow.Index].Cells["CodigoProducto"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                                grData.Rows[grData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                                grData.Rows[grData.ActiveRow.Index].Cells["d_Importe"].Value = DevolverPrecioProducto(1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), 1, 0);
                                grData.Rows[grData.ActiveRow.Index].Cells["i_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : int.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                                grData.Rows[grData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                            }
                        }
                    }
                }
            }

            CalcularValoresDetalle();
        }
        private void CalcularValoresDetalle()
        {
            if (grData.Rows.Count() == 0) return;
            foreach (UltraGridRow Fila in grData.Rows)
            {
                CalcularValoresFila(Fila);
            }

        }

        private void CalcularValoresFila(UltraGridRow Fila)
        {



            if (Fila.Cells["d_Importe"].Value == null) { Fila.Cells["d_Importe"].Value = "0"; }
            if (Fila.Cells["d_ImporteRegistral"].Value == null) { Fila.Cells["d_ImporteRegistral"].Value = "0"; }
            if (Fila.Cells["i_Cantidad"].Value == null) { Fila.Cells["i_Cantidad"].Value = "0"; }
            Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado(((decimal.Parse(Fila.Cells["d_Importe"].Value.ToString()) + decimal.Parse(Fila.Cells["d_ImporteRegistral"].Value.ToString()))) * decimal.Parse(Fila.Cells["i_Cantidad"].Value.ToString()), 2).ToString();
            // Fila.Cells["d_Total"].Value = Fila.Cells["d_Importe"].Value == null || Fila.Cells["i_Cantidad"].Value == null || Fila.Cells["d_Importe"].Value.ToString() == "0" || Fila.Cells["i_Cantidad"].Value.ToString() == "0" ? "0.00" : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Importe"].Value.ToString()) * int.Parse(Fila.Cells["i_Cantidad"].Value.ToString())), 2).ToString();
            CalcularTotales();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grData.ActiveRow == null) return;
            if (grData.Rows[grData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {

                if (int.Parse(grData.Rows[grData.ActiveRow.Index].Cells["i_UsadoEnFUF"].Value.ToString()) == 1)
                {

                    string NumeroFormato = grData.Rows[grData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null && grData.Rows[grData.ActiveRow.Index].Cells["v_IdOrdenTrabajo"].Value != null ? _objOrdenTrabajoBL.BuscarNumeroFormatoUnico(grData.Rows[grData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString(), grData.Rows[grData.ActiveRow.Index].Cells["v_IdOrdenTrabajo"].Value.ToString()) : string.Empty;
                    UltraMessageBox.Show("No se puede Eliminar,Este servicio esta siendo utilizado en Formato Unico de Factuación :  " + NumeroFormato, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _ordenTrabajoDetalleDto = new nbs_ordentrabajodetalleDto();
                    _ordenTrabajoDetalleDto.v_IdOrdenTrabajoDetalle = grData.Rows[grData.ActiveRow.Index].Cells["v_IdOrdenTrabajoDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_ordenTrabajoDetalleDto);
                    grData.Rows[grData.ActiveRow.Index].Delete(false);
                }
            }
            else
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    grData.Rows[grData.ActiveRow.Index].Delete(false);
                }
            }
            CalcularValoresDetalle();
        }

        private void grData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grData.ActiveCell == null || grData.ActiveCell.Column == null || grData.ActiveCell.Column.Key == null) return;

            switch (this.grData.ActiveCell.Column.Key)
            {


                case "CodigoProducto":

                    if (grData.Rows[grData.ActiveRow.Index].Cells["CodigoProducto"].Value != null && grData.Rows[grData.ActiveRow.Index].Cells["CodigoProducto"].Value.ToString().StartsWith("I")) // Registral
                    {
                        grData.Rows[grData.ActiveRow.Index].Cells["d_Importe"].Activation = Activation.Disabled;
                        grData.Rows[grData.ActiveRow.Index].Cells["d_Importe"].Activation = Activation.NoEdit;
                        grData.Rows[grData.ActiveRow.Index].Cells["d_ImporteRegistral"].Activation = Activation.ActivateOnly;
                        grData.Rows[grData.ActiveRow.Index].Cells["d_ImporteRegistral"].Activation = Activation.AllowEdit;


                    }
                    else
                    {

                        grData.Rows[grData.ActiveRow.Index].Cells["d_Importe"].Activation = Activation.ActivateOnly;
                        grData.Rows[grData.ActiveRow.Index].Cells["d_Importe"].Activation = Activation.AllowEdit;
                        grData.Rows[grData.ActiveRow.Index].Cells["d_ImporteRegistral"].Activation = Activation.Disabled;
                        grData.Rows[grData.ActiveRow.Index].Cells["d_ImporteRegistral"].Activation = Activation.NoEdit;
                    }


                    break;


                case "DescripcionProducto":
                    if (grData.Rows[grData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                    {
                        grData.Rows[grData.ActiveRow.Index].Cells["v_DescripcionTemporal"].Value = grData.Rows[grData.ActiveRow.Index].Cells["DescripcionProducto"].Value == null ? "" : grData.Rows[grData.ActiveRow.Index].Cells["DescripcionProducto"].Value.ToString().Trim();
                    }
                    break;
            }


            CalcularValoresFila(grData.Rows[grData.ActiveRow.Index]);


        }

        private void CalcularTotales()
        {
            decimal Total = 0, TotalRegistral = 0, TotalNotarial = 0;
            foreach (UltraGridRow Fila in grData.Rows)
            {
                //Registral --Irp
                if (Fila.Cells["CodigoProducto"].Value != null && !Fila.Cells["CodigoProducto"].Value.ToString().StartsWith("I"))
                {
                    TotalNotarial = Fila.Cells["d_Importe"].Value == null ? Utils.Windows.DevuelveValorRedondeado(TotalNotarial, 2) : Utils.Windows.DevuelveValorRedondeado(TotalNotarial + decimal.Parse(Fila.Cells["d_Total"].Value.ToString()), 2);
                }
                if (Fila.Cells["CodigoProducto"].Value != null && Fila.Cells["CodigoProducto"].Value.ToString().StartsWith("I"))
                {
                    TotalRegistral = Fila.Cells["d_ImporteRegistral"].Value == null ? Utils.Windows.DevuelveValorRedondeado(TotalRegistral, 2) : Utils.Windows.DevuelveValorRedondeado(TotalRegistral + decimal.Parse(Fila.Cells["d_Total"].Value.ToString()), 2);
                }
                Total = Fila.Cells["d_Total"].Value == null ? Utils.Windows.DevuelveValorRedondeado(Total, 2) : Utils.Windows.DevuelveValorRedondeado(Total + decimal.Parse(Fila.Cells["d_Total"].Value.ToString()), 2);
              

            }
            txtTotalNotarial.Text = Utils.Windows.DevuelveValorRedondeado(TotalNotarial, 2).ToString();
            txtTotal.Text = Utils.Windows.DevuelveValorRedondeado(Total, 2).ToString();
            txtTotalRegistral.Text = Utils.Windows.DevuelveValorRedondeado(TotalRegistral, 2).ToString();
        }

        private void grData_CellChange(object sender, CellEventArgs e)
        {
            grData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void grData_MouseDown(object sender, MouseEventArgs e)
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

        private void grData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grData.ActiveCell == null || grData.ActiveCell.Column == null || grData.ActiveCell.Column.Key == null) return;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    grData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                    grData.PerformAction(UltraGridAction.AboveCell, false, false);
                    e.Handled = true;
                    grData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    break;
                case Keys.Down:
                    grData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                    grData.PerformAction(UltraGridAction.BelowCell, false, false);
                    e.Handled = true;
                    grData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    break;
                case Keys.Right:
                    grData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                    grData.PerformAction(UltraGridAction.NextCellByTab, false, false);
                    e.Handled = true;
                    grData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    break;
                case Keys.Left:
                    grData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                    grData.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                    e.Handled = true;
                    grData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    break;
                case Keys.Enter:
                    DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grData.ActiveCell);
                    grData_DoubleClickCell(sender, eventos);
                    e.Handled = true;
                    break;
            }

            switch (grData.ActiveCell.Column.Key)
            {
                case "DescripcionProducto":

                    if (grData.Rows[grData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                    {

                        if (grData.Rows[grData.ActiveRow.Index].Cells["EsNombreEditable"].Value.ToString() == "0")
                        {
                            e.SuppressKeyPress = true;
                        }

                    }
                    break;



            }
        }

        private void grData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grData.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grData.ActiveCell.Column.Key)
                {
                    case "i_Cantidad":

                        Celda = grData.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;
                    case "d_Importe":
                        Celda = grData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_ImporteRegistral":
                        Celda = grData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;
                }
            }
        }

        private void grData_ClickCellButton(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {

                case "CodigoProducto":
                    UltraGridCell celda = this.grData.ActiveRow.Cells["CodigoProducto"];
                    DoubleClickCellEventArgs f = new DoubleClickCellEventArgs(celda);
                    grData_DoubleClickCell(sender, f);
                    break;


                case "DescripcionProducto":
                    var des = new FrmEditarDescripcion(e.Cell.Value != null ? e.Cell.Value.ToString() : string.Empty);
                    des.FormClosing += delegate
                    {
                        e.Cell.Value = des.Descripcion;
                    };

                    des.ShowDialog();
                    break;
            }

      

        }

        private void ValidarFechas()
        {
            if (DateTime.Now.Year.ToString().Trim() == txtPeriodo.Text.Trim())
            {


                if (strModo == "Nuevo")
                {
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                }
                else
                {
                    if (int.Parse(_ordenTrabajoDto.v_Mes.Trim()) <= int.Parse(DateTime.Now.Month.ToString()))
                    {
                        dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    }
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                }


            }
            else
            {
                if (strModo == "Nuevo")
                {
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());

                }
                else
                {

                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/01").ToString());
                    dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());

                }

            }
        }

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (strModo == "Nuevo")
            {
                txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.OrdenTrabajo, null, DateTime.Parse(dtpFechaRegistro.Value.ToString()), _ordenTrabajoDto.v_Correlativo == null ? "" : _ordenTrabajoDto.v_Correlativo, 0);
                txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
                txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            }
            else
            {
                string MesCambiado = int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? (dtpFechaRegistro.Value.Month.ToString("00")).Trim() : dtpFechaRegistro.Value.Month.ToString();
                string AnioCambiado = dtpFechaRegistro.Value.Year.ToString().Trim();
                if (MesCambiado.Trim() != _ordenTrabajoDto.v_Mes.Trim() || AnioCambiado != _ordenTrabajoDto.v_Periodo.Trim())
                {
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.OrdenTrabajo, null, DateTime.Parse(dtpFechaRegistro.Value.ToString()), _ordenTrabajoDto.v_Correlativo == null ? "" : _ordenTrabajoDto.v_Correlativo, 0);
                    txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
                    txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
                }
                else
                {
                    txtPeriodo.Text = _ordenTrabajoDto.v_Periodo.Trim();
                    txtMes.Text = _ordenTrabajoDto.v_Mes.Trim();
                    txtCorrelativo.Text = _ordenTrabajoDto.v_Correlativo.Trim();
                }
            }
            //if (_objCierreMensualBL.VerificarMesCerrado(txtPeriodo.Text.Trim(), txtMes.Text.Trim(), "ALMACÉN")|| Utilizado =="KARDEX")
            //{
            //    btnGuardar.Visible = false;
            //   // this.Text = "Nota de Salida [MES CERRADO]";
            //    this.Text = Utilizado == "KARDEX" ? "Nota de Salida" : "Nota de Salida [MES CERRADO]";
            //    if (Utilizado == "KARDEX")
            //    {
            //        BtnImprimir.Visible = false;
            //        btnSalir.Visible = false;
            //        btnAgregar.Visible = false;
            //        btnEliminar.Visible = false;


            //    }
            //}
            //else
            //{
            //    btnGuardar.Visible = true;
            //    this.Text = "Nota de Salida";
            //}


        }

        private void txtRucCliente_TextChanged(object sender, EventArgs e)
        {
            if (txtRucCliente.Text == "")
            {


                txtRazonSocialCliente.Clear();
                txtDireccionCliente.Clear();
                _ordenTrabajoDto.v_IdCliente = null;
            }
        }

        private void txtRucCliente_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (e.KeyCode == Keys.Enter)
            {
                if (txtRucCliente.Text.Trim() != "" & txtRucCliente.TextLength <= 5)
                {
                    Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text.Trim());
                    frm.ShowDialog();

                    if (frm._IdCliente != null)
                    {
                        _ordenTrabajoDto.v_IdCliente = frm._IdCliente;
                        txtRazonSocialCliente.Text = frm._RazonSocial;
                        txtDireccionCliente.Text = frm._Direccion;
                        txtRucCliente.Text = frm._NroDocumento;

                    }

                    else
                    {

                        txtRazonSocialCliente.Clear();
                        txtDireccionCliente.Clear();

                    }
                }
                else if (txtRucCliente.TextLength == 8 || txtRucCliente.TextLength == 11)
                {

                    string[] DatosCliente = new string[3];
                    DatosCliente = _objClienteBL.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());


                    if (DatosCliente != null)
                    {

                        _ordenTrabajoDto.v_IdCliente = DatosCliente[0];

                        txtRazonSocialCliente.Text = DatosCliente[2];
                        txtDireccionCliente.Text = DatosCliente[3];

                    }
                    else
                    {
                        // Cliente rápido

                        Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucCliente.Text.Trim(), "C");
                        frm.ShowDialog();
                        if (frm._Guardado == true)
                        {
                            txtRucCliente.Text = frm._NroDocumentoReturn;
                            DatosCliente = _objClienteBL.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                            if (DatosCliente != null)
                            {
                                _ordenTrabajoDto.v_IdCliente = DatosCliente[0];
                                txtRazonSocialCliente.Text = DatosCliente[2];
                                txtDireccionCliente.Text = DatosCliente[3];

                            }
                        }

                    }

                }

            }
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                _ordenTrabajoDto.v_IdCliente = frm._IdCliente;
                txtRucCliente.Text = frm._NroDocumento;
                txtRazonSocialCliente.Text = frm._RazonSocial;
                txtDireccionCliente.Text = frm._Direccion;


            }

        }

        private void grData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");


        }

        private void txtRucCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnActualizarCliente")
            {
                if (string.IsNullOrEmpty(_ordenTrabajoDto.v_IdCliente))
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
                        ClienteBL.ActualizarDatosCliente(_ordenTrabajoDto.v_IdCliente, _Contribuyente[5], _Contribuyente[0]);
                        KeyEventArgs e1 = new KeyEventArgs(Keys.Enter);
                        txtRucCliente_KeyDown(sender, e1);
                    }
                }
            }
        }

        private void txtResponsableInterno_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarResponsable")
            {
                var f = new frmBuscarAnexo();
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    //txtResponsableInterno.Text = f.Anexo.NombresApellidos;
                    _ordenTrabajoDto.v_IdResponsable = f.Anexo.IdAnexo;
                    
                }
            }
        }



    }
}
