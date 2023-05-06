using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Windows.WinClient.UI.Procesos;
using SAMBHS.Venta.BL;
using Infragistics.Win;
using SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarProducto : Form
    {
        readonly ProductoBL __objProductoBL = new ProductoBL();
        string _strFilterExpression, strNombre, _CodProducto;
        public string _IdCliente;
        public int _IdAlmacen, _EsAfectoDetraccion, _EsServicio, _NombreEditable, _ValidarStock, _EsAfectoPercepcion, _PrecioEditable, _IdMoneda;
        public string _IdProducto, _NombreProducto, _CodigoInternoProducto, _tipBusqueda, _UnidadMedida, _UnidadMedidaEmpaque,_IdProductoDetalle, _IdProductoAlmacen, _NroCuentaVenta, _NroCuentaCompra, _NroPedidoExportacion, _Observacion,_DescrProducto;
        public decimal _Empaque;
        public int _tipoProducto;
        public decimal _SeparaccionTotal, _stockActual, _PrecioUnitario, _PrecioVenta, _Descuento, _TasaPercepcion;
        private readonly Timer _myTimer = new Timer();
        public TipoBusquedaProductos _tipoBusqueda;
        public bool UsarMetodoPrueba, _EsProductoFinal, _EsAfectoisc, EditarPrecioVenta;
        private readonly MovimientoBL _objMovimientoBl = new MovimientoBL();
        public int IdAlmacen;
        public string IdProductoDetalleKardex = null, NombreProductoKardex, CodProductoKardex, IdProductoKardex;
        readonly ClienteBL _objClienteBl = new ClienteBL();
        private readonly bool _viewHistorial;
        private Task _tareaBuscar;
        public List<UltraGridRow> Filas = new List<UltraGridRow>();

        public frmBuscarProducto(int pstrIdAlmacen, string tipoBusqueda, string pstrIdCliente, string CodProducto, TipoBusquedaProductos _tipo = TipoBusquedaProductos.Ambos, bool ShowHistorial = false ,int TipoProducto =-1)
        {
            try
            {
                OperationResult objOperationResult = new OperationResult();
                _IdAlmacen = pstrIdAlmacen;
                _tipBusqueda = tipoBusqueda;
                _IdCliente = pstrIdCliente;
                _CodProducto = CodProducto;
                _tipoProducto = TipoProducto;

                var Producto = __objProductoBL.ObtenerProductoPorCodigoDetallado(ref objOperationResult, _CodProducto);
                IdProductoDetalleKardex = Producto != null ? Producto.v_IdProductoDetalle : null;
                IdProductoKardex = Producto != null ? Producto.v_IdProducto : null;
                InitializeComponent();
                if (tipoBusqueda == "REPORTE")
                {
                    Size = new Size(750, 550);

                }
                _viewHistorial = ShowHistorial;
                txtBuscarNombre.Text = CodProducto;
                if (_tipo == TipoBusquedaProductos.Ambos || _tipo == TipoBusquedaProductos.Mercaderia)
                {
                    rbMercaderias.Checked = true;
                }
                else rbServicios.Checked = true;
                _tipoBusqueda = _tipo;
                chkConStock.Checked = Globals.ClientSession.i_VisualizarBusquedaProductos != 1;   
            }
            catch (Exception ex)
            {
                
                throw;
            }
           
        }

        private void frmBuscarProducto_Load(object sender, EventArgs e)
        {

            var RucEmpresa = new NodeBL().ReporteEmpresa().FirstOrDefault().RucEmpresaPropietaria.Trim();

            if (RucEmpresa == Constants.RucWortec && (_tipBusqueda == "PedidoVenta" || _tipBusqueda == "Salida"))
            {
                grdData.DisplayLayout.Bands[0].Columns["d_Precio"].Header.Caption = "P.U.UU$.";

            }
            var objOperationResult = new OperationResult();
            _myTimer.Tick += OnTimedEvent;
            _myTimer.Interval = 500;

            using (GlobalFormColors colors = new GlobalFormColors())
            {
                BackColor = colors.FormColor;
                panel1.BackColor = colors.BannerColor;
                panel2.BackColor = colors.BannerColor;
                Tabs.BackColor = colors.FormColor;
            }
            btnBuscar_Click();
            //chkConStock.Enabled = _tipBusqueda != null;
            if (_tipBusqueda == "REPORTE")
            {
                grdData.DisplayLayout.Bands[0].Columns["d_separacion"].Hidden = true;
                grdData.DisplayLayout.Bands[0].Columns["d_Precio"].Hidden = true;
                grdData.DisplayLayout.Bands[0].Columns["stockActual"].Hidden = true;
                grdData.DisplayLayout.Bands[0].Columns["Saldo"].Hidden = true;
                chkConStock.Visible = false;
                rbMercaderias.Visible = false;
                rbServicios.Visible = false;
                Tabs.Controls.RemoveAt(3);
                Tabs.Controls.RemoveAt(2);
            }
            txtBuscarNombre.Focus();
            if (!string.IsNullOrEmpty(txtBuscarNombre.Text.Trim())) LabelContador(Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim()));

            grdData.DisplayLayout.Bands[0].Columns["_Seleccionado"].Hidden = !UsarMetodoPrueba;
            grdData.DisplayLayout.Bands[0].Columns["_Cantidad"].Hidden = !UsarMetodoPrueba;

            grdData.DisplayLayout.Bands[0].Columns["v_NroPedidoExportacion"].Hidden =
              Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta != 1;
            EditarPrecioVenta = Globals.ClientSession.i_UsaListaPrecios == 0 && Globals.ClientSession.i_EditarPrecioVenta == 1 && Globals.ClientSession.i_PrecioEditableTodosProductosPedido == 1;

            grdData.DisplayLayout.Bands[0].Columns["d_Precio"].CellActivation = EditarPrecioVenta ? Activation.AllowEdit : Activation.ActivateOnly;
            grdData.DisplayLayout.Bands[0].Columns["d_Precio"].CellClickAction = EditarPrecioVenta ? CellClickAction.Edit : CellClickAction.RowSelect;



            if (_tipBusqueda != "REPORTE")
            {
                var Cliente = _IdCliente != null ? _objClienteBl.ObtenerClientePorID(ref objOperationResult, _IdCliente) : null;
                //if (Cliente == null || Cliente.v_FlagPantalla == "V")
                //{
                //    Tabs.Controls.RemoveAt(3);
                //}
            }
            if (this._viewHistorial && Tabs.Tabs.Exists("DescuentosCliente"))
                Tabs.SelectedTab = Tabs.Tabs["DescuentosCliente"];
        }

        private void OnTimedEvent(Object myObject, EventArgs myEventArgs)
        {
            _myTimer.Stop();
            if (_tareaBuscar == null || _tareaBuscar.IsCompleted)
            {

                if (chkDescripcion1.Checked)
                {
                    _tareaBuscar = new Task(() =>
                    {
                        if (!this.IsDisposed)
                            Invoke((MethodInvoker)delegate
                            {
                                LabelContador(Utils.Windows.FiltrarGrillaPorColumnas(grdData, txtBuscarNombre.Text.Trim(), new List<string> { "v_Descripcion", "v_CodInterno" }));
                            });
                    }, TaskCreationOptions.PreferFairness);
                }
                else
                {
                    _tareaBuscar = new Task(() =>
                    {
                        if (!this.IsDisposed)
                            Invoke((MethodInvoker)delegate
                            {
                                LabelContador(Utils.Windows.FiltrarGrillaPorColumnas(grdData, txtBuscarNombre.Text.Trim(), new List<string> { "v_Descripcion2", "v_CodInterno" }));
                            });
                    }, TaskCreationOptions.PreferFairness);
                }

                _tareaBuscar.Start();
            }
        }

        private void btnBuscar_Click()
        {
            var filters = new Queue<string>();
            if (_tipBusqueda != "REPORTE")
                if (rbMercaderias.Checked && chkConStock.Checked && _tipBusqueda != null) filters.Enqueue("stockActual > 0");
            _strFilterExpression = (filters.Count > 0) ? string.Join(" && ", filters) : null;
            txtBuscarNombre.Enabled = false;
            chkConStock.Enabled = false;
            BindGrid();
            txtBuscarNombre.Enabled = true;
            chkConStock.Enabled = true;
            Activate();
            txtBuscarNombre.Focus();
            if (!string.IsNullOrEmpty(txtBuscarNombre.Text.Trim())) LabelContador(Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim()));
        }

        private void BindGrid()
        {
            try
            {
                var objData = GetData("v_Descripcion ASC", _strFilterExpression);
                grdData.DataSource = objData;
                if (!grdData.Rows.Any()) return;

                foreach (var f in grdData.Rows)
                {
                    f.Cells["_Seleccionado"].Value = false;
                    f.Cells["_Cantidad"].Activation = Activation.Disabled;
                    if (f.Cells["Observacion"].Value == null ||
                        string.IsNullOrEmpty(f.Cells["Observacion"].Value.ToString())) continue;
                    f.Cells["v_Descripcion"].ToolTipText = f.Cells["Observacion"].Value.ToString();
                    f.Cells["v_Descripcion"].Value = "*" + f.Cells["v_Descripcion"].Value;
                    f.RowSelectorAppearance.BackColor = Color.SteelBlue;
                }
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                ConfigurarGrillaBuscarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ConfiguraGrillaDescripcion()
        {
            UltraGridColumn Descripcion2 = grdData.DisplayLayout.Bands[0].Columns["v_Descripcion2"];
            UltraGridColumn Descripcion1 = grdData.DisplayLayout.Bands[0].Columns["v_Descripcion"];
            Descripcion2.Hidden = chkDescripcion1.Checked;
            Descripcion1.Hidden = !chkDescripcion1.Checked;
        }
        private void ConfigurarGrillaBuscarProductos()
        {

            UltraGridColumn StockActual = grdData.DisplayLayout.Bands[0].Columns["stockActual"];
            UltraGridColumn Separacion = grdData.DisplayLayout.Bands[0].Columns["d_separacion"]; //PrecioVenta
            UltraGridColumn Saldo = grdData.DisplayLayout.Bands[0].Columns["Saldo"];
            UltraGridColumn StockActualUM = grdData.DisplayLayout.Bands[0].Columns["StockActualUM"];
            UltraGridColumn SeparacionUM = grdData.DisplayLayout.Bands[0].Columns["SeparacionActualUM"]; //PrecioVenta
            UltraGridColumn SaldoUM = grdData.DisplayLayout.Bands[0].Columns["saldoUM"];

            UltraGridColumn UMProducto = grdData.DisplayLayout.Bands[0].Columns["EmpaqueUnidadMedida"]; //PrecioVenta
            UltraGridColumn UMUnidades = grdData.DisplayLayout.Bands[0].Columns["UM"];

            StockActualUM.Hidden = !chkUnidadMedida.Checked;

            UMProducto.Hidden = !chkUnidadMedida.Checked;
            UMUnidades.Hidden = chkUnidadMedida.Checked;
            StockActual.Hidden = chkUnidadMedida.Checked;
            ConfiguraGrillaDescripcion();
            if (_tipBusqueda != null)
            {
                SeparacionUM.Hidden = !chkUnidadMedida.Checked;
                SaldoUM.Hidden = !chkUnidadMedida.Checked;
                Separacion.Hidden = chkUnidadMedida.Checked;
                Saldo.Hidden = chkUnidadMedida.Checked;

            }
            else if (rbServicios.Checked)
            {
                UMProducto.Hidden = !chkUnidadMedida.Checked;
                UMUnidades.Hidden = chkUnidadMedida.Checked;

            }
        }



        private List<productoshortDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            List<productoshortDto> objData;

            var objOperationResult = new OperationResult();

            if (rbMercaderias.Checked)
            {
                switch (_tipBusqueda)
                {
                    case "Salida":
                        objData = _objMovimientoBl.ListarBusquedaProductoAlmacenNS(ref objOperationResult, pstrSortExpression, pstrFilterExpression, _IdAlmacen);
                        if (objOperationResult.Success == 0)
                        {
                            MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, "Error en la consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Clipboard.SetText(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage);
                            return null;
                        }
                        return objData;

                    case "PedidoVenta":
                        objData = Globals.ClientSession.i_UsaListaPrecios == 1 ? _objMovimientoBl.ListarBusquedaProductoAlmacenPV(ref objOperationResult, _IdAlmacen, _IdCliente, chkConStock.Checked)
                            : _objMovimientoBl.ListarBusquedaProductoAlmacenPV(ref objOperationResult, _IdAlmacen, chkConStock.Checked);

                        if (objOperationResult.Success == 0)
                        {
                            MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Clipboard.SetText(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage);
                            return null;
                        }
                        if (!objData.Any())
                        {
                            objData = Globals.ClientSession.i_UsaListaPrecios == 1 ? _objMovimientoBl.ListarBusquedaProductoAlmacenNSPrecios(ref objOperationResult, pstrSortExpression, pstrFilterExpression, _IdAlmacen, _IdCliente) :
                                 _objMovimientoBl.ListarBusquedaProductoAlmacenNS(ref objOperationResult, pstrSortExpression, pstrFilterExpression, _IdAlmacen);

                        }
                        return objData;

                    case "REPORTE":
                        objData = _objMovimientoBl.ListarBusquedaProductos(ref objOperationResult, "v_CodInterno ASC",_tipoProducto);
                        if (objOperationResult.Success == 0)
                        {
                            MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Clipboard.SetText(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage);
                            return null;
                        }
                        return objData;

                    default:
                        var layaoutGrid = grdData.DisplayLayout.Bands[0];
                        try
                        {
                            layaoutGrid.Columns["d_separacion"].Hidden = true;
                            layaoutGrid.Columns["Saldo"].Hidden = true;
                            layaoutGrid.Columns["SeparacionActualUM"].Hidden = true;
                            layaoutGrid.Columns["saldoUM"].Hidden = true;
                            layaoutGrid.Columns["d_Precio"].Header.Caption = @"Precio Costo";
                        }
                        catch (Exception ex)
                        {
                        }

                        objData = _objMovimientoBl.ListarBusquedaProductoAlmacen(ref objOperationResult, pstrSortExpression, pstrFilterExpression, _IdAlmacen);
                        if (objOperationResult.Success == 0)
                        {
                            MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Clipboard.SetText(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage);
                            return null;
                        }
                        return objData;
                }
            }
            objData = _objMovimientoBl.ListarBusquedaServicios(ref objOperationResult, pstrSortExpression, pstrFilterExpression, _IdCliente, _IdAlmacen);

            return objData;
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {

                if (e.KeyCode == Keys.Enter && (grdData.Selected.Rows.Count > 0 || 
                    grdData.Rows.Any(f => Convert.ToBoolean(f.Cells["_Seleccionado"].Value.ToString()))))
                {

                    if (!UsarMetodoPrueba)
                        Filas.AddRange(grdData.Selected.Rows.Cast<UltraGridRow>());
                    
                    else
                        Filas = grdData.Rows.Where(f => Convert.ToBoolean(f.Cells["_Seleccionado"].Value.ToString())).ToList();
                    
                    if (Application.OpenForms["frmRegistroVenta"] != null)
                    {
                        ((frmRegistroVenta)Application.OpenForms["frmRegistroVenta"]).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmRegistroVentaNBS"] != null)
                    {
                        ((frmRegistroVentaNBS)Application.OpenForms["frmRegistroVentaNBS"]).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmPedido"] != null)
                    {
                        ((frmPedido)Application.OpenForms["frmPedido"]).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmGuiaRemision"] != null)
                    {
                        ((frmGuiaRemision)Application.OpenForms["frmGuiaRemision"]).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmNotaIngreso"] != null)
                    {
                        ((frmNotaIngreso)Application.OpenForms["frmNotaIngreso"]).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmNotaSalida"] != null)
                    {
                        ((frmNotaSalida)Application.OpenForms["frmNotaSalida"]).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmRegistroCompra"] != null)
                    {
                        ((frmRegistroCompra)Application.OpenForms["frmRegistroCompra"]).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmGuiaRemisionCompra"] != null)
                    {
                        (Application.OpenForms["frmGuiaRemisionCompra"] as frmGuiaRemisionCompra).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmRegistroVentaRapida"] != null)
                    {
                        (Application.OpenForms["frmRegistroVentaRapida"] as frmRegistroVentaRapida).RecibirItems(Filas);
                    }
                    
                    if (Application.OpenForms["frmRegistroVentaRapidaNBS"] != null)
                    {
                        (Application.OpenForms["frmRegistroVentaRapidaNBS"] as frmRegistroVentaRapidaNBS).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmTransferencia"] != null)
                    {
                        (Application.OpenForms["frmTransferencia"] as frmTransferencia).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmOrdenTrabajo"] != null)
                    {
                        (Application.OpenForms["frmOrdenTrabajo"] as frmOrdenTrabajo).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmOrdenCompra"] != null)
                    {
                        (Application.OpenForms["frmOrdenCompra"] as frmOrdenCompra).RecibirItems(Filas);
                    }

                    if (Application.OpenForms["frmRegistroImportacion"] != null)
                    {
                        (Application.OpenForms["frmRegistroImportacion"] as frmRegistroImportacion).RecibirItems(Filas);
                    }
                    
                    
                    Close();
                }
                else if (e.KeyCode == Keys.Space)
                {
                    grdData.Selected.Rows.Add(grdData.ActiveRow);
                    grdData.ActiveRow.Selected = true;
                }

                switch (e.KeyCode)
                {
                    case Keys.Up:

                        if (grdData.ActiveRow.Cells["_Cantidad"].IsActiveCell || grdData.ActiveRow.Cells["d_Precio"].IsActiveCell)
                        {
                            grdData.PerformAction(UltraGridAction.AboveRow);
                            e.SuppressKeyPress = true;
                        }

                        break;
                    case Keys.Down:
                        if (grdData.ActiveRow.Cells["_Cantidad"].IsActiveCell || grdData.ActiveRow.Cells["d_Precio"].IsActiveCell)
                        {
                            grdData.PerformAction(UltraGridAction.BelowRow);
                            e.SuppressKeyPress = true;
                        }
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
                }
            }
        }

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void frmBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void chkConStock_CheckedChanged(object sender, EventArgs e)
        {
            //txtBuscarNombre.Clear();
            btnBuscar_Click();
        }

        private void grdData_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            lblItemsSeleccionados.Text = string.Format("Items Seleccionados: {0}", grdData.Selected.Rows.Count.ToString());
        }

        private void rbMercaderias_CheckedChanged(object sender, EventArgs e)
        {
            chkConStock.Enabled = rbMercaderias.Checked;
            btnBuscar_Click();
            //txtBuscarNombre.Clear();
        }

        private void txtBuscarNombre_KeyUp(object sender, KeyEventArgs e)
        {
            _myTimer.Stop();
            _myTimer.Start();
        }

        void LabelContador(int Cantidad)
        {
            lblContadorFilas.Text = String.Format("Se encontraron {0} registros", Cantidad);
        }

        private void btnBuscar_Click_1(object sender, EventArgs e)
        {

        }

        private void rbServicios_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void frmBuscarProducto_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            try
            {
                bool _checked = false;
                if (e.Cell.Column.Key == "_Seleccionado")
                {
                    try
                    {

                        _checked = Convert.ToBoolean(e.Cell.EditorResolved.Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        _checked = true;
                    }
                    if (_checked)
                    {
                        e.Cell.Row.Cells["_Cantidad"].Activation = Activation.AllowEdit;
                        e.Cell.Row.Cells["_Cantidad"].Activate();
                        grdData.PerformAction(UltraGridAction.EnterEditMode);
                    }
                    else
                    {
                        e.Cell.Row.Cells["_Cantidad"].Value = null;
                        e.Cell.Row.Cells["_Cantidad"].Activation = Activation.Disabled;
                        if (EditarPrecioVenta)
                        {
                            e.Cell.Row.Cells["d_Precio"].Value = null;
                            e.Cell.Row.Cells["d_Precio"].Activation = Activation.Disabled;
                        }
                    }
                }

                if (e.Cell.Column.Key == "_Cantidad")
                    e.Cell.Value = e.Cell.EditorResolved.Value;
                if (e.Cell.Column.Key == "d_Precio")
                {
                    EmbeddableEditorBase editor = e.Cell.EditorResolved;
                    if (editor.Value != DBNull.Value)
                        e.Cell.Value = editor.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void chkIncluirCant_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkIncluirCant.Checked)
                {
                    grdData.DisplayLayout.Bands[0].Columns["d_Precio"].Header.VisiblePosition = 17;
                }
                else
                {
                    grdData.DisplayLayout.Bands[0].Columns["d_Precio"].Header.VisiblePosition = EditarPrecioVenta ? 2 : 17;
                    lblF1.Visible = true;
                }
                UsarMetodoPrueba = chkIncluirCant.Checked;
                grdData.DisplayLayout.Bands[0].Columns["_Seleccionado"].Hidden = !UsarMetodoPrueba;
                grdData.DisplayLayout.Bands[0].Columns["_Cantidad"].Hidden = !UsarMetodoPrueba;
                ultraCheckEditor1.Visible = UsarMetodoPrueba;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraCheckEditor1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ultraCheckEditor1.Checked)
                {
                    txtBuscarNombre.Clear();
                    LabelContador(Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim()));
                    txtBuscarNombre.Enabled = false;
                    grdData.Rows.ToList().ForEach(f =>
                    {
                        f.Hidden = !Convert.ToBoolean(f.Cells["_Seleccionado"].Value.ToString());
                    });
                }
                else
                {
                    txtBuscarNombre.Enabled = true;
                    grdData.Rows.ToList().ForEach(f => f.Hidden = false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.B))
            {
                txtBuscarNombre.Focus();
                return true;
            }

            if (!grdData.Focused && keyData == Keys.Enter)
            {
                grdData.Focus();
                return true;
            }

            if (!grdData.Focused && keyData == Keys.Down)
            {
                grdData.Focus();
                if (grdData.Rows == null) return false;
                var primeraFila = grdData.Rows.FirstOrDefault();
                if (primeraFila != null) primeraFila.Activate();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!txtBuscarNombre.Focused && char.IsLetter(e.KeyChar))
            {
                txtBuscarNombre.Focus();
                txtBuscarNombre.Text = txtBuscarNombre.Text + @" " + e.KeyChar;
                txtBuscarNombre.SelectionStart = txtBuscarNombre.Text.Length;
                txtBuscarNombre.SelectionLength = 0;
            }
        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                var filaActiva = grdData.ActiveRow;
                if (filaActiva != null)
                {
                    if (e.KeyChar == Convert.ToChar(Keys.Space) && grdData.ActiveRow != null)
                    {

                        chkIncluirCant.Checked = true;
                        filaActiva.Cells["_Seleccionado"].Value = true;
                        filaActiva.Cells["_Cantidad"].Activation = Activation.AllowEdit;
                        filaActiva.Cells["_Cantidad"].Activate();
                        grdData.PerformAction(UltraGridAction.EnterEditMode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void Tabs_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        {
            IdProductoDetalleKardex = IdProductoDetalleKardex ?? (grdData.Rows.Any() ? grdData.Rows.FirstOrDefault().Cells["v_IdProductoDetalle"].Value.ToString() : null);
            IdAlmacen = IdAlmacen == 0 ? grdData.Rows.Any() ? int.Parse(grdData.Rows.FirstOrDefault().Cells["IdAlmacen"].Value.ToString()) : 0 : IdAlmacen;
            IdProductoKardex = IdProductoKardex ?? (grdData.Rows.Any() ? grdData.Rows.FirstOrDefault().Cells["v_IdProducto"].Value.ToString() : null);

            dtpFechaDesdeKardex.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpFechaHastaKardex.Value = DateTime.Now;

            dtpFechaDesdeDescuentos.Value = DateTime.Parse("01/" + DateTime.Now.Month + "/" + DateTime.Now.Year);
            dtpFechaHastaDescuentos.Value = DateTime.Now;

            switch (e.Tab.Key)
            {
                case "Kardex":
                    BindGridKardex(IdAlmacen, IdProductoDetalleKardex);
                    CalcularSaldoKardex();

                    break;
                case "DescuentosCliente":
                    chkDescuentoClienteProductoEspecifico.Checked = true;
                    ConfiguracionGrillaDescuentosCliente();
                    BindGridDescuentosCliente(_IdCliente, IdProductoDetalleKardex);
                    break;
                case "ResultadoBusqueda":
                    chkTodosDescuentosClienteSeleccionado.Checked = false;
                    break;
            }

        }
        private void ConfiguracionGrillaDescuentosCliente()
        {
            grdDescuentos.DisplayLayout.Bands[0].Columns["Producto"].Hidden = !chkTodosDescuentosClienteSeleccionado.Checked;
            grdDescuentos.DisplayLayout.Bands[0].Columns["Cliente"].Hidden = !chkTodosDescuentosTodosClientes.Checked;
        }
        private void CalcularSaldoKardex()
        {
            decimal saldo = 0;
            foreach (var Fila in grdKardex.Rows)
            {

                //if (Fila.Cells["EsDevolucion"].Value == null || int.Parse(Fila.Cells["EsDevolucion"].Value.ToString()) == 0)
                //{
                //    saldo = Fila.Cells["TipoMovimiento"].Value.ToString() == "1" ? saldo + decimal.Parse(Fila.Cells["IngresosCantidadEmpaque"].Value.ToString()) : saldo - decimal.Parse(Fila.Cells["SalidasCantidadEmpaque"].Value.ToString());
                //    Fila.Cells["Saldo"].Value = saldo;
                //}
                //else
                //{
                //    saldo = Fila.Cells["TipoMovimiento"].Value.ToString() == "1" ? saldo - decimal.Parse(Fila.Cells["IngresosCantidadEmpaque"].Value.ToString()) : saldo + decimal.Parse(Fila.Cells["SalidasCantidadEmpaque"].Value.ToString());
                //    Fila.Cells["Saldo"].Value = saldo;
                //}

                var fechaemision = DateTime.Parse(Fila.Cells["Fecha"].Value.ToString()).Year;
                if (dtpFechaDesdeKardex.Value.Year != dtpFechaHastaKardex.Value.Year && dtpFechaHastaKardex.Value.Year == fechaemision && Fila.Cells["TipoMotivo"].Value.ToString() == "5")
                {
                }
                else
                {
                    saldo = Fila.Cells["TipoMovimiento"].Value.ToString() == "1" ? saldo + decimal.Parse(Fila.Cells["IngresosCantidadEmpaque"].Value.ToString()) : saldo - decimal.Parse(Fila.Cells["SalidasCantidadEmpaque"].Text);
                }
                Fila.Cells["Saldo"].Value = saldo;

            }
        }
        private void grdData_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
            {
                NombreProductoKardex = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString().Trim();
                CodProductoKardex = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value.ToString().Trim();
                IdProductoDetalleKardex = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
                IdProductoKardex = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProducto"].Value.ToString();
                IdAlmacen = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["IdAlmacen"].Value.ToString());

            }
        }

        private void BindGridKardex(int pIntAlmacen, string strProductoDetalle)
        {
            OperationResult objOperationResult = new OperationResult();
            var objData = GetDataKardex(ref objOperationResult, pIntAlmacen, strProductoDetalle);

            if (objOperationResult.Success == 1)
            {
                grdKardex.DataSource = objData;
                grdKardex.Focus();
                lblContadorFilasKardex.Text = string.Format("Se encontraron {0} Movimientos.", objData.Count);
            }
            else
            {

                lblContadorFilasKardex.Text = @"Se encontraron 0 Movimientos.";
            }
            var Producto = IdProductoDetalleKardex != null ? __objProductoBL.ObtenerProducto(ref objOperationResult, IdProductoKardex) : null;
            txtDescripcionProductoKardex.Text = IdProductoKardex != null ? Producto.v_Descripcion.Trim() : NombreProductoKardex;
            txtCodigoProductoKardex.Text = IdProductoKardex != null ? Producto != null ? Producto.v_CodInterno.Trim() : "" : CodProductoKardex == null ? "" : CodProductoKardex.Trim();
        }


        private void BindGridDescuentosCliente(string pIdCliente, string strProductoDetalle)
        {
            OperationResult objOperationResult = new OperationResult();
            var objData = GetDataDescuentosClientes(ref objOperationResult, pIdCliente, strProductoDetalle);

            if (objOperationResult.Success == 1)
            {

                grdDescuentos.DataSource = objData;
                grdDescuentos.Focus();
                lblContadorFilasDescuentoCliente.Text = string.Format("Se encontraron {0} Movimientos.", objData.Count);
            }
            else
            {

                lblContadorFilasDescuentoCliente.Text = string.Format("Se encontraron {0} Movimientos.");
            }
            var Cliente = _IdCliente != null ? _objClienteBl.ObtenerClientePorID(ref objOperationResult, _IdCliente) : null;
            var Producto = IdProductoDetalleKardex != null ? __objProductoBL.ObtenerProducto(ref objOperationResult, IdProductoKardex) : null;

            txtCliente.Text = pIdCliente == null && chkTodosDescuentosTodosClientes.Checked ? "TODOS LOS CLIENTES" : _IdCliente == null && !chkTodosDescuentosTodosClientes.Checked ? "CLIENTE NO EXISTE" : (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno + " " + Cliente.v_PrimerNombre + " " + Cliente.v_SegundoNombre + " " + Cliente.v_RazonSocial).Trim();
            txtProductoDescuento.Text = chkTodosDescuentosClienteSeleccionado.Checked ? "TODOS LOS PRODUCTOS" : IdProductoDetalleKardex != null ? Producto.v_CodInterno.Trim() + " " + Producto.v_Descripcion.Trim() : "";
        }

        private List<movimientodetalleDto> GetDataKardex(ref OperationResult objOperationResult, int pIntAlmacen, string IdProductoDetalle)
        {

            var _objData = _objMovimientoBl.ObtenerDetalleKardexManguifajasFecha(ref objOperationResult, pIntAlmacen, IdProductoDetalle, 2, dtpFechaDesdeKardex.Value, DateTime.Parse(dtpFechaHastaKardex.Text + " 23:59"));
            return _objData;
        }
        private List<ClientesDescuentos> GetDataDescuentosClientes(ref OperationResult objOperationResult, string IdCliente, string IdProductoDetalle)
        {

            List<ClientesDescuentos> _objData = new List<ClientesDescuentos>();
            if (chkTodosDescuentosClienteSeleccionado.Checked)
            {
                _objData = _objClienteBl.ObtenerDescuentosCliente(ref objOperationResult, _IdCliente, null, dtpFechaDesdeDescuentos.Value.Date, DateTime.Parse(dtpFechaHastaDescuentos.Text + " 23:59"));

            }
            else if (chkTodosDescuentosTodosClientes.Checked)
            {
                _objData = _objClienteBl.ObtenerDescuentosCliente(ref objOperationResult, null, IdProductoDetalleKardex, dtpFechaDesdeDescuentos.Value.Date, DateTime.Parse(dtpFechaHastaDescuentos.Text + " 23:59"));
            }
            else
            {

                _objData = _objClienteBl.ObtenerDescuentosCliente(ref objOperationResult, _IdCliente, IdProductoDetalleKardex, dtpFechaDesdeDescuentos.Value.Date, DateTime.Parse(dtpFechaHastaDescuentos.Text + " 23:59"));
            }
            return _objData;
        }

        private void chkTodosDescuentos_CheckedChanged(object sender, EventArgs e)
        {

            ConfiguracionGrillaDescuentosCliente();
            if (chkTodosDescuentosClienteSeleccionado.Checked == true)
            {
                BindGridDescuentosCliente(_IdCliente, null);
            }
            else
            {
                BindGridDescuentosCliente(_IdCliente, IdProductoDetalleKardex);
            }
        }

        private void chkTodosDescuentosTodosClientes_CheckedChanged(object sender, EventArgs e)
        {
            ConfiguracionGrillaDescuentosCliente();
            if (chkTodosDescuentosTodosClientes.Checked == true)
            {
                BindGridDescuentosCliente(null, IdProductoDetalleKardex);
            }

        }

        private void btnBuscarKardexFecha_Click(object sender, EventArgs e)
        {
            BindGridKardex(IdAlmacen, IdProductoDetalleKardex);
            CalcularSaldoKardex();
        }

        private void dtpFechaHastaDescuentos_ValueChanged(object sender, EventArgs e)
        {
            //if (chkTodosDescuentosTodosClientes.Checked)
            //{
            //    BindGridDescuentosCliente(null, IdProductoDetalleKardex);
            //}
            //else
            //{
            //    BindGridDescuentosCliente(null, IdProductoDetalleKardex);
            //}
        }

        private void dtpFechaDesdeDescuentos_ValueChanged(object sender, EventArgs e)
        {
            //if (chkTodosDescuentosTodosClientes.Checked)
            //{
            //    BindGridDescuentosCliente(null, IdProductoDetalleKardex);
            //}
            //else
            //{
            //    BindGridDescuentosCliente(null, IdProductoDetalleKardex);
            //}
        }

        private void grdKardex_ClickCellButton(object sender, CellEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string serie, correlativo;
            string[] SerieCorrelativo;

            if (grdKardex.ActiveRow == null) return;
            switch (e.Cell.Column.Key)
            {
                case "Ver":
                    string seriecorrelativo = grdKardex.ActiveRow.Cells["v_NumeroDocumento"].Value != null && grdKardex.ActiveRow.Cells["v_NumeroDocumento"].Value.ToString() != string.Empty ? grdKardex.ActiveRow.Cells["v_NumeroDocumento"].Value.ToString() : "";
                    if (seriecorrelativo != string.Empty)
                    {
                        SerieCorrelativo = seriecorrelativo.Split(new Char[] { '-' });

                        serie = SerieCorrelativo[0].Trim();
                        correlativo = SerieCorrelativo[1].Trim();

                        if (grdKardex.ActiveRow.Cells["Origen"].Value != null && grdKardex.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenCompra)
                        {
                            var IdCompra = _objMovimientoBl.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.Compra, int.Parse(grdKardex.ActiveRow.Cells["i_TipoDocumento"].Value.ToString()), serie, correlativo);
                            if (IdCompra != null)
                            {
                                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                                frmRegistroCompra frm = new frmRegistroCompra("Edicion", IdCompra, "KARDEX");
                                frm.ShowDialog();
                            }
                            else
                            {


                            }

                        }
                        else if (grdKardex.ActiveRow.Cells["Origen"].Value != null && grdKardex.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenVenta)
                        {
                            var IdVenta = _objMovimientoBl.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.Venta, int.Parse(grdKardex.ActiveRow.Cells["i_TipoDocumento"].Value.ToString()), serie, correlativo);

                            if (IdVenta != null)
                            {

                                frmRegistroVenta frm = new frmRegistroVenta("Edicion", IdVenta, "KARDEX");
                                frm.ShowDialog();
                            }
                            else
                            {


                            }
                        }
                        else if (grdKardex.ActiveRow.Cells["Origen"].Value != null && grdKardex.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenGuiaInterna)
                        {
                            var IdGuia = _objMovimientoBl.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.GuiaRemision, 438, serie, correlativo);
                            if (IdGuia != null)
                            {
                                frmGuiaRemision frm = new frmGuiaRemision("Edicion", IdGuia, "KARDEX");
                                frm.ShowDialog();
                            }

                        }
                        else if (grdKardex.ActiveRow.Cells["Origen"].Value != null && grdKardex.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenImportacion)
                        {
                            var IdVenta = _objMovimientoBl.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.Importacion, int.Parse(grdKardex.ActiveRow.Cells["i_TipoDocumento"].Value.ToString()), serie, correlativo);

                            if (IdVenta != null)
                            {

                                //  frmRegistroVenta frm = new frmRegistroVenta("Edicion", IdVenta, "KARDEX");
                                frmRegistroImportacion frm = new frmRegistroImportacion("Edicion", IdVenta, "KARDEX");
                                frm.ShowDialog();
                            }


                        }

                        else if (grdKardex.ActiveRow.Cells["Origen"].Value == string.Empty || grdKardex.ActiveRow.Cells["Origen"].Value.ToString() == string.Empty)
                        {


                            if (grdKardex.ActiveRow.Cells["v_IdMovimiento"] != null)
                            {

                                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                                if (int.Parse(grdKardex.ActiveRow.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeIngreso)
                                {

                                    frmNotaIngreso frm = new frmNotaIngreso("Edicion", grdKardex.ActiveRow.Cells["v_IdMovimiento"].Value.ToString(), "KARDEX");
                                    frm.ShowDialog();
                                }
                                else
                                {
                                    frmNotaSalida frm = new frmNotaSalida("Edicion", grdKardex.ActiveRow.Cells["v_IdMovimiento"].Value.ToString(), "KARDEX");
                                    frm.ShowDialog();
                                }

                            }


                        }
                    }
                    break;

            }
        }

        private void grdDescuentos_ClickCellButton(object sender, CellEventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            string serie, correlativo;
            string[] SerieCorrelativo;

            if (grdDescuentos.ActiveRow == null) return;
            switch (e.Cell.Column.Key)
            {
                case "Ver":
                    string seriecorrelativo = grdDescuentos.ActiveRow.Cells["NroDocumento"].Value != null && grdDescuentos.ActiveRow.Cells["NroDocumento"].Value.ToString() != string.Empty ? grdDescuentos.ActiveRow.Cells["NroDocumento"].Value.ToString() : "";
                    if (seriecorrelativo != string.Empty)
                    {
                        SerieCorrelativo = seriecorrelativo.Split(new Char[] { '-' });

                        serie = SerieCorrelativo[0].Trim();
                        correlativo = SerieCorrelativo[1].Trim();

                        if (grdDescuentos.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString() != ((int)TiposDocumentos.GuiaInterna).ToString())
                        {
                            var IdVenta = _objMovimientoBl.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.Venta, int.Parse(grdDescuentos.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString()), serie, correlativo);

                            if (IdVenta != null)
                            {

                                frmRegistroVenta frm = new frmRegistroVenta("Edicion", IdVenta, "KARDEX");
                                frm.ShowDialog();
                            }

                        }
                        else
                        {
                            var IdGuia = _objMovimientoBl.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.GuiaRemision, 438, serie, correlativo);
                            if (IdGuia != null)
                            {
                                frmGuiaRemision frm = new frmGuiaRemision("Edicion", IdGuia, "KARDEX");
                                frm.ShowDialog();
                            }

                        }

                    }
                    break;

            }
        }

        private void btnBuscarDescuentosClienteFecha_Click(object sender, EventArgs e)
        {
            if (chkTodosDescuentosTodosClientes.Checked)
            {
                BindGridDescuentosCliente(null, IdProductoDetalleKardex);
            }
            else
            {
                BindGridDescuentosCliente(null, IdProductoDetalleKardex);
            }
        }

        private void chkUnidadMedida_CheckedChanged(object sender, EventArgs e)
        {
            ConfigurarGrillaBuscarProductos();
        }

        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            _IdProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProducto"].Value.ToString();
            _IdProductoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
            _CodigoInternoProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value.ToString();
            //_NombreProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString();
            _DescrProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString();
            grdData_KeyDown(this, new KeyEventArgs(Keys.Enter));
            
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            if (Globals.ClientSession.i_ValidarStockMinimoProducto == 1 && e.Row.Cells["StockMinimo"].Value.ToString() == "1")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
            if (e.Row.Band.Index == 0 && e.Row.Cells["t_FechaCaducidad"].Value != null && DateTime.Parse(e.Row.Cells["t_FechaCaducidad"].Value.ToString()).Date.ToShortDateString() == Constants.FechaNula)
            {
               e.Row.Cells ["t_FechaCaducidad"].Appearance.BackColor = Color.White ;
               e.Row.Cells["t_FechaCaducidad"].Appearance.ForeColor = Color.White;
            }
        }

        private void chkDescuentoClienteProductoEspecifico_CheckedChanged(object sender, EventArgs e)
        {
            chkTodosDescuentos_CheckedChanged(sender, e);
        }

        private void chkDescripcion1_CheckedChanged(object sender, EventArgs e)
        {
            ConfiguraGrillaDescripcion();
        }
    }
}
