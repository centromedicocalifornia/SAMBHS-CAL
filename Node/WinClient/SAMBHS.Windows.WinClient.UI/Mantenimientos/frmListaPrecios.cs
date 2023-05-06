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
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmListaPrecios : Form
    {
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        listaprecioDto _objListaPrecio = new listaprecioDto();
        listapreciodetalleDto _objListaPrecioDetalle = new listapreciodetalleDto();
        ListaPreciosBL _objListaPrecioBL = new ListaPreciosBL();
        List<listaprecioDto> _ListadoListaPrecio = new List<listaprecioDto>();
        productoDto objProductoDto = new productoDto();
        string _strFilterExpression, TipoCambio, _Mode, strModo, strIdListaPrecio, IdListaPrecioE;
        public string _pstrIdMovimiento_Nuevo;
        productoDto _objProducto = new productoDto();
        int _MaxV, _ActV;
        public List<string> ProductosAgregar = new List<string>();
        SecurityBL _objSecurityBL = new SecurityBL();
        bool _btnGuardar, _btnMigrar,_btnAgregar;
        #region Temporales Detalles de Pedido
        List<listapreciodetalleDto> _TempDetalle_AgregarDto = new List<listapreciodetalleDto>();
        List<listapreciodetalleDto> _TempDetalle_ModificarDto = new List<listapreciodetalleDto>();
        List<listapreciodetalleDto> _TempDetalle_EliminarDto = new List<listapreciodetalleDto>();

        List<productoDto> _TempProducto_AgregarDto = new List<productoDto>();
        List<productoDto> _TempProducto_ModificarDto = new List<productoDto>();
        #endregion
        System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public frmListaPrecios(string Modo, string idListaPrecios)
        {
            InitializeComponent();
            strModo = Modo;
            strIdListaPrecio = idListaPrecios;
        }
        private void frmListaPrecios_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            myTimer.Tick += OnTimedEvent;
            myTimer.Interval = 500;
            #region ControlAcciones
            var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmListaPrecios", Globals.ClientSession.i_RoleId);
            _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmListaPrecios_Save", _formActions);
            _btnMigrar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmListaPrecios_Migrate", _formActions);
            _btnAgregar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmListaPrecios_Add", _formActions);
            btnGuardar.Enabled = _btnGuardar;
            btnMigracion.Enabled = _btnMigrar;
            #endregion
            #region CargarCombos
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboListaPrecios, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 47, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            #endregion
            ObtenerListadoListaPrecios();
            TipoCambio = _objListaPrecioBL.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Now.Date);
            ConfigurarGrilla();
            btnMigracion.Visible = Globals.ClientSession.i_SystemUserId == 1;
        }
        private void OnTimedEvent(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();
            LabelContador(Utils.Windows.FiltrarGrillaPorColumnas(grdData, txtBuscarProducto.Text, new List<string> { "v_CodInterno", "v_Descripcion" }));
            var x = grdData.Rows.Count();
            //btnExportar.Enabled = grdData.Rows.Count() > 0 ? true : false;

        }
        void LabelContador(int Cantidad)
        {
            lblContadorFilas.Text = String.Format("Se encontraron {0} registros", Cantidad);
        }
        private void ConfigurarGrilla()
        {
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales, (int)Globals.ClientSession.i_PrecioDecimales, 4);
            //grdData.DisplayLayout.Bands[0].Columns["Costo"].CellActivation =Globals.ClientSession.i_UsaListaPrecios ==1 ? Globals.ClientSession.i_CostoListaPreciosDiferentesxAlmacen ==1? Activation.AllowEdit : Activation.NoEdit :Activation.NoEdit;
        }

        #region CRUD
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvListaPrecios.Validate(true, false).IsValid)
            {

                if (_Mode == "New")
                {

                    _objListaPrecio.i_IdAlmacen = int.Parse(cboAlmacen.Value.ToString());
                    _objListaPrecio.i_IdLista = int.Parse(cboListaPrecios.Value.ToString());
                    LlenarTemporales();
                    //_objListaPrecioBL.InsertarListaPrecios(ref objOperationResult, _objListaPrecio, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);

                }
                else if (_Mode == "Edit")
                {
                    _objListaPrecio.i_IdAlmacen = int.Parse(cboAlmacen.Value .ToString());
                    _objListaPrecio.i_IdLista = int.Parse(cboListaPrecios.Value.ToString());
                    LlenarTemporales();
                    using (new LoadingClass.PleaseWait(this.Location, "Guardando...")) 
                    {
                    _objListaPrecioBL.ActualizarListaPrecio(ref objOperationResult, _objListaPrecio, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto,_TempProducto_AgregarDto,_TempProducto_ModificarDto,decimal.Parse (TipoCambio ));
                    }
                }
                if (objOperationResult.Success == 1)
                {
                    strModo = "Guardado";
                    strIdListaPrecio = _objListaPrecio.v_IdListaPrecios;
                    ObtenerListadoListaPrecios();
                    _pstrIdMovimiento_Nuevo = _objListaPrecio.v_IdListaPrecios;
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                _TempDetalle_AgregarDto = new List<listapreciodetalleDto>();
                _TempDetalle_ModificarDto = new List<listapreciodetalleDto>();
                _TempDetalle_EliminarDto = new List<listapreciodetalleDto>();

            }


        }

        private void ObtenerListadoListaPrecios()
        {
            OperationResult objOperationResult = new OperationResult();

            _ListadoListaPrecio = _objListaPrecioBL.ObtenerListadoListaPrecios(ref objOperationResult);

            switch (strModo)
            {
                case "Edicion":

                    CargarCabecera(strIdListaPrecio);
                    IdListaPrecioE = _objListaPrecioBL.ObtenerIdListaPreciostr(ref  objOperationResult, int.Parse(cboAlmacen.Value.ToString()), int.Parse(cboListaPrecios.Value.ToString()));
                    List<string> ProductosAgregar = _objListaPrecioBL.ObtenerMinusProductoAlmacen(int.Parse(cboAlmacen.Value.ToString()), IdListaPrecioE);
                    if (ProductosAgregar.Count == 0)
                    {
                        btnAgregar.Enabled = false;
                    }
                    else
                    {
                        btnAgregar.Enabled = _btnAgregar;
                    }
                    break;

                case "Nuevo":
                    if (_ListadoListaPrecio.Count != 0)
                    {
                        _MaxV = _ListadoListaPrecio.Count() - 1;
                        _ActV = _MaxV;
                        _Mode = "New";

                    }
                    else
                    {
                        _Mode = "New";
                        _MaxV = 1;
                        _ActV = 1;

                    }
                    break;

                case "Guardado":

                    if (strIdListaPrecio == "" | strIdListaPrecio == null)
                    {
                        CargarCabecera(_ListadoListaPrecio[_MaxV].v_IdListaPrecios);
                    }
                    else
                    {
                        CargarCabecera(strIdListaPrecio);
                    }
                    IdListaPrecioE = _objListaPrecioBL.ObtenerIdListaPreciostr(ref  objOperationResult, int.Parse(cboAlmacen.Value.ToString()), int.Parse(cboListaPrecios.Value.ToString()));
                    ProductosAgregar = _objListaPrecioBL.ObtenerMinusProductoAlmacen(int.Parse(cboAlmacen.Value.ToString()), IdListaPrecioE);

                    if (ProductosAgregar.Count == 0)
                    {
                        btnAgregar.Enabled = false;
                    }
                    else
                    {
                        
                        btnAgregar.Enabled = _btnAgregar;
                    }
                    break;

                case "Consulta":
                    if (_ListadoListaPrecio.Count != 0)
                    {
                        _MaxV = _ListadoListaPrecio.Count() - 1;
                        _ActV = _MaxV;
                        CargarCabecera(_ListadoListaPrecio[_MaxV].v_IdListaPrecios);
                        _Mode = "Edit";
                       
                    }
                    else
                    {
                       
                        _Mode = "New";
                        CargarDetalle("",-1);
                        _MaxV = 1;
                        _ActV = 1;
                       
                    }
                   
                    break;
            }


        }

        #endregion
        #region Grilla
        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            CalcularValoresFila(grdData.Rows[grdData.ActiveRow.Index]);
           
        }
        private void CalcularValoresFila(UltraGridRow Fila)
        {

            if (Fila.Cells["d_Descuento"].Value == null) { Fila.Cells["d_Descuento"].Value = "0"; }
            if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }
            if (Fila.Cells["d_Descuento"].Value.ToString () == "") { Fila.Cells["d_Descuento"].Value = "0"; }
            if (Fila.Cells["d_Precio"].Value.ToString () == "") { Fila.Cells["d_Precio"].Value = "0"; }
            if (decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString()) != 0 & decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0)
            {
               // Fila.Cells["d_PrecioMinSoles"].Value = decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) - (decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString())/10);
                Fila.Cells["d_PrecioMinSoles"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Precio"].Text.ToString()) - (decimal.Parse(Fila.Cells["d_Precio"].Text.ToString()) * (decimal.Parse(Fila.Cells["d_Descuento"].Text.ToString())) / 100), (int)Globals.ClientSession.i_PrecioDecimales);
                Fila.Cells["d_PrecioMinDolares"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_PrecioMinSoles"].Text.ToString()) / decimal.Parse(TipoCambio)), (int)Globals.ClientSession.i_PrecioDecimales);
            }
            else
            {
                Fila.Cells["d_PrecioMinSoles"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Precio"].Text.ToString()) - (decimal.Parse(Fila.Cells["d_Precio"].Text.ToString()) * (decimal.Parse(Fila.Cells["d_Descuento"].Text.ToString())) / 100), (int)Globals.ClientSession.i_PrecioDecimales);
                Fila.Cells["d_PrecioMinDolares"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_PrecioMinSoles"].Text.ToString()) / decimal.Parse(TipoCambio)), (int)Globals.ClientSession.i_PrecioDecimales);
            }

        }
        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void FormatoDecimalesGrilla(int DecimalesCantidad, int DecimalesPrecio, int DecimalesUtilidad)
        {
            string  FormatoPrecio;
            string FormatoUtilidad;
            UltraGridColumn _Precio = this.grdData.DisplayLayout.Bands[0].Columns["d_Precio"];
            _Precio.MaskDataMode = MaskMode.IncludeLiterals;
            _Precio.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _PrecioSoles = this.grdData.DisplayLayout.Bands[0].Columns["d_PrecioMinSoles"];
            _PrecioSoles.MaskDataMode = MaskMode.IncludeLiterals;
            _PrecioSoles.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _PrecioDolares = this.grdData.DisplayLayout.Bands[0].Columns["d_PrecioMinDolares"];
            _PrecioDolares.MaskDataMode = MaskMode.IncludeLiterals;
            _PrecioDolares.MaskDisplayMode = MaskMode.IncludeLiterals;



            UltraGridColumn _Costo= this.grdData.DisplayLayout.Bands[0].Columns["Costo"];
            _Costo.MaskDataMode = MaskMode.IncludeLiterals;
            _Costo.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _Utilidad = this.grdData.DisplayLayout.Bands[0].Columns["d_Utilidad"];
            _Utilidad.MaskDataMode = MaskMode.IncludeLiterals;
            _Utilidad.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _Descuento = this.grdData.DisplayLayout.Bands[0].Columns["d_Descuento"];
            _Utilidad.MaskDataMode = MaskMode.IncludeLiterals;
            _Utilidad.MaskDisplayMode = MaskMode.IncludeLiterals;

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

            if (DecimalesUtilidad > 0)
            {
                string sharp = "n";
                FormatoUtilidad = "-nnnnnnnnnnnnnnnn.";

                for (int i = 0; i < DecimalesUtilidad; i++)
                {
                    FormatoUtilidad = FormatoUtilidad + sharp;
                }

            }
            else
            {
                FormatoUtilidad = "-nnnnnnnnnnnnnnnn";
            }

           

            //_Cantidad.MaskInput = FormatoCantidad;
            _Precio.MaskInput = FormatoPrecio;
            _PrecioSoles.MaskInput = FormatoPrecio;
            _PrecioDolares.MaskInput = FormatoPrecio;
            _Costo.MaskInput = FormatoPrecio;
            _Utilidad.MaskInput = FormatoUtilidad;
            _Descuento.MaskInput = FormatoUtilidad;
        }

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            //int idAlmacen;

            //if (e.Cell.Column.Key == "v_CodInterno")
            //{
            //    idAlmacen = Convert.ToInt32(cboAlmacen.SelectedValue.ToString());
            //    Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(idAlmacen, "Salida");

            //    frm.ShowDialog();

            //    if (frm._NombreProducto != null)
            //    {

            //        foreach (UltraGridRow Fila in grdData.Rows)
            //        {
            //            if (Fila.Cells["v_CodInterno"].Value != null)
            //            {

            //                if (frm._CodigoInternoProducto == Fila.Cells["v_CodInterno"].Value.ToString())
            //                {
            //                    UltraMessageBox.Show("El producto ya existe para este almacén ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                    return;
            //                }
            //            }
            //        }


            //        grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value = frm._NombreProducto.ToString();
            //        grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value = frm._CodigoInternoProducto.ToString();
            //        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value = frm._IdProductoAlmacen != null ? frm._IdProductoAlmacen.ToString() : string.Empty;
            //        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
            //        UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Descuento"];
            //        this.grdData.ActiveCell = aCell;
            //        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
            //        aCell.Activate();
            //    }

            //}
        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdData.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grdData.ActiveCell.Column.Key)
                {
                    case "d_Precio":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_Descuento":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;


                }
            }
        }


        private void BuscarProductos()
        {
           
           // var x = new ListaPreciosBL().DevolverListaProductosDto(strIdListaPrecio, int.Parse(cboAlmacen.Value.ToString()));

            //for (int i = 0; i < x.Count(); i++)
            //{
            //   if (grdData.Rows[i].Cells["v_IdProductoAlmacen"].Value!=null &&  grdData.Rows[i].Cells["v_IdProductoAlmacen"].Value.ToString () == x[i].v_IdProductoAlmacen.ToString ()) // If y Else se agrego domngo 13 marzo
            //    {
            //    grdData.Rows[i].Cells["v_CodInterno"].Value = x[i].v_CodInterno;
            //    grdData.Rows[i].Cells["v_Descripcion"].Value = x[i].v_Descripcion;
            //    grdData.Rows[i].Cells["Costo"].Value = x[i].d_Costo;
            //    grdData.Rows[i].Cells["v_IdProductoDetalle"].Value = x[i].v_IdProductoDetalle;
            //    grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
            //    }
            //   else
            //   {
            //       UltraMessageBox.Show("Hubo un Error al Cargar Datos, Contactese  con el Administrador de Sistema", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //       return;

            //   }
            //}


            for (int i = 0; i < grdData.Rows.Count (); i++)
            {
                if (grdData.Rows[i].Cells["v_IdProductoDetalle"].Value != null ) // If y Else se agrego domngo 13 marzo
                {
                    //grdData.Rows[i].Cells["v_CodInterno"].Value = x[i].v_CodInterno;
                    //grdData.Rows[i].Cells["v_Descripcion"].Value = x[i].v_Descripcion;
                    //grdData.Rows[i].Cells["Costo"].Value = x[i].d_Costo;
                    //grdData.Rows[i].Cells["v_IdProductoDetalle"].Value = x[i].v_IdProductoDetalle;
                    grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                }
                else
                {
                    UltraMessageBox.Show("Hubo un Error al Cargar Datos, Contactese  con el Administrador de Sistema", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;

                }
            }
        }
        private void CargarCabecera(string idListaPrecio)
        {
            OperationResult objOperationResult = new OperationResult();
            _objListaPrecio = new listaprecioDto();
            _objListaPrecio = _objListaPrecioBL.ObtenerListaPrecioCabecera(ref objOperationResult, idListaPrecio);
            if (_objListaPrecio != null)
            {
                _Mode = "Edit";
                cboAlmacen.Value = _objListaPrecio.i_IdAlmacen.ToString();
                cboListaPrecios.Value = _objListaPrecio.i_IdLista.ToString();
                cboMoneda.Value = _objListaPrecio.i_IdMoneda.ToString();
                CargarDetalle(_objListaPrecio.v_IdListaPrecios , int.Parse (cboAlmacen.Value.ToString ()));

            }
        }

        private void CargarDetalle(string pstridListaPrecio ,int Almacen)
        {
            OperationResult objOperationResult = new OperationResult();
            //grdData.DataSource = _objListaPrecioBL.ObtenerListaPreciosDetalles(ref objOperationResult, pstridListaPrecio); // hasta sabado 12 marzo
            grdData.DataSource = _objListaPrecioBL.ObtenerListaPrecioDetallePrueba(ref objOperationResult, pstridListaPrecio);
            BuscarProductos();
            OrdenarGrilla();
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", grdData.Rows.Count());
           // InicializarIndex();

        }
        private void InicializarIndex()
        {
            int Index = 0;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                
                Fila.Cells["Index"].Value = (Index + 1).ToString("00");
                Index = Index + 1;
            }
        }
        private void OrdenarGrilla()
        {
            UltraGridBand band = this.grdData.DisplayLayout.Bands[0];

            band.Columns["v_CodInterno"].SortIndicator = SortIndicator.Ascending;

            grdData.ActiveRow = grdData.Rows[0];

            
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

                                _objListaPrecioDetalle = new listapreciodetalleDto();
                                _objListaPrecioDetalle.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _objListaPrecioDetalle.v_IdListaPrecios = _objListaPrecio.v_IdListaPrecios;
                                _objListaPrecioDetalle.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Text .ToString()); //value
                                _objListaPrecioDetalle.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Text.ToString());
                                _objListaPrecioDetalle.d_PrecioMinSoles = Fila.Cells["d_PrecioMinSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioMinSoles"].Text .ToString()); //value
                                _objListaPrecioDetalle.d_PrecioMinDolares = Fila.Cells["d_PrecioMinDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioMinDolares"].Text.ToString());//value
                                _objListaPrecioDetalle.d_Utilidad = Fila.Cells["d_Utilidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Utilidad"].Text.ToString()); //value
                                _objListaPrecioDetalle.d_Costo = Fila.Cells["Costo"].Value == null ? 0 : decimal.Parse(Fila.Cells["Costo"].Text.ToString());
                                _objProducto.v_CodInterno = Fila.Cells["v_CodInterno"].Value == null ? null : Fila.Cells["v_CodInterno"].Value.ToString();
                                _objListaPrecioDetalle.i_IdAlmacen = int.Parse ( cboAlmacen.Value.ToString());  //Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Text.ToString()); //value
                                _objProducto.d_PrecioCosto = Fila.Cells["Costo"].Value == null ? 0 : decimal.Parse(Fila.Cells["Costo"].Text.ToString());//value
                                _objProducto.d_PrecioVenta = _objListaPrecioDetalle.d_Precio; //value
                                _objProducto.d_Utilidad = _objListaPrecioDetalle.d_Utilidad;
                                _TempDetalle_AgregarDto.Add(_objListaPrecioDetalle);
                                _TempProducto_ModificarDto.Add(_objProducto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _objListaPrecioDetalle = new listapreciodetalleDto();
                                _objListaPrecioDetalle.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _objListaPrecioDetalle.v_IdListaPrecios = Fila.Cells["v_IdListaPrecios"].Value.ToString();
                                _objListaPrecioDetalle.v_idListaPrecioDetalle = Fila.Cells["v_IdListaPrecioDetalle"].Value.ToString();
                                _objListaPrecioDetalle.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Text .ToString()); //value
                                _objListaPrecioDetalle.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Text.ToString());//value
                                _objListaPrecioDetalle.d_PrecioMinSoles = Fila.Cells["d_PrecioMinSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioMinSoles"].Text .ToString()); //value
                                _objListaPrecioDetalle.d_PrecioMinDolares = Fila.Cells["d_PrecioMinDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioMinDolares"].Text .ToString());//value
                                _objListaPrecioDetalle.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _objListaPrecioDetalle.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _objListaPrecioDetalle.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Text.ToString());
                                _objListaPrecioDetalle.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _objListaPrecioDetalle.d_Utilidad = Fila.Cells["d_Utilidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Utilidad"].Text.ToString());//value
                                _objListaPrecioDetalle.d_Costo = Fila.Cells["Costo"].Value == null ? 0 : decimal.Parse(Fila.Cells["Costo"].Text.ToString());
                                _objProducto.v_CodInterno = Fila.Cells["v_CodInterno"].Value == null ? null : Fila.Cells["v_CodInterno"].Value.ToString();
                                _objProducto.d_PrecioCosto = Fila.Cells["Costo"].Value == null ? 0 : decimal.Parse(Fila.Cells["Costo"].Text.ToString());//value
                                _objProducto.d_PrecioVenta = _objListaPrecioDetalle.d_Precio; //value
                                _objProducto.d_Utilidad = _objListaPrecioDetalle.d_Utilidad;
                                _TempDetalle_ModificarDto.Add(_objListaPrecioDetalle);
                                _TempProducto_ModificarDto.Add(_objProducto);

                            }
                            break;
                    }
                }
            }

        }
        #endregion
        private void txtBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            //List<string> Filters = new List<string>();
            //OperationResult objOperationResult = new OperationResult();
            //string celda;
            //bool Encontrado = false;
            //if (e.KeyCode == Keys.Enter)
            //{

            //    string CodBuscar = txtBuscarProducto.Text.Trim().ToUpper();
            //    if (CodBuscar != string.Empty)
            //    {
            //        foreach (UltraGridRow row in grdData.Rows)
            //        {

            //            if (row.Cells["v_CodInterno"].Value != null) // Solo busca código Interno
            //            {
            //                celda = row.Cells["v_CodInterno"].Value.ToString().ToUpper();
            //                if (celda.Contains(CodBuscar))
            //                {
            //                    grdData.Selected.Cells.Add(row.Cells["v_CodInterno"]);
            //                    grdData.Focus();
            //                    row.Activate();
            //                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            //                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodInterno"];
            //                    this.grdData.ActiveCell = aCell;
            //                    Encontrado = true;
            //                    return;

            //                }
            //            }

            //            if (row.Cells["v_Descripcion"].Value != null) // Solo busca Descripcion
            //            {
            //                celda = row.Cells["v_Descripcion"].Value.ToString().ToUpper();
            //                if (celda.Contains(CodBuscar))
            //                {
            //                    grdData.Selected.Cells.Add(row.Cells["v_Descripcion"]);
            //                    grdData.Focus();
            //                    row.Activate();
            //                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            //                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_Descripcion"];
            //                    this.grdData.ActiveCell = aCell;
            //                    Encontrado = true;
            //                    return;

            //                }
            //            }
            //        }

            //        if (Encontrado == false)
            //        {
            //            grdData.ActiveRow.Selected = false;
            //        }
            //    }
            //    else
            //    {
            //        foreach (UltraGridRow row in grdData.Rows)
            //        {
            //            if (row.Cells["v_Descripcion"].Selected == true)
            //            {
            //                row.Cells["v_Descripcion"].Selected = false;
            //            }
            //            if (row.Cells["v_CodInterno"].Selected == true)
            //            {
            //                row.Cells["v_CodInterno"].Selected = false;
            //            }
            //        }
            //    }
            //}
          
        }
        private void BindGrid()
        {
            var objData = GetData("v_CodInterno ASC", _strFilterExpression);
            grdData.DataSource = objData;
           

        }
        private List<listapreciodetalleDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var _objData = _objListaPrecioBL.ListarBusquedaListaPreciosDetalle(ref objOperationResult, pstrSortExpression, pstrFilterExpression);


            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string IdListaPrecio = _objListaPrecioBL.ObtenerIdListaPreciostr(ref  objOperationResult, int.Parse(cboAlmacen.Value.ToString()), int.Parse(cboListaPrecios.Value.ToString()));
            List<string> ProductosAgregar = _objListaPrecioBL.ObtenerMinusProductoAlmacen(int.Parse(cboAlmacen.Value.ToString()), IdListaPrecio);
            int i = 0;
            string codProducto = "";

            if (ProductosAgregar.Count == 0)
            {
                UltraMessageBox.Show("Todos los Productos de éste almacén se encuentran asignados a la Lista precios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (var ListaProductosAgregar in ProductosAgregar)
            {
                i = i + 1;
                if (grdData.ActiveRow != null)
                {
                    string[] Cadena = new string[4];
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["d_PrecioMinSoles"].Value = "0";
                    row.Cells["d_PrecioMinDolares"].Value = "0";
                    row.Cells["v_IdProductoDetalle"].Value = ListaProductosAgregar.ToString();
                    Cadena = _objListaPrecioBL.DevolverProductos(ListaProductosAgregar.ToString());
                    row.Cells["v_CodInterno"].Value = Cadena[0]; 
                    row.Cells["v_Descripcion"].Value = Cadena[1];
                    row.Cells["Costo"].Value = Cadena[2];

                    if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
                    {

                        row.Cells["d_Precio"].Value = Cadena[4];
                        row.Cells["d_Utilidad"].Value = Cadena[3];
                        row.Cells["d_PrecioMinSoles"].Value = row.Cells["d_Precio"].Value;
                        row.Cells["d_PrecioMinDolares"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(row.Cells["d_PrecioMinSoles"].Text.ToString()) / decimal.Parse(TipoCambio)), (int)Globals.ClientSession.i_PrecioDecimales);
                        
                    }
                    if (i == 1)
                    {
                        codProducto = row.Cells["v_CodInterno"].Value.ToString().Trim();
                    }

                }
                else
                {
                    string[] Cadena = new string[4];
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["d_PrecioMinSoles"].Value = "0";
                    row.Cells["d_PrecioMinDolares"].Value = "0";
                    row.Cells["v_IdProductoDetalle"].Value = ListaProductosAgregar.ToString();
                    Cadena = _objListaPrecioBL.DevolverProductos(ListaProductosAgregar.ToString());
                    row.Cells["v_CodInterno"].Value = Cadena[0];
                    row.Cells["v_Descripcion"].Value = Cadena[1];
                    row.Cells["Costo"].Value = Cadena[2];
                    if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
                    {

                        row.Cells["d_Precio"].Value = Cadena[4];
                        row.Cells["d_Utilidad"].Value = Cadena[3];
                        row.Cells["d_PrecioMinSoles"].Value = row.Cells["d_Precio"].Value;
                        row.Cells["d_PrecioMinDolares"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(row.Cells["d_PrecioMinSoles"].Text.ToString()) / decimal.Parse(TipoCambio)), (int)Globals.ClientSession.i_PrecioDecimales);


                    }
                    if (i == 1)
                    {
                        codProducto = row.Cells["v_CodInterno"].Value.ToString().Trim();
                    }


                }

                btnAgregar.Enabled = false;
            }


            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if ( Fila.Cells["v_CodInterno"].Value !=null &&  Fila.Cells["v_CodInterno"].Value.ToString().Trim() == codProducto)
                {

                    grdData.ActiveRow = Fila;
                    grdData.ActiveRow.Selected = true;
                    break;


                }
            }

        }
        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

           
        }
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell == null) return;

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
        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //foreach (UltraGridRow Fila in grdData.Rows)
            //{
            //    Fila.Cells["Index"].Value = (Fila.Index + 1).ToString("00");
            //}
        }
        private void btnMigracion_Click(object sender, EventArgs e)
        {
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
            UltraMessageBox.Show("Todos los Cáculos han finalizado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        private void frmListaPrecios_Fill_Panel_PaintClient(object sender, PaintEventArgs e)
        {

        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult= new OperationResult ();

            Reportes.Ventas.frmDocumentoListaPrecios frm = new Reportes.Ventas.frmDocumentoListaPrecios(int.Parse(cboAlmacen.Value.ToString()), int.Parse(cboListaPrecios.Value.ToString()),"D");

            frm.ShowDialog();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            List<UltraGridRow> Filas = new List<UltraGridRow>();
            List<pedidodetalleDto> Filita = new List<pedidodetalleDto>();

            if (grdData.ActiveRow == null) return;

            if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _objListaPrecioDetalle = new listapreciodetalleDto();
                _TempDetalle_EliminarDto.Add(_objListaPrecioDetalle);
                grdData.Rows[grdData.ActiveRow.Index].Delete(false);
            }
        }

        private void txtBuscarProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            myTimer.Stop();
            myTimer.Start();
        }

       
       

    }
}
