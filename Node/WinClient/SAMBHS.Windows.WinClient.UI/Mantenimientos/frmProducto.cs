using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Security.BL;
using System.IO;
using System.Threading.Tasks;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using LoadingClass;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmProducto : Form
    {
        #region Properties
        private readonly UltraCombo _ucAlmacen = new UltraCombo();
        readonly ProductoBL _objProductoBl = new ProductoBL();
        readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        readonly SecurityBL _obSecurityBl = new SecurityBL();
        productoDto _productoDto;
        productodetalleDto _productodetalleDto;
        readonly ColorBL _objColorBl = new ColorBL();
        readonly TallaBL _objTallaBl = new TallaBL();
        readonly LineaBL _objLineaBl = new LineaBL();
        readonly MarcaBL _objMarcaBl = new MarcaBL();
        readonly ModeloBL _objModeloBl = new ModeloBL();
        colorDto _colorDto;
        tallaDto _tallaDto;
        lineaDto _lineaDto;
        marcaDto _marcaDto;
        modeloDto _modeloDto;
        bool IngresoFormulario = true;
        string _idProducto;
        string _idProductoDetalle;
        string _modeDatos = string.Empty;
        string _modeDetalleColor;
        string _modeDetalleTalla;
        string _modeDetalleLinea;
        string _modeDetalleMarca;
        string _modeDetalleModelo;
        string _strFilterExpression;
        decimal CostoInicial = 0;
        bool ActualizarListaPrecio = false;
        string TipoCambio = "0";
        string _codigoInternoOld;
        bool _btnNuevoProducto, _btnEliminarProducto, _btnGuardarProducto, _btnGuardarAtributos, _btnAgregarLinea, _btnEditarLinea, _btnEliminarLinea, _btnAgregarMarca, _btnEditarMarca, _btnEliminarMarca, _btnAgregarModelo, _btnEditarModelo, _btnEliminarModelo;
        bool _btnAgregarTalla, _btnEditarTalla, _btnEliminarTalla, _btnAgregarColor, _btnEditarColor, _btnEliminarColor;
        Timer _myTimer = new Timer();
        Dictionary<string, string> CodigoGen = new Dictionary<string, string>();

        private bool isHormigita;
        private byte[] imageLinea = null;
        #endregion

        public frmProducto(string Parametro)
        {
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;

            TbsProducto.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();

            _modeDetalleColor = _modeDetalleTalla = "New";
            _modeDetalleLinea = _modeDetalleMarca = "New";
            _modeDetalleModelo = "New";
            // TbsProducto.Controls["tabPage1"].Enabled = false;
            //TbsProducto.Controls["tabAtributos"].Enabled = false;
            TbsProducto.Tabs["tabPage1"].Enabled = false;
            TbsProducto.Tabs["tabAtributos"].Enabled = false;

            #region CONTROL DE ACCIONES
            var _formActions = _obSecurityBl.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmProducto", Globals.ClientSession.i_RoleId);

            _btnNuevoProducto = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_New", _formActions);
            _btnEliminarProducto = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Delete", _formActions);
            _btnGuardarProducto = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Save", _formActions);
            _btnGuardarAtributos = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Atributos_Save", _formActions);
            _btnAgregarLinea = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Linea_Add", _formActions);
            _btnEditarLinea = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Linea_Edit", _formActions);
            _btnEliminarLinea = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Linea_Delete", _formActions);

            _btnAgregarMarca = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Marca_Add", _formActions);
            _btnEliminarMarca = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Marca_Delete", _formActions);
            _btnEditarMarca = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Marca_Edit", _formActions);


            _btnAgregarModelo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Modelo_Add", _formActions);
            _btnEliminarModelo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Modelo_Delete", _formActions);
            _btnEditarModelo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Modelo_Edit", _formActions);

            _btnAgregarTalla = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Talla_Add", _formActions);
            _btnEliminarTalla = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Talla_Delete", _formActions);
            _btnEditarTalla = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Talla_Edit", _formActions);

            _btnAgregarColor = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Color_Add", _formActions);
            _btnEliminarColor = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Color_Delete", _formActions);
            _btnEditarColor = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmProducto_Color_Edit", _formActions);

            btnNuevoProducto.Enabled = _btnNuevoProducto;
            btnEliminarProducto.Enabled = _btnEliminarProducto;
            btnDatosGrabar.Enabled = _btnGuardarProducto;
            btnAtributosGrabar.Enabled = _btnGuardarAtributos;

            btnLineaAgregar.Enabled = _btnAgregarLinea;
            btnLineaEditar.Enabled = _btnEditarLinea;
            btnLineaEliminar.Enabled = _btnEliminarLinea;

            btnMarcaAgregar.Enabled = _btnAgregarMarca;
            btnMarcaEditar.Enabled = _btnEditarMarca;
            btnMarcaEliminar.Enabled = _btnEliminarMarca;

            btnModeloAgregar.Enabled = _btnAgregarModelo;
            btnModeloEditar.Enabled = _btnEditarModelo;
            btnModeloEliminar.Enabled = _btnEliminarModelo;

            btnTallaAgregar.Enabled = _btnAgregarTalla;
            btnTallaEditar.Enabled = _btnEditarTalla;
            btnTallaEliminar.Enabled = _btnEliminarTalla;


            btnColorAgregar.Enabled = _btnAgregarColor;
            btnColorEditar.Enabled = _btnEditarColor;
            btnColorEliminar.Enabled = _btnEliminarColor;


            #endregion
            CargarCombos();
            _myTimer = new Timer() { Interval = 100 };
            _myTimer.Tick += OnTimedEvent;
            _myTimer.Interval = 500;
            FormatoDecimalesGrilla(Globals.ClientSession.i_CantidadDecimales ?? 2);
            isHormigita = Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita;
            txtDatosCodigo.ReadOnly = isHormigita;
            txtDatosPrecioCosto.Visible = Globals.ClientSession.i_UsaListaPrecios == 1 ? Globals.ClientSession.i_CostoListaPreciosDiferentesxAlmacen == 1 ? false : true : true;
            lblPrecioCosto.Visible = Globals.ClientSession.i_UsaListaPrecios == 1 ? Globals.ClientSession.i_CostoListaPreciosDiferentesxAlmacen == 1 ? false : true : true;

            if (isHormigita)
                LaodEventsHormigita();

            btnBuscar_Click(sender, e);
            lblDescripcion2.Text = Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? "Desc. Inglés " : "Desc. Alterna";
            chkOtrosTributos.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic || Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic2 || Globals.ClientSession.v_RucEmpresa == Constants.RucDemo ? true : false;
            cboDatosTipoProducto.Value = Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? ((int)TipoExistencia.Mercaderia).ToString() : "-1";
            ConfigurarGrilla();
            IngresoFormulario = false;
            txtAtributosAño.Minimum = 0;
            txtAtributosAño.Maximum = 2050;
            txtUtilidad.Enabled = false;
            
        }




        private void ConfigurarGrilla()
        {


            UltraGridColumn NroParte = grdData.DisplayLayout.Bands[0].Columns["v_NroParte"];
            UltraGridColumn Talla = grdData.DisplayLayout.Bands[0].Columns["NombreTalla"];
            UltraGridColumn NombreColor = grdData.DisplayLayout.Bands[0].Columns["NombreColor"];
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucWortec) //
            {
                NroParte.Hidden = false;
                Talla.Hidden = true;
                NombreColor.Hidden = true;
            }
            else
            {
                NroParte.Hidden = true;
                Talla.Hidden = false;
                NroParte.Hidden = false;
            }


        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();

            //Datos
            Utils.Windows.LoadUltraComboList(cboDatosLinea, "Value1", "Id", _objLineaBl.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDatosMarca, "Value1", "Id", _objMarcaBl.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDatosTalla, "Value1", "Id", _objTallaBl.LlenarComboTalla(ref objOperationResult, "v_CodTalla"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDatosColor, "Value1", "Id", _objColorBl.LlenarComboColor(ref objOperationResult, "v_CodColor"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboDatosTipoProducto, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 6, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboUndMedida, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 17, null), DropDownListAction.Select);

            //Atributos            
            Utils.Windows.LoadUltraComboEditorList(cboAtributosProveedor, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 7, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosTipo, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 8, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosUsuario, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 9, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosTela, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 10, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosEtiqueta, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 11, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosCuello, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 12, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosAplicacion, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 13, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosMaterial, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 14, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosColeccion, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 15, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosTemporada, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 16, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboTipoTributo, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 166, null), DropDownListAction.Select);

            Utils.Windows.LoadUltraComboList(cboPerfilDetraccion, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 176, null), DropDownListAction.Select);

            Utils.Windows.LoadDropDownList(cboModeloLinea, "Value1", "Id", _objLineaBl.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboModeloMarca, "Value1", "Id", _objMarcaBl.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.Select);
            CargarCombosDetalleReceta();
            //Sistema ISC
            Utils.Windows.LoadUltraComboEditorList(ucTipoIsc, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 151, null), DropDownListAction.Select);
            cboEstadoProducto.Value = "-1";
            cboTipoTributo.Value = "-1";
        }

        #region Bandeja

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var filters = new Queue<string>();
            _strFilterExpression = filters.Any() ? string.Join(" && ", filters) : null;
            BindGrid();
        }

        private void BindGrid()
        {
            var objData = new List<productoDto>();
            pBuscando.Visible = true;
            Task.Factory.StartNew(() =>
            {
                objData = GetData("v_Descripcion ASC", _strFilterExpression);
            }, TaskCreationOptions.AttachedToParent).ContinueWith(t =>
            {
                grdData.DataSource = objData;
                if (objData.Count > 0)
                {
                    if (_idProducto != null)
                    {
                        MantenerSeleccion(_idProducto);
                        object sender = new object();
                        char h = 'x';
                        KeyPressEventArgs e = new KeyPressEventArgs(h);
                        txtDescripción_KeyPress(sender, e);
                        TbsProducto.Tabs["tabPage1"].Enabled = true;
                        TbsProducto.Tabs["tabAtributos"].Enabled = true;
                        TbsProducto.Tabs["tabPage3"].Enabled = true;
                        TbsProducto.Tabs["tabPage4"].Enabled = true;
                        TbsProducto.Tabs["tabPage5"].Enabled = true;
                        TbsProducto.Tabs["tabPage6"].Enabled = true;
                        TbsProducto.Tabs["tabPage7"].Enabled = true;
                    }
                    else
                    {
                        grdData.Rows[0].Selected = true;
                        _idProducto = grdData.Selected.Rows[0].Cells["v_IdProducto"].Value.ToString();
                        _idProductoDetalle = grdData.Selected.Rows[0].Cells["v_IdProductoDetalle"].Value.ToString();
                        TbsProducto.Tabs["tabPage1"].Enabled = true;
                        TbsProducto.Tabs["tabAtributos"].Enabled = true;
                    }
                }
                else
                {
                    LimpiarDatos();
                    TbsProducto.Tabs["tabPage1"].Enabled = false;
                    TbsProducto.Tabs["tabAtributos"].Enabled = false;
                    TbsProducto.Tabs["tabPage3"].Enabled = false;
                    TbsProducto.Tabs["tabPage4"].Enabled = false;
                    TbsProducto.Tabs["tabPage5"].Enabled = false;
                    TbsProducto.Tabs["tabPage6"].Enabled = false;
                    TbsProducto.Tabs["tabPage7"].Enabled = false;
                }
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                pBuscando.Visible = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<productoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            var objOperationResult = new OperationResult();
            var objData = _objProductoBl.ListarProducto(ref objOperationResult, pstrSortExpression, pstrFilterExpression, IngresoFormulario ? 0 : int.Parse(cboEstadoProducto.Value.ToString()));

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return objData;
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (chkEdicionMultiple.Checked) return;
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow) return;
            if (e.Button == MouseButtons.Left)
            {
                Point point = new Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                UltraGridRow row = (UltraGridRow)uiElement.GetContext(typeof(UltraGridRow));

                if (row != null)
                {
                    grdData.Rows[row.Index].Selected = true;
                    btnNuevoProducto.Enabled = _btnNuevoProducto;
                    btnEliminarProducto.Enabled = _btnEliminarProducto;

                    _idProducto = grdData.Selected.Rows[0].Cells["v_IdProducto"].Value.ToString();
                    _idProductoDetalle = grdData.Selected.Rows[0].Cells["v_IdProductoDetalle"].Value.ToString();
                    TbsProducto.Tabs["tabPage1"].Enabled = true;
                    TbsProducto.Tabs["tabAtributos"].Enabled = true;
                    //CargarDatos(_idProducto);
                }
                else
                {
                    btnEliminarProducto.Enabled = false;
                }
            }
        }

        private void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            _modeDatos = "New";
            _productoDto = new productoDto();
            LimpiarDatos();

            TbsProducto.Tabs["tabPage1"].Enabled = true;
            TbsProducto.SelectedTab = TbsProducto.Tabs["tabPage1"];
            TbsProducto.Tabs["tabAtributos"].Enabled = false;
            TbsProducto.Tabs["tabReceta"].Visible = false;
            btnDatosGrabar.Enabled = _btnGuardarProducto;
            txtDatosCodigo.ReadOnly = false;
            txtDatosCodigo.Select();

            if (grdData.Rows.Count > 0)
            {
                grdData.Rows[0].Selected = false;
            }

        }

        private void btnMatriz_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmGenerarProductoMatriz))) return;
            var frm = new frmGenerarProductoMatriz();
            frm.FormClosed += delegate
            {
                InvokeOnClick(btnBuscar, null);
            };
            ((frmMaster)MdiParent).RegistrarForm(this, frm);
        }

        private void btnEliminarProducto_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada

            string pstrIdProducto = grdData.Selected.Rows[0].Cells["v_IdProducto"].Value.ToString();
            if (_objProductoBl.MovimientosProducto(ref objOperationResult, pstrIdProducto))
            {
                UltraMessageBox.Show("Este producto está siendo utilizado,no se puede eliminar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;
            _objProductoBl.EliminarProducto(ref objOperationResult, pstrIdProducto, Globals.ClientSession.GetAsList());

            btnBuscar_Click(sender, e);
            LimpiarDatos();
            TbsProducto.Tabs["tabPage1"].Enabled = false;
            TbsProducto.Tabs["tabAtributos"].Enabled = false;
            btnEliminarProducto.Enabled = false;
            //btnNuevoProducto.Enabled = false;
        }

        void CargarDatos(string pstrIdProducto)
        {
            _modeDatos = "";
            var objOperationResult = new OperationResult();
            _productoDto = new productoDto();
            _productoDto = _objProductoBl.ObtenerProducto(ref objOperationResult, pstrIdProducto);
            _productodetalleDto = _objProductoBl.ObtenerProductoDetalle(ref objOperationResult, _productoDto.v_IdProductoDetalle);
            txtDatosCodigo.ReadOnly = true;
            _codigoInternoOld = _productoDto.v_CodInterno;
            txtDatosCodigo.Text = _productoDto.v_CodInterno;
            cboDatosLinea.Value = _productoDto.v_IdLinea ?? "-1";
            cboPerfilDetraccion.Value = (_productoDto.i_IdPerfilDetraccion ?? -1).ToString();
            cboDatosMarca.Value = _productoDto.v_IdMarca ?? "-1";
            txtDatosModelo.Text = _productoDto.v_Modelo;
            txtDatosNroModel.Text = (txtDatosCodigo.TextLength > 8) ? txtDatosCodigo.Text.Substring(5, 3) : string.Empty;
            cboDatosTalla.Value = _productodetalleDto.v_IdTalla ?? "-1";
            cboDatosColor.Value = _productodetalleDto.v_IdColor ?? "-1";
            txtDatosDescripcion.Text = _productoDto.v_Descripcion;
            txtDatosEmpaque.Text = _productoDto.d_Empaque == (decimal)0.000 ? string.Empty : _productoDto.d_Empaque.ToString();
            cboUndMedida.Value = _productoDto.i_IdUnidadMedida.ToString();
            txtDatosPeso.Text = _productoDto.d_Peso == (decimal) 0.000 ? string.Empty : _productoDto.d_Peso.Value.ToString("N3");
            txtDatosUbicacion.Text = _productoDto.v_Ubicacion;
            txtDatosCaracteristicas.Text = _productoDto.v_Caracteristica;
            txtDatosCodProveedor.Text = _productoDto.v_CodProveedor;
            txtDatosDescripcion2.Text = _productoDto.v_Descripcion2;
            cboDatosTipoProducto.Value = _productoDto.i_IdTipoProducto.ToString();
            chkDatosServicio.Checked = _productoDto.i_EsServicio != 0;
            chkAfectoDetraccion.Checked = _productoDto.i_EsAfectoDetraccion != 0;
            chkDatosLote.Checked = _productoDto.i_EsLote != 0;
            chkDatosActivo.Checked = _productoDto.i_EsActivo != 0;
            txtDatosPrecioVenta.Text = _productoDto.d_PrecioVenta == (decimal)0.000 ? string.Empty : _productoDto.d_PrecioVenta.Value.ToString("N2");
            chkAfectoPercepcion.Checked = _productoDto.i_EsAfectoPercepcion == 1;
            txtTasaPercepcion.Enabled = _productoDto.i_EsAfectoPercepcion == 1;
            txtTasaPercepcion.Text = _productoDto.d_TasaPercepcion.ToString();
            txtNroPartidaArancelaria.Text = _productoDto.v_NroPartidaArancelaria;
            cboTipoTributo.Value = _productoDto.i_IdTipoTributo.ToString();
            txtUtilidad.Text = _productoDto.d_Utilidad.Value.ToString("N2");
            txtPrecioMayorista.Text = _productoDto.d_PrecioMayorista.Value.ToString("N2");
            if (cboDatosTipoProducto.Value != null) TbsProducto.Tabs["tabReceta"].Visible = cboDatosTipoProducto.Text.ToUpper().Contains("TERMINADO");

            if (_productoDto.b_Foto != null)
            {
                var fotoPicture = _productoDto.b_Foto;
                pictureProduct.Image = fotoPicture != null ? Utils.Windows.BinaryToImage(fotoPicture) : null;
            }
            else
            {
                var foto = _objLineaBl.ObtenerLinea(ref objOperationResult, _productoDto.v_IdLinea);
                if (foto != null)
                {
                    var fotoPicture = foto.b_Foto;
                    pictureProduct.Image = fotoPicture != null ? Utils.Windows.BinaryToImage(fotoPicture) : null;
                }
            }

            txtDatosPrecioCosto.Text = _productoDto.d_PrecioCosto == null ? "0" : _productoDto.d_PrecioCosto.Value.ToString("N2");
            CostoInicial = decimal.Parse(txtDatosPrecioCosto.Text.Trim().Replace(" ", ""));
            txtDatosStockMin.Text = _productoDto.d_StockMinimo.Value == null ? "0" : Math.Round(_productoDto.d_StockMinimo.Value, 0).ToString();
            //txtDatosStockMax.Text = _productoDto.d_StockMaximo == (decimal)0.000 ? string.Empty : _productoDto.d_StockMaximo.ToString();
            chkDescripcionEditable.Checked = (_productoDto.i_NombreEditable??0) != 0;
            chkNoStock.Checked = (_productoDto.i_ValidarStock??0) != 0;

            chkInsumo.Checked = (_productoDto.i_Insumo ?? 0) != 0;

            cboAtributosProveedor.Value = (_productoDto.i_IdProveedor??0).ToString();
            cboAtributosTipo.Value = (_productoDto.i_IdTipo??-1).ToString();
            cboAtributosUsuario.Value = _productoDto.i_IdUsuario.ToString();
            cboAtributosTela.Value = (_productoDto.i_IdTela??-1).ToString();
            cboAtributosEtiqueta.Value = (_productoDto.i_IdEtiqueta??-1).ToString();
            cboAtributosCuello.Value = (_productoDto.i_IdCuello??-1).ToString();
            cboAtributosAplicacion.Value = (_productoDto.i_IdAplicacion??-1).ToString();
            cboAtributosMaterial.Value = (_productoDto.i_IdArte??-1).ToString();
            cboAtributosColeccion.Value = (_productoDto.i_IdColeccion??-1).ToString();
            cboAtributosTemporada.Value = (_productoDto.i_IdTemporada??-1).ToString();
            txtAtributosAño.Text = _productoDto.i_Anio == 0 ? string.Empty : _productoDto.i_Anio.ToString();
            txtNroOrdenProduccion.Text = (_productoDto.v_NroOrdenProduccion??string.Empty);
            btnDatosGrabar.Enabled = _btnGuardarProducto;
            // chkDescripcionEditable.Enabled = _productoDto.i_EsServicio == 1;
            chkPrecioEditable.Checked = _productoDto.i_PrecioEditable == 1;
            txtNroParte.Text = _productoDto.v_NroParte;
            uchAfectoIsc.Checked = _productoDto.i_EsAfectoIsc != null && (_productoDto.i_EsAfectoIsc == 1);
            txtCantidadFabricacionMensual.Text = _productoDto.i_CantidadFabricacionMensual.ToString();
            chkOtrosTributos.Checked = _productoDto.i_IndicaFormaParteOtrosTributos == null ? false : _productoDto.i_IndicaFormaParteOtrosTributos == 1 ? true : false;
            chkSolicitarNroSerieIngreso.Checked = _productoDto.i_SolicitarNroSerieIngreso == 1 ? true : false;
            chkSolicitarNroLoteIngreso.Checked = _productoDto.i_SolicitarNroLoteIngreso == 1 ? true : false;
            chkSolicitarNroOrdenProduccionIngreso.Checked = _productoDto.i_SolicitaOrdenProduccionIngreso==1?true:false;

            chkSolicitarNroSerieSalida.Checked = _productoDto.i_SolicitarNroSerieSalida == 1 ? true : false;
            chkSolicitarNroLoteSalida.Checked = _productoDto.i_SolicitarNroLoteSalida == 1 ? true : false;
            chkSolicitarNroOrdenProduccionSalida.Checked = _productoDto.i_SolicitaOrdenProduccionSalida == 1 ? true : false;
            LoadDataIsc();
            IsLoadReceta = false;

            if (TbsProducto.Tabs["tabReceta"].Selected)
                LoadRecetasForProduct(_productoDto.v_IdProductoDetalle);


            IsLoadRecetaSalida = false;
            if (TbsProducto.Tabs["tabAtributos"].Selected)
                LoadRecetasSalida(_productoDto.v_IdProductoDetalle);

            _modeDatos = "Edit";
        }

        private void txtAtributosAño_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtAtributosAño, e);
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow.IsFilterRow || chkEdicionMultiple.Checked) return;

            _modeDatos = "Edit";
            string idProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProducto"].Value.ToString();
            CargarDatos(idProducto);
            grdData.Focus();
            ActivarBotonesVentaExportacion();
        }

        private void ActivarBotonesVentaExportacion()
        {
            string Descripcion = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString();
            string TipoTributo = grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdTipoTributo"].Value.ToString();
            btnAñadirRecetaSalida.Enabled = Descripcion.Contains("EXPORT") ? true : false;
            btnEliminarRecetaSalida.Enabled = Descripcion.Contains("EXPORT") ? true : false;
            btnGuardarRecetaSalida.Enabled = Descripcion.Contains("EXPORT") ? true : false;
            cboTipoTributo.Enabled = Descripcion.Contains("FLETE") || Descripcion.Contains("SEGURO") ? true : false;
        }
        #endregion



        #region Edicion 1

        private void btnDatosGrabar_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (uvDatos.Validate(true, false).IsValid)
            {
                #region Validaciones
                if (txtDatosCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese una Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtDatosDescripcion.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese una Descripción.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((string)cboDatosLinea.Value == "-1")
                {
                    UltraMessageBox.Show("Seleccione una Linea.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboDatosLinea.Focus();
                    return;
                }
                if (decimal.Parse(txtDatosEmpaque.Text) < 1.0M)
                {
                    UltraMessageBox.Show("Por favor ingrese un Empaque Válido", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDatosEmpaque.Focus();
                    return;
                }
                if ((string)cboUndMedida.Value == "-1")
                {
                    UltraMessageBox.Show("Seleccione una Unidad de Medida", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboUndMedida.Focus();
                    return;
                }
                if ((string)cboDatosTipoProducto.Value == "-1")
                {
                    UltraMessageBox.Show("Seleccione un Tipo de Producto", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboDatosTipoProducto.Focus();
                    return;
                }
                if (chkAfectoPercepcion.Checked == true)
                {
                    if (string.IsNullOrEmpty(txtTasaPercepcion.Text) || decimal.Parse(txtTasaPercepcion.Text.Trim()) == 0)
                    {
                        UltraMessageBox.Show("Por favor si el artículo es Afecto a Percepción ingrese una tasa (%)", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTasaPercepcion.Focus();
                        return;
                    }
                }
                if (uchAfectoIsc.Checked)
                    if (!uvColor.Validate("ISC", true, false).IsValid)
                    {
                        UltraMessageBox.Show("Falta Algunos Datos para ISC");
                        popupIsc.Show(btnShowIsc);
                        return;
                    }

                if (chkAfectoDetraccion.Checked &&
                    (cboPerfilDetraccion.Value == null || cboPerfilDetraccion.Value.ToString().Equals("-1")))
                {
                    UltraMessageBox.Show("Por favor seleccione un perfil de detracción para el artículo", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboPerfilDetraccion.PerformAction(UltraComboAction.Dropdown);
                    return;
                }

                #endregion

                #region Validacion Hormigita
                if (isHormigita)
                {
                    if ((string)cboDatosMarca.Value == "-1")
                    {
                        UltraMessageBox.Show("Seleccione una Marca.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cboDatosMarca.Focus();
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(txtDatosModelo.Text))
                    {
                        UltraMessageBox.Show("Escriba un Modelo Valido", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDatosModelo.Focus();
                        return;
                    }
                    if (txtDatosNroModel.TextLength != 3)
                    {
                        UltraMessageBox.Show("Necesita ingresar 3 digitos del Modelo", "Error de Validacion!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDatosNroModel.Focus();
                        return;
                    }
                    if ((string)cboDatosTalla.Value == "-1")
                    {
                        UltraMessageBox.Show("Seleccione una Talla.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cboDatosTalla.Focus();
                        return;
                    }
                    if ((string)cboDatosColor.Value == "-1")
                    {
                        UltraMessageBox.Show("Seleccione un Color.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cboDatosColor.Focus();
                        return;
                    }

                }
                #endregion

                if (_modeDatos == "New")
                {

                    if (_objProductoBl.ObtenerProductoCodigo(ref objOperationResult, txtDatosCodigo.Text) != null)
                    {
                        UltraMessageBox.Show("No se grabó porque el Código (" + txtDatosCodigo.Text + ") le pertenece a otro Producto ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    //_productoDto = new productoDto();
                    _productoDto.v_CodInterno = txtDatosCodigo.Text.Trim();
                    _productoDto.v_IdLinea = cboDatosLinea.Value.ToString() == "-1" ? null : cboDatosLinea.Value.ToString();
                    _productoDto.v_IdMarca = cboDatosMarca.Value.ToString() == "-1" ? null : cboDatosMarca.Value.ToString();
                    _productoDto.v_Modelo = txtDatosModelo.Text.Trim();
                    _productoDto.v_IdTalla = cboDatosTalla.Value.ToString() == "-1" ? null : cboDatosTalla.Value.ToString();
                    _productoDto.v_IdColor = cboDatosColor.Value.ToString() == "-1" ? null : cboDatosColor.Value.ToString();
                    _productoDto.v_Descripcion = txtDatosDescripcion.Text;
                    _productoDto.d_Empaque = txtDatosEmpaque.Text == string.Empty ? 0 : decimal.Parse(txtDatosEmpaque.Text.ToString());
                    _productoDto.i_IdUnidadMedida = int.Parse(cboUndMedida.Value.ToString());
                    _productoDto.d_Peso = txtDatosPeso.Text == string.Empty ? 0 : decimal.Parse(txtDatosPeso.Text.ToString());
                    _productoDto.v_Ubicacion = txtDatosUbicacion.Text;
                    _productoDto.v_Caracteristica = txtDatosCaracteristicas.Text;
                    _productoDto.v_CodProveedor = txtDatosCodProveedor.Text;
                    _productoDto.v_Descripcion2 = txtDatosDescripcion2.Text;
                    _productoDto.i_IdTipoProducto = int.Parse(cboDatosTipoProducto.Value.ToString());
                    _productoDto.i_EsServicio = chkDatosServicio.Checked == true ? 1 : 0;
                    _productoDto.i_EsLote = chkDatosLote.Checked == true ? 1 : 0;
                    _productoDto.i_EsAfectoDetraccion = chkAfectoDetraccion.Checked == true ? 1 : 0;
                    _productoDto.i_EsActivo = chkDatosActivo.Checked == true ? 1 : 0;
                    _productoDto.d_PrecioVenta = txtDatosPrecioVenta.Text == string.Empty ? 0 : decimal.Parse(txtDatosPrecioVenta.Text.ToString());
                    _productoDto.d_PrecioCosto = txtDatosPrecioCosto.Text == string.Empty ? 0 : decimal.Parse(txtDatosPrecioCosto.Text.ToString());
                    _productoDto.d_StockMinimo = txtDatosStockMin.Text == string.Empty ? 0 : decimal.Parse(txtDatosStockMin.Text.ToString());
                    _productoDto.d_StockMaximo = 0;
                    _productoDto.i_NombreEditable = chkDescripcionEditable.Checked == true ? 1 : 0;
                    _productoDto.i_ValidarStock = chkNoStock.Checked == true ? 1 : 0;
                    _productoDto.i_Insumo = chkInsumo.Checked == true ? 1 : 0;

                    _productoDto.i_IdProveedor = -1;
                    _productoDto.i_IdTipo = -1;
                    _productoDto.i_IdUsuario = -1;
                    _productoDto.i_IdTela = -1;
                    _productoDto.i_IdEtiqueta = -1;
                    _productoDto.i_IdCuello = -1;
                    _productoDto.i_IdAplicacion = -1;
                    _productoDto.i_IdArte = -1;
                    _productoDto.i_IdColeccion = -1;
                    _productoDto.i_IdTemporada = -1;
                    _productoDto.i_Anio = 0;
                    _productoDto.i_EsAfectoPercepcion = chkAfectoPercepcion.Checked ? 1 : 0;
                    _productoDto.d_TasaPercepcion = string.IsNullOrEmpty(txtTasaPercepcion.Text) ? 0 : decimal.Parse(txtTasaPercepcion.Text.Trim());
                    _productoDto.i_PrecioEditable = chkPrecioEditable.Checked ? 1 : 0;
                    _productoDto.i_EsActivoFijo = 0;
                    _productoDto.i_EsAfectoIsc = (short)(uchAfectoIsc.Checked ? 1 : 0);
                    _productoDto.i_CantidadFabricacionMensual = string.IsNullOrEmpty(txtCantidadFabricacionMensual.Text) ? 0 : int.Parse(txtCantidadFabricacionMensual.Text.Trim());
                    _productoDto.v_NroPartidaArancelaria = txtNroPartidaArancelaria.Text.Trim();
                    _productoDto.i_IndicaFormaParteOtrosTributos = chkOtrosTributos.Checked ? 1 : 0;
                    _productoDto.i_IdPerfilDetraccion = cboPerfilDetraccion.Value != null ? int.Parse(cboPerfilDetraccion.Value.ToString()) : -1;
                    _productoDto.v_NroParte = txtNroParte.Text.Trim();
                    _productoDto.d_Utilidad = decimal.Parse(txtUtilidad.Text);
                    _productoDto.d_PrecioMayorista = string.IsNullOrEmpty(txtPrecioMayorista.Text) ? 0 : decimal.Parse(txtPrecioMayorista.Text);
                    _productoDto.i_SolicitarNroLoteIngreso = chkSolicitarNroLoteIngreso.Checked ? 1 : 0;
                    _productoDto.i_SolicitarNroSerieIngreso = chkSolicitarNroSerieIngreso.Checked ? 1 : 0;
                    _productoDto.i_SolicitaOrdenProduccionIngreso = chkSolicitarNroOrdenProduccionIngreso.Checked ? 1 : 0;
                    _productoDto.i_SolicitarNroLoteSalida = chkSolicitarNroLoteSalida.Checked ? 1 : 0;
                    _productoDto.i_SolicitarNroSerieSalida = chkSolicitarNroSerieSalida.Checked ? 1 : 0;
                    _productoDto.i_SolicitaOrdenProduccionSalida = chkSolicitarNroOrdenProduccionSalida.Checked ? 1 : 0;
                    // Save the data
                    string valor = _objProductoBl.InsertarProducto(ref objOperationResult, _productoDto, Globals.ClientSession.GetAsList());

                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(
                            objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                            objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var id2 = valor.Split(';');
                    _idProducto = id2[0];
                    _idProductoDetalle = id2[1];
                    GuardarIsc();

                }
                else if (_modeDatos == "Edit")
                {

                    if (_codigoInternoOld != txtDatosCodigo.Text.Trim().ToUpper())
                    {
                        if (_objProductoBl.ObtenerProductoCodigo(ref objOperationResult, txtDatosCodigo.Text) != null)
                        {
                            UltraMessageBox.Show("No se grabó porque el Código (" + txtDatosCodigo.Text + ") le pertenece a otro Producto ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    _productoDto.v_CodInterno = txtDatosCodigo.Text.Trim();
                    _productoDto.v_IdLinea = cboDatosLinea.Value.ToString() == "-1" ? null : cboDatosLinea.Value.ToString();
                    _productoDto.v_IdMarca = cboDatosMarca.Value.ToString() == "-1" ? null : cboDatosMarca.Value.ToString();
                    _productoDto.v_Modelo = txtDatosModelo.Text.Trim();
                    _productodetalleDto.v_IdTalla = cboDatosTalla.Value.ToString() == "-1" ? null : cboDatosTalla.Value.ToString();
                    _productodetalleDto.v_IdColor = cboDatosColor.Value.ToString() == "-1" ? null : cboDatosColor.Value.ToString();
                    _productoDto.v_Descripcion = txtDatosDescripcion.Text;
                    _productoDto.d_Empaque = txtDatosEmpaque.Text == string.Empty ? 0 : decimal.Parse(txtDatosEmpaque.Text.ToString());
                    _productoDto.i_IdUnidadMedida = int.Parse(cboUndMedida.Value.ToString());
                    _productoDto.d_Peso = txtDatosPeso.Text == string.Empty ? 0 : decimal.Parse(txtDatosPeso.Text.ToString());
                    _productoDto.v_Ubicacion = txtDatosUbicacion.Text;
                    _productoDto.v_Caracteristica = txtDatosCaracteristicas.Text;
                    _productoDto.v_CodProveedor = txtDatosCodProveedor.Text;
                    _productoDto.v_Descripcion2 = txtDatosDescripcion2.Text;
                    _productoDto.i_IdTipoProducto = cboDatosTipoProducto.Value == null ? -1 : int.Parse(cboDatosTipoProducto.Value.ToString());
                    _productoDto.i_EsServicio = chkDatosServicio.Checked == true ? 1 : 0;
                    _productoDto.i_EsAfectoDetraccion = chkAfectoDetraccion.Checked == true ? 1 : 0;
                    _productoDto.i_EsLote = chkDatosLote.Checked == true ? 1 : 0;
                    _productoDto.i_EsActivo = chkDatosActivo.Checked == true ? 1 : 0;
                    _productoDto.d_PrecioVenta = txtDatosPrecioVenta.Text == string.Empty ? 0 : decimal.Parse(txtDatosPrecioVenta.Text.ToString());
                    _productoDto.d_PrecioCosto = txtDatosPrecioCosto.Text == string.Empty ? 0 : decimal.Parse(txtDatosPrecioCosto.Text.ToString());
                    if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
                    {
                        ActualizarListaPrecio = true;
                    }
                    else
                    {
                        ActualizarListaPrecio = (CostoInicial != decimal.Parse(txtDatosPrecioCosto.Text.Trim().Replace(" ", "")));
                    }
                    if (ActualizarListaPrecio)
                    {
                        TipoCambio = _objProductoBl.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Now.Date);


                        if (TipoCambio == "0")
                        {
                            UltraMessageBox.Show("Por favor registre un tipo de cambio válido en el Sistema.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    _productoDto.d_StockMinimo = txtDatosStockMin.Text == string.Empty ? 0 : decimal.Parse(txtDatosStockMin.Text.ToString());
                    _productoDto.d_StockMaximo = 0;
                    _productoDto.i_NombreEditable = chkDescripcionEditable.Checked == true ? 1 : 0;
                    _productoDto.i_ValidarStock = chkNoStock.Checked == true ? 1 : 0;
                    _productoDto.i_Insumo = chkInsumo.Checked == true ? 1 : 0;
                    _productoDto.i_IdProveedor = int.Parse(cboAtributosProveedor.Value.ToString());
                    _productoDto.i_IdTipo = int.Parse(cboAtributosTipo.Value.ToString());
                    _productoDto.i_IdUsuario = int.Parse(cboAtributosUsuario.Value.ToString());
                    _productoDto.i_IdTela = int.Parse(cboAtributosTela.Value.ToString());
                    _productoDto.i_IdEtiqueta = int.Parse(cboAtributosEtiqueta.Value.ToString());
                    _productoDto.i_IdCuello = int.Parse(cboAtributosCuello.Value.ToString());
                    _productoDto.i_IdAplicacion = int.Parse(cboAtributosAplicacion.Value.ToString());
                    _productoDto.i_IdArte = int.Parse(cboAtributosMaterial.Value.ToString());
                    _productoDto.i_IdColeccion = int.Parse(cboAtributosColeccion.Value.ToString());
                    _productoDto.i_IdTemporada = cboAtributosTemporada.Value ==null ?-1:  int.Parse(cboAtributosTemporada.Value.ToString());
                    _productoDto.i_EsAfectoPercepcion = chkAfectoPercepcion.Checked == true ? 1 : 0;
                    _productoDto.d_TasaPercepcion = string.IsNullOrEmpty(txtTasaPercepcion.Text) ? 0 : decimal.Parse(txtTasaPercepcion.Text.Trim());
                    _productoDto.i_Anio = txtAtributosAño.Text.ToString() == string.Empty ? 0 : int.Parse(txtAtributosAño.Text.ToString());
                    // Save the data
                    _productoDto.i_PrecioEditable = chkPrecioEditable.Checked == true ? 1 : 0;
                    _productoDto.i_EsActivoFijo = 0;
                    _productoDto.i_EsAfectoIsc = (short)(uchAfectoIsc.Checked ? 1 : 0);
                    _productoDto.i_CantidadFabricacionMensual = string.IsNullOrEmpty(txtCantidadFabricacionMensual.Text) ? 0 : int.Parse(txtCantidadFabricacionMensual.Text.Trim());
                    _productoDto.v_NroPartidaArancelaria = txtNroPartidaArancelaria.Text.Trim();
                    _productoDto.i_IndicaFormaParteOtrosTributos = chkOtrosTributos.Checked ? 1 : 0;
                    _productoDto.v_NroParte = txtNroParte.Text.Trim();
                    _productoDto.d_Utilidad = decimal.Parse(txtUtilidad.Text);
                    _productoDto.i_IdPerfilDetraccion = cboPerfilDetraccion.Value != null ? int.Parse(cboPerfilDetraccion.Value.ToString()) : -1;
                    _productoDto.d_PrecioMayorista = string.IsNullOrEmpty(txtPrecioMayorista.Text) ? 0 : decimal.Parse(txtPrecioMayorista.Text);
                    _productoDto.i_SolicitarNroLoteIngreso = chkSolicitarNroLoteIngreso.Checked ? 1 : 0;
                    _productoDto.i_SolicitarNroSerieIngreso = chkSolicitarNroSerieIngreso.Checked ? 1 : 0;
                    _productoDto.i_SolicitaOrdenProduccionIngreso = chkSolicitarNroOrdenProduccionIngreso.Checked ? 1 : 0;
                    _productoDto.i_SolicitarNroLoteSalida = chkSolicitarNroLoteSalida.Checked ? 1 : 0;
                    _productoDto.i_SolicitarNroSerieSalida = chkSolicitarNroSerieSalida.Checked ? 1 : 0;
                    _productoDto.i_SolicitaOrdenProduccionSalida = chkSolicitarNroOrdenProduccionSalida.Checked ? 1 : 0;
                    _objProductoBl.ActualizarProducto(ref objOperationResult, _productoDto, _productodetalleDto, Globals.ClientSession.GetAsList(), ActualizarListaPrecio, decimal.Parse(TipoCambio));
                    GuardarIsc();
                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    //LimpiarDatos();
                    //btnBuscar_Click(sender, e);
                    //MantenerSeleccion(_idProducto);
                    btnEliminarProducto.Enabled = false;
                    //btnNuevoProducto.Enabled = false;
                    TbsProducto.Tabs["tabAtributos"].Enabled = true;
                    TbsProducto.Tabs["tabReceta"].Visible = cboDatosTipoProducto.Text.ToUpper().Contains("TERMINADO");
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnBuscar_Click(this, new EventArgs());
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }
            }
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_IdProducto"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void LimpiarDatos()
        {
            txtDatosCodigo.Text = string.Empty;
            cboDatosLinea.Value = "-1";
            cboDatosMarca.Value = "-1";
            txtDatosModelo.Clear();
            txtDatosNroModel.Clear();
            cboDatosTalla.Value = "-1";
            cboDatosColor.Value = "-1";
            txtDatosDescripcion.Text = string.Empty;
            txtDatosEmpaque.Text = string.Empty;
            cboUndMedida.Value = "-1";
            txtDatosPeso.Text = string.Empty;
            txtDatosUbicacion.Text = string.Empty;
            txtDatosCaracteristicas.Text = string.Empty;
            txtDatosCodProveedor.Text = string.Empty;
            txtDatosDescripcion2.Text = string.Empty;
            // cboDatosTipoProducto.Value = "-1";
            chkDatosServicio.Checked = false;
            chkDatosLote.Checked = false;
            chkDatosActivo.Checked = true;
            txtDatosPrecioVenta.Text = string.Empty;
            txtDatosPrecioCosto.Text = string.Empty;
            txtDatosStockMin.Text = string.Empty;
            cboAtributosProveedor.Value = "-1";
            cboAtributosTipo.Value = "-1";
            cboAtributosUsuario.Value = "-1";
            cboAtributosTela.Value = "-1";
            cboAtributosEtiqueta.Value = "-1";
            cboAtributosCuello.Value = "-1";
            cboAtributosAplicacion.Value = "-1";
            cboAtributosMaterial.Value = "-1";
            cboAtributosColeccion.Value = "-1";
            txtAtributosAño.Value = Globals.ClientSession.i_Periodo.Value;
            chkDatosActivo.Checked = true;
            cboAtributosTemporada.Value = "-1";
            chkNoStock.Checked = false;
            chkInsumo.Checked = false;
            chkAfectoPercepcion.Checked = false;
            txtTasaPercepcion.Enabled = false;
            txtTasaPercepcion.Clear();
            txtCantidadFabricacionMensual.Text = @"0";
            txtUtilidad.Text = @"0";
            txtDatosPrecioVenta.Text = @"0";
            txtDatosPrecioCosto.Text = @"0";
            cboTipoTributo.Value = "-1";
            txtDatosEmpaque.Text = @"1";
            uchAfectoIsc.Checked = false;
            chkOtrosTributos.Checked = false;
            chkSolicitarNroSerieIngreso.Checked = false;
            chkSolicitarNroLoteIngreso.Checked = false;
            chkSolicitarNroOrdenProduccionIngreso.Checked = false;
            cboDatosTipoProducto.Value = Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? ((int)TipoExistencia.Mercaderia).ToString() : "-1";
        }

        private void txtNumberDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
            // Utils.Windows.NumeroDecimalUltraTextBox(txtDatosPrecioCosto, e);

        }
        private void txtNumberWhole_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }
        #endregion

        #region Edicion 2
        private void btnAtributosGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            _productoDto = _objProductoBl.ObtenerProducto(ref objOperationResult, _idProducto);
            _productodetalleDto = _objProductoBl.ObtenerProductoDetalle(ref objOperationResult, _idProductoDetalle);

            _productoDto.i_IdProveedor = int.Parse(cboAtributosProveedor.Value.ToString());

            _productoDto.i_IdTipo = int.Parse(cboAtributosTipo.Value.ToString());
            _productoDto.i_IdUsuario = int.Parse(cboAtributosUsuario.Value.ToString());
            _productoDto.i_IdTela = int.Parse(cboAtributosTela.Value.ToString());
            _productoDto.i_IdEtiqueta = int.Parse(cboAtributosEtiqueta.Value.ToString());
            _productoDto.i_IdCuello = int.Parse(cboAtributosCuello.Value.ToString());
            _productoDto.i_IdAplicacion = int.Parse(cboAtributosAplicacion.Value.ToString());
            _productoDto.i_IdArte = int.Parse(cboAtributosMaterial.Value.ToString());
            _productoDto.i_IdColeccion = int.Parse(cboAtributosColeccion.Value.ToString());
            _productoDto.i_IdTemporada = int.Parse(cboAtributosTemporada.Value.ToString());
            _productoDto.i_Anio = txtAtributosAño.Text == string.Empty ? 0 : int.Parse(txtAtributosAño.Text.Trim());
            _productoDto.v_NroOrdenProduccion = txtNroOrdenProduccion.Text.Trim();
            _productoDto.i_EsActivoFijo = 0;
            _productoDto.i_IdTipoTributo = int.Parse(cboTipoTributo.Value.ToString());
            // Save the data
            _objProductoBl.ActualizarProducto(ref objOperationResult, _productoDto, _productodetalleDto, Globals.ClientSession.GetAsList(), false, 0);

            //// Analizar el resultado de la operación
            if (objOperationResult.Success == 1)  // Operación sin error
            {
                //LimpiarDatos();
                //btnBuscar_Click(sender, e);
                //btnEliminarProducto.Enabled = false;
                //btnNuevoProducto.Enabled = false;
                //TbsProducto.Controls["tabAtributos"].Enabled = true;
                UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else  // Operación con error
            {
                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Se queda en el formulario.
            }
        }

        #endregion

        #region Linea
        private void btnLineaBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            var filters = new Queue<string>();
            if (!string.IsNullOrEmpty(txtLineaCodigo.Text)) filters.Enqueue("v_CodLinea==" + "\"" + txtLineaCodigo.Text + "\"");
            if (!string.IsNullOrEmpty(txtLineaNombre.Text)) filters.Enqueue("v_Nombre.Contains(\"" + txtLineaNombre.Text.Trim().ToUpper() + "\")");

            // Create the Filter Expression
            _strFilterExpression = filters.Count > 0 ? String.Join(" && ", filters) : null;
            BindGridLinea();
        }

        private void btnLineaAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCtaConsumoVenta.Text) ||
                string.IsNullOrWhiteSpace(txtCtaConsumoCompra.Text))
                ultraTabControl1.Tabs[1].Selected = true;

            if (uvLinea.Validate(true, false).IsValid)
            {
                var Code = txtLineaCodigo.Text.Trim();

                if (string.IsNullOrWhiteSpace(Code) || Code.Length < 2)
                {
                    UltraMessageBox.Show("Por favor ingrese el Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtLineaNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtCtaConsumoCompra.Text))
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente la cuenta para el Rubro de Compra", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCtaConsumoCompra.Focus();
                    return;

                }
                else
                {
                    if (!Utils.Windows.EsCuentaImputable(txtCtaConsumoCompra.Text))
                    {

                        UltraMessageBox.Show("La cuenta ingresada no es imputable o no existe!", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCtaConsumoCompra.Focus();
                        return;

                    }
                }
                if (string.IsNullOrEmpty(txtCtaConsumoVenta.Text))
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente la cuenta para el Rubro de Venta", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCtaConsumoVenta.Focus();
                    return;
                }
                else
                {
                    if (!Utils.Windows.EsCuentaImputable(txtCtaConsumoVenta.Text))
                    {

                        UltraMessageBox.Show("La cuenta ingresada no es imputable o no existe!", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCtaConsumoVenta.Focus();
                        return;

                    }
                }



                if (_modeDetalleLinea == "New" && grdDataLinea.Rows.ToList().Any(r => (r.Cells["v_CodLinea"].Value.ToString() == Code)))
                {
                    UltraMessageBox.Show("Este código ya esta registrado.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_modeDetalleLinea == "New")
                    _lineaDto = new lineaDto();

                var isHeader = (txtLineaCodigo.Text.EndsWith("00"));
                _lineaDto.v_CodLinea = txtLineaCodigo.Text;
                _lineaDto.v_Nombre = txtLineaNombre.Text;
                _lineaDto.v_NroCuentaCompra = txtCtaConsumoCompra.Text;
                _lineaDto.v_NroCuentaVenta = txtCtaConsumoVenta.Text;
                _lineaDto.v_NroCuentaDConsumo = txtCtaConsumoDeudora.Text;
                _lineaDto.v_NroCuentaHConsumo = txtCtaConsumoAcreedora.Text;
                _lineaDto.b_Foto = this.imageLinea;
                _lineaDto.i_Header = (txtLineaCodigo.Text.EndsWith("00")) ? 1 : 0;

                var objOperationResult = new OperationResult();

                switch (_modeDetalleLinea)
                {
                    case "New":
                        _objLineaBl.InsertarLinea(ref objOperationResult, _lineaDto, Globals.ClientSession.GetAsList());
                        break;
                    case "Edit":
                        _objLineaBl.ActualizarLinea(ref objOperationResult, _lineaDto, Globals.ClientSession.GetAsList());
                        break;
                }

                if (objOperationResult.Success == 1)
                {

                    CargarGrillaDetalleLinea();
                    btnLineaEditar.Enabled = false;
                    btnLineaEliminar.Enabled = false;
                    CargarCombos();
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema");
                }
                else
                {
                    UltraMessageBox.Show(
                        objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                        objOperationResult.AdditionalInformation, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                this.SetNewLinea();
            }
        }

        private void SetNewLinea()
        {

            txtLineaCodigo.Text = string.Empty;
            txtLineaNombre.Text = string.Empty;
            txtCtaConsumoAcreedora.Clear();
            txtCtaConsumoCompra.Clear();
            txtCtaConsumoVenta.Clear();
            txtCtaConsumoDeudora.Clear();
            this.pictureLinea.Image = null;
            this.imageLinea = null;
            _modeDetalleLinea = "New";
        }

        private void btnLineaEditar_Click(object sender, EventArgs e)
        {

            if (grdDataLinea.Selected.Rows.Count > 0)
            {
                string pstrIdLinea = grdDataLinea.Selected.Rows[0].Cells["v_IdLinea"].Value.ToString();

                CargarDetalleLinea(pstrIdLinea);
                CargarCombos();
            }
        }

        void CargarDetalleLinea(string pstrIdLinea)
        {
            OperationResult objOperationResult = new OperationResult();
            _lineaDto = new lineaDto();

            _lineaDto = _objLineaBl.ObtenerLinea(ref objOperationResult, pstrIdLinea);
            txtLineaCodigo.Text = _lineaDto.v_CodLinea;
            txtLineaNombre.Text = _lineaDto.v_Nombre;
            txtCtaConsumoAcreedora.Text = _lineaDto.v_NroCuentaHConsumo;
            txtCtaConsumoDeudora.Text = _lineaDto.v_NroCuentaDConsumo;
            txtCtaConsumoCompra.Text = _lineaDto.v_NroCuentaCompra;
            txtCtaConsumoVenta.Text = _lineaDto.v_NroCuentaVenta;
            pictureLinea.Image = (_lineaDto.b_Foto != null) ? Utils.Windows.BinaryToImage(_lineaDto.b_Foto) : null;
            imageLinea = _lineaDto.b_Foto;
            _modeDetalleLinea = "Edit";
        }

        private void btnLineaEliminar_Click(object sender, EventArgs e)
        {
            // Obtener los IDs de la fila seleccionada
            if (grdDataLinea.ActiveRow != null)
            {
                var lineasItems = new List<lineaDto>();
                lineaDto ObjLinea = (grdDataLinea.ActiveRow.ListObject as lineaDto);
                var isHeader = (ObjLinea.i_Header.HasValue && ObjLinea.i_Header == 1);
                if (isHeader)
                {
                    int intCodigo = int.Parse(ObjLinea.v_CodLinea);
                    var items = (from n in grdDataLinea.Rows
                                 let cod = int.Parse(n.Cells["v_CodLinea"].Value.ToString())
                                 where (cod - cod % 100) == intCodigo
                                 select (n.ListObject as lineaDto));
                    lineasItems.AddRange(items.ToList());
                }
                else
                    lineasItems.Add((grdDataLinea.ActiveRow.ListObject as lineaDto));

                OperationResult objOperationResult = new OperationResult();
                foreach (var linea in lineasItems)
                {
                    if (_objProductoBl.ExistAnyProduct("v_IdLinea=\"" + linea.v_IdLinea + "\""))
                    {
                        UltraMessageBox.Show("La Linea " + linea.v_Nombre + "esta siendo utilizada , no se puede eliminar.", "Info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                var message = (isHeader) ? string.Concat("Esta acción eliminara los hijos de ", ObjLinea.v_Nombre, ".\n¿Desea Continuar?") :
                                           "¿Está seguro de eliminar este registro?";

                if (UltraMessageBox.Show(message, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (var item in lineasItems)
                    {
                        _objLineaBl.EliminarLinea(ref objOperationResult, item.v_IdLinea, Globals.ClientSession.GetAsList());
                        if (objOperationResult.Success == 0) break;
                    }
                    if (objOperationResult.Success == 1)
                    {
                        CargarGrillaDetalleLinea();
                        btnLineaEditar.Enabled = false;
                        btnLineaEliminar.Enabled = false;
                        CargarCombos();
                        SetNewLinea();
                    }
                    else
                        UltraMessageBox.Show(objOperationResult.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //if (_objProductoBL.ExistAnyProduct("v_IdLinea=\"" + pstrIdLinea + "\""))
                //{
                //    UltraMessageBox.Show("Esta linea esta siendo utilizada , no se puede eliminar.", "Info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //if (Result == System.Windows.Forms.DialogResult.Yes)
                //{
                //    // Delete the item              
                //    _objLineaBL.EliminarLinea(ref objOperationResult, pstrIdLinea, Globals.ClientSession.GetAsList());


                //    CargarGrillaDetalleLinea();
                //    btnLineaEditar.Enabled = false;
                //    btnLineaEliminar.Enabled = false;

                //    CargarCombos();

                //}
            }
        }

        private void grdDataLinea_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                if (row != null)
                {
                    grdDataLinea.Rows[row.Index].Selected = true;

                    btnLineaEditar.Enabled = _btnEditarLinea;
                    btnLineaEliminar.Enabled = _btnEliminarLinea;
                }
                else
                {
                    btnLineaEditar.Enabled = false;
                    btnLineaEliminar.Enabled = false;
                }
            }
        }

        private void BindGridLinea()
        {
            var objData = GetDataLinea("v_Nombre ASC", _strFilterExpression);

            grdDataLinea.DataSource = objData;
            lblContadorFilasLinea.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<lineaDto> GetDataLinea(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objLineaBl.ListarLinea(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        void CargarGrillaDetalleLinea()
        {
            grdDataLinea.DataSource = GetDataLinea("", "");
        }

        private void pictureLinea_DoubleClick(object sender, EventArgs e)
        {
            var arr = OpenImage(ref pictureLinea);
            if (arr != null)
                this.imageLinea = arr;
        }

        private void btnDelImageLine_Click(object sender, EventArgs e)
        {
            this.pictureLinea.Image = null;
            this.imageLinea = null;
        }

        private void grdDataLinea_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var ValueHeader = e.Row.Cells["i_Header"].Value;
            if (ValueHeader != null)
            {
                if (ValueHeader.ToString() == "1")
                {
                    e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Row.Appearance.BackColor = Color.Bisque;
                    e.Row.Appearance.BackColor2 = Color.Bisque;
                }
                else if (ValueHeader.ToString() == "0")
                {
                    e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    e.Row.Appearance.BackColor = Color.White;
                }
            }
        }

        private void grdDataLinea_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            this.InvokeOnClick(btnLineaEditar, new EventArgs());
        }
        #endregion

        #region Marca

        private void btnMarcaBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtMarcaCodigo.Text)) Filters.Add("v_CodMarca==" + "\"" + txtMarcaCodigo.Text + "\"");
            if (!string.IsNullOrEmpty(txtMarcaNombre.Text)) Filters.Add("v_Nombre.Contains(\"" + txtMarcaNombre.Text.Trim().ToUpper() + "\")");

            // Create the Filter Expression
            _strFilterExpression = null;
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            this.BindGridMarca();
        }

        private void btnMarcaAgregar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvMarca.Validate(true, false).IsValid)
            {



                if (txtMarcaCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtMarcaNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (_modeDetalleMarca == "New")
                {

                    if (_objMarcaBl.ObtenerMarcasPorNombre(ref objOperationResult, txtMarcaNombre.Text.Trim(), txtMarcaCodigo.Text.Trim()) != null)
                    {
                        UltraMessageBox.Show("Este Marca ya fue agregada con este nombre", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }

                    if (_objMarcaBl.ObtenerMarcasPorCodigo(ref objOperationResult, txtMarcaNombre.Text.Trim(), txtMarcaCodigo.Text.Trim()) != null)
                    {
                        UltraMessageBox.Show("Este Marca ya fue agregada con este código", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }
                    _marcaDto = new marcaDto();
                    _marcaDto.v_CodMarca = txtMarcaCodigo.Text;
                    _marcaDto.v_Nombre = txtMarcaNombre.Text;

                    // Save the data
                    _objMarcaBl.InsertarMarca(ref objOperationResult, _marcaDto, Globals.ClientSession.GetAsList());

                }
                else if (_modeDetalleMarca == "Edit")
                {


                    if (_objMarcaBl.ObtenerMarcasPorNombreEditado(ref objOperationResult, txtMarcaNombre.Text.Trim(), txtMarcaCodigo.Text.Trim(), _marcaDto.v_IdMarca) != null)
                    {
                        UltraMessageBox.Show("Este Marca ya fue agregada con este nombre", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }

                    if (_objMarcaBl.ObtenerMarcasPorCodigoEditado(ref objOperationResult, txtMarcaNombre.Text.Trim(), txtMarcaCodigo.Text.Trim(), _marcaDto.v_IdMarca) != null)
                    {
                        UltraMessageBox.Show("Este Marca ya fue agregada con este código", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }

                    _marcaDto.v_CodMarca = txtMarcaCodigo.Text;
                    _marcaDto.v_Nombre = txtMarcaNombre.Text;
                    // Save the data
                    _objMarcaBl.ActualizarMarca(ref objOperationResult, _marcaDto, Globals.ClientSession.GetAsList());

                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {

                    CargarGrillaDetalleMarca();
                    btnMarcaEditar.Enabled = false;
                    btnMarcaEliminar.Enabled = false;
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }
                txtMarcaCodigo.Text = string.Empty;
                txtMarcaNombre.Text = string.Empty;
                _modeDetalleMarca = "New";
            }
        }

        private void btnMarcaEditar_Click(object sender, EventArgs e)
        {
            if (grdDataMarca.Selected.Rows.Count > 0)
            {
                string pstrIdMarca = grdDataMarca.Selected.Rows[0].Cells["v_IdMarca"].Value.ToString();
                CargarDetalleMarca(pstrIdMarca);
            }
        }

        private void btnMarcaEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada
            if (grdDataMarca.Selected.Rows.Count > 0)
            {
                string pstrIdMarca = grdDataMarca.Selected.Rows[0].Cells["v_IdMarca"].Value.ToString();
                if (_objProductoBl.ExistAnyProduct("v_IdMarca=\"" + pstrIdMarca + "\""))
                {
                    UltraMessageBox.Show("Esta Marca esta siendo utilizada , no se puede eliminar.", "Info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    // Delete the item              
                    _objMarcaBl.EliminarMarca(ref objOperationResult, pstrIdMarca, Globals.ClientSession.GetAsList());

                    CargarGrillaDetalleMarca();
                    btnMarcaEditar.Enabled = false;
                    btnMarcaEliminar.Enabled = false;
                }
            }
        }

        private void grdDataMarca_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                if (row != null)
                {
                    grdDataMarca.Rows[row.Index].Selected = true;

                    btnMarcaEditar.Enabled = _btnEditarMarca;
                    btnMarcaEliminar.Enabled = _btnEliminarMarca;
                }
                else
                {
                    btnMarcaEditar.Enabled = false;
                    btnMarcaEliminar.Enabled = false;
                }
            }
        }

        void CargarDetalleMarca(string pstrIdMarca)
        {
            OperationResult objOperationResult = new OperationResult();
            _marcaDto = new marcaDto();

            _marcaDto = _objMarcaBl.ObtenerMarca(ref objOperationResult, pstrIdMarca);
            txtMarcaCodigo.Text = _marcaDto.v_CodMarca;
            txtMarcaNombre.Text = _marcaDto.v_Nombre;

            _modeDetalleMarca = "Edit";
        }

        private void BindGridMarca()
        {
            var objData = GetDataMarca("v_Nombre ASC", _strFilterExpression);

            grdDataMarca.DataSource = objData;
            lblContadorFilasMarca.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<marcaDto> GetDataMarca(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objMarcaBl.ListarMarca(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        void CargarGrillaDetalleMarca()
        {
            grdDataMarca.DataSource = GetDataMarca("", "");
        }


        #endregion

        #region Modelo

        private void btnModeloBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            List<string> Filters = new List<string>();
            if (cboModeloMarca.SelectedValue.ToString() != "-1") Filters.Add("v_IdMarca==" + "\"" + cboModeloMarca.SelectedValue.ToString() + "\"");
            if (!string.IsNullOrEmpty(txtModeloCodigo.Text)) Filters.Add("v_CodMarca==" + "\"" + txtModeloCodigo.Text + "\"");
            if (!string.IsNullOrEmpty(txtModeloNombre.Text)) Filters.Add("v_Nombre.Contains(\"" + txtModeloNombre.Text.Trim().ToUpper() + "\")");

            // Create the Filter Expression
            _strFilterExpression = null;
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            this.BindGridModelo();
        }

        private void btnModeloAgregar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvModelo.Validate(true, false).IsValid)
            {
                if (txtModeloCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtModeloNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (_modeDetalleModelo == "New")
                {
                    _modeloDto = new modeloDto();
                    _modeloDto.v_IdMarca = cboModeloMarca.SelectedValue.ToString();
                    _modeloDto.v_CodModelo = txtModeloCodigo.Text;
                    _modeloDto.v_Nombre = txtModeloNombre.Text;

                    // Save the data
                    _objModeloBl.InsertarModelo(ref objOperationResult, _modeloDto, Globals.ClientSession.GetAsList());

                }
                else if (_modeDetalleModelo == "Edit")
                {
                    _modeloDto.v_IdMarca = cboModeloMarca.SelectedValue.ToString();
                    _modeloDto.v_CodModelo = txtModeloCodigo.Text;
                    _modeloDto.v_Nombre = txtModeloNombre.Text;
                    // Save the data
                    _objModeloBl.ActualizarModelo(ref objOperationResult, _modeloDto, Globals.ClientSession.GetAsList());

                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {

                    CargarGrillaDetalleModelo();
                    btnModeloEditar.Enabled = false;
                    btnModeloEliminar.Enabled = false;
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }

                cboModeloLinea.SelectedValue = "-1";
                cboModeloMarca.SelectedValue = "-1";
                txtModeloCodigo.Text = string.Empty;
                txtModeloNombre.Text = string.Empty;
                _modeDetalleModelo = "New";
            }
        }

        private void btnModeloEditar_Click(object sender, EventArgs e)
        {
            if (grdDataModelo.Selected.Rows.Count > 0)
            {
                string pstrIdMarca = grdDataModelo.Selected.Rows[0].Cells["v_IdMarca"].Value.ToString();
                string pstrIdModelo = grdDataModelo.Selected.Rows[0].Cells["v_IdModelo"].Value.ToString();
                CargarDetalleModelo(pstrIdModelo, pstrIdMarca);
            }
        }

        private void btnModeloEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada
            if (grdDataModelo.Selected.Rows.Count > 0)
            {
                string pstrIdModelo = grdDataModelo.Selected.Rows[0].Cells["v_IdModelo"].Value.ToString();
                if (_objProductoBl.ExistAnyProduct("v_IdModelo=\"" + pstrIdModelo + "\""))
                {
                    UltraMessageBox.Show("Este Modelo esta siendo utilizado , no se puede eliminar.", "Info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    // Delete the item              
                    _objModeloBl.EliminarModelo(ref objOperationResult, pstrIdModelo, Globals.ClientSession.GetAsList());

                    CargarGrillaDetalleModelo();
                    btnModeloEditar.Enabled = false;
                    btnModeloEliminar.Enabled = false;
                }
            }
        }

        private void grdDataModelo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                if (row != null)
                {
                    grdDataModelo.Rows[row.Index].Selected = true;

                    btnModeloEditar.Enabled = _btnEditarModelo;
                    btnModeloEliminar.Enabled = _btnEliminarModelo;
                }
                else
                {
                    btnModeloEditar.Enabled = false;
                    btnModeloEliminar.Enabled = false;
                }
            }
        }

        private void cboModeloLinea_SelectedValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (cboModeloLinea.SelectedValue == null) return;
            Utils.Windows.LoadDropDownList(cboModeloMarca, "Value1", "Id", _objMarcaBl.LlenarComboMarca(ref objOperationResult, "v_CodMarca", cboModeloLinea.SelectedValue.ToString()), DropDownListAction.Select);

        }

        void CargarDetalleModelo(string pstrIdModelo, string pstrIdMarca)
        {
            OperationResult objOperationResult = new OperationResult();
            _modeloDto = new modeloDto();

            _modeloDto = _objModeloBl.ObtenerModelo(ref objOperationResult, pstrIdMarca, pstrIdModelo);
            cboModeloLinea.SelectedValue = _modeloDto.v_IdLinea.ToString();
            cboModeloMarca.SelectedValue = _modeloDto.v_IdMarca.ToString();
            txtModeloCodigo.Text = _modeloDto.v_CodModelo;
            txtModeloNombre.Text = _modeloDto.v_Nombre;

            _modeDetalleModelo = "Edit";
        }

        private void BindGridModelo()
        {
            var objData = GetDataModelo("v_Nombre ASC", _strFilterExpression);

            grdDataModelo.DataSource = objData;
            lblContadorFilasModelo.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<modeloDto> GetDataModelo(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objModeloBl.ListarModelo(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        void CargarGrillaDetalleModelo()
        {
            grdDataModelo.DataSource = GetDataModelo("", "");
        }

        #endregion

        #region Talla
        private void btnTallaBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            var filters = new Queue<string>();
            if (!string.IsNullOrEmpty(txtTallaCodigo.Text)) filters.Enqueue("v_CodTalla==" + "\"" + txtTallaCodigo.Text + "\"");
            if (!string.IsNullOrEmpty(txtTallaNombre.Text)) filters.Enqueue("v_Nombre.Contains(\"" + txtTallaNombre.Text.Trim().ToUpper() + "\")");

            // Create the Filter Expression
            _strFilterExpression = filters.Any() ? _strFilterExpression = string.Join(" && ", filters)
            : null;

            BindGridTalla();
        }

        private void btnTallaAgregar_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (uvTalla.Validate(true, false).IsValid)
            {
                if (txtTallaCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtTallaNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (_modeDetalleTalla == "New")
                {
                    _tallaDto = new tallaDto();
                    _tallaDto.v_CodTalla = txtTallaCodigo.Text;
                    _tallaDto.v_Nombre = txtTallaNombre.Text;

                    // Save the data
                    _objTallaBl.InsertarTalla(ref objOperationResult, _tallaDto, Globals.ClientSession.GetAsList());

                }
                else if (_modeDetalleTalla == "Edit")
                {
                    _tallaDto.v_CodTalla = txtTallaCodigo.Text;
                    _tallaDto.v_Nombre = txtTallaNombre.Text;
                    // Save the data
                    _objTallaBl.ActualizarTalla(ref objOperationResult, _tallaDto, Globals.ClientSession.GetAsList());

                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {

                    CargarGrillaDetalleTalla();
                    btnTallaEditar.Enabled = false;
                    btnTallaEliminar.Enabled = false;
                    CargarCombos();
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }

                txtTallaCodigo.Text = string.Empty;
                txtTallaNombre.Text = string.Empty;
                _modeDetalleTalla = "New";
            }
        }

        private void btnTallaEditar_Click(object sender, EventArgs e)
        {
            if (grdDataTalla.Selected.Rows.Count > 0)
            {
                string pstrIdTalla = grdDataTalla.Selected.Rows[0].Cells["v_IdTalla"].Value.ToString();

                CargarDetalleTalla(pstrIdTalla);
            }
        }

        void CargarDetalleTalla(string pstrIdTalla)
        {
            OperationResult objOperationResult = new OperationResult();
            _tallaDto = new tallaDto();

            _tallaDto = _objTallaBl.ObtenerTalla(ref objOperationResult, pstrIdTalla);
            txtTallaCodigo.Text = _tallaDto.v_CodTalla;
            txtTallaNombre.Text = _tallaDto.v_Nombre;

            _modeDetalleTalla = "Edit";
        }

        private void btnTallaEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada
            if (grdDataTalla.Selected.Rows.Count > 0)
            {
                string pstrIdTalla = grdDataTalla.Selected.Rows[0].Cells["v_IdTalla"].Value.ToString();
                if (_objProductoBl.ExistAnyProduct("v_IdTalla=\"" + pstrIdTalla + "\""))
                {
                    UltraMessageBox.Show("Esta Talla esta siendo utilizada , no se puede eliminar.", "Info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    // Delete the item              

                    _objTallaBl.EliminarTalla(ref objOperationResult, pstrIdTalla, Globals.ClientSession.GetAsList());


                    CargarGrillaDetalleTalla();
                    btnTallaEditar.Enabled = false;
                    btnTallaEliminar.Enabled = false;
                    CargarCombos();

                }
            }
        }

        private void grdDataTalla_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var point = new Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                var row = (UltraGridRow)uiElement.GetContext(typeof(UltraGridRow));

                if (row != null)
                {
                    grdDataTalla.Rows[row.Index].Selected = true;

                    btnTallaEditar.Enabled = _btnEditarTalla;
                    btnTallaEliminar.Enabled = _btnEliminarTalla;
                }
                else
                {
                    btnTallaEditar.Enabled = false;
                    btnTallaEliminar.Enabled = false;
                }
            }
        }

        private void BindGridTalla()
        {
            var objData = GetDataTalla("v_Nombre ASC", _strFilterExpression);

            grdDataTalla.DataSource = objData;
            lblContadorFilasTalla.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<tallaDto> GetDataTalla(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objTallaBl.ListarTalla(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        void CargarGrillaDetalleTalla()
        {
            grdDataTalla.DataSource = GetDataTalla("", "");
        }

        #endregion

        #region Color

        private void btnColorBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            var filters = new Queue<string>();
            if (!string.IsNullOrEmpty(txtColorCodigo.Text)) filters.Enqueue("v_CodColor==" + "\"" + txtColorCodigo.Text + "\"");
            if (!string.IsNullOrEmpty(txtColorNombre.Text)) filters.Enqueue("v_Nombre.Contains(\"" + txtColorNombre.Text.Trim().ToUpper() + "\")");

            // Create the Filter Expression
            _strFilterExpression = filters.Any() ? string.Join(" && ", filters) : null;

            BindGridColor();
        }

        private void btnColorAgregar_Click(object sender, EventArgs e)
        {
            if (uvColor.Validate("Color", true, false).IsValid)
            {
                if (txtColorCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtColorNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                OperationResult objOperationResult = new OperationResult();
                if (_modeDetalleColor == "New")
                {
                    _colorDto = new colorDto
                    {
                        v_CodColor = txtColorCodigo.Text,
                        v_Nombre = txtColorNombre.Text
                    };

                    // Save the data
                    _objColorBl.InsertarColor(ref objOperationResult, _colorDto, Globals.ClientSession.GetAsList());

                }
                else if (_modeDetalleColor == "Edit")
                {
                    _colorDto.v_CodColor = txtColorCodigo.Text;
                    _colorDto.v_Nombre = txtColorNombre.Text;
                    // Save the data
                    _objColorBl.ActualizarColor(ref objOperationResult, _colorDto, Globals.ClientSession.GetAsList());

                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    CargarCombos();
                    CargarGrillaDetalleColor();
                    btnColorEditar.Enabled = false;
                    btnColorEliminar.Enabled = false;
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }

                txtColorCodigo.Text = string.Empty;
                txtColorNombre.Text = string.Empty;
                _modeDetalleColor = "New";
            }
        }

        private void btnColorEditar_Click(object sender, EventArgs e)
        {
            if (grdDataColor.Selected.Rows.Count > 0)
            {
                string pstrIdColor = grdDataColor.Selected.Rows[0].Cells["v_IdColor"].Value.ToString();

                CargarDetalleColor(pstrIdColor);
            }
        }

        void CargarDetalleColor(string pstrIdColor)
        {
            OperationResult objOperationResult = new OperationResult();
            _colorDto = new colorDto();

            _colorDto = _objColorBl.ObtenerColor(ref objOperationResult, pstrIdColor);
            txtColorCodigo.Text = _colorDto.v_CodColor;
            txtColorNombre.Text = _colorDto.v_Nombre;

            _modeDetalleColor = "Edit";
        }

        private void btnColorEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada
            if (grdDataColor.Selected.Rows.Count > 0)
            {
                string pstrIdColor = grdDataColor.Selected.Rows[0].Cells["v_IdColor"].Value.ToString();
                if (_objProductoBl.ExistAnyProduct("v_IdColor=\"" + pstrIdColor + "\""))
                {
                    UltraMessageBox.Show("Este Color esta siendo utilizado , no se puede eliminar.", "Info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    // Delete the item              

                    _objColorBl.EliminarColor(ref objOperationResult, pstrIdColor, Globals.ClientSession.GetAsList());

                    CargarCombos();
                    CargarGrillaDetalleColor();
                    btnColorEditar.Enabled = false;
                    btnColorEliminar.Enabled = false;

                }
            }
        }

        private void grdDataColor_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                if (row != null)
                {
                    grdDataColor.Rows[row.Index].Selected = true;

                    btnColorEditar.Enabled = _btnEditarColor;
                    btnColorEliminar.Enabled = _btnEliminarColor;
                }
                else
                {
                    btnColorEditar.Enabled = false;
                    btnColorEliminar.Enabled = false;
                }
            }
        }

        private void BindGridColor()
        {
            var objData = GetDataColor("v_Nombre ASC", _strFilterExpression);

            grdDataColor.DataSource = objData;
            lblContadorFilasColor.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<colorDto> GetDataColor(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objColorBl.ListarColor(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        void CargarGrillaDetalleColor()
        {
            grdDataColor.DataSource = GetDataColor("", "");
        }

        #endregion

        private void chkDatosServicio_CheckedChanged(object sender, EventArgs e)
        {
            //chkDescripcionEditable.Enabled = chkDatosServicio.Checked == true ? true : false;
            //if (chkDatosServicio.Checked == false)
            //{
            //    chkDescripcionEditable.Checked = false;
            //}


        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkAfectoPercepcion_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAfectoPercepcion.Checked == true)
            {
                txtTasaPercepcion.Enabled = true;
                txtTasaPercepcion.Focus();
            }
            else
            {
                txtTasaPercepcion.Clear();
                txtTasaPercepcion.Enabled = false;
            }
        }

        private void txtTasaPercepcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox((UltraTextEditor)sender, e);
        }

        private void txtCtaConsumoDeudora_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            using (var f = new frmPlanCuentasConsulta(""))
            {
                f.ShowDialog();

                if (f._NroSubCuenta != null)
                {
                    txtCtaConsumoDeudora.Text = f._NroSubCuenta;
                }
            }
        }

        private void txtCtaConsumoAcreedora_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            using (var f = new frmPlanCuentasConsulta(""))
            {
                f.ShowDialog();

                if (f._NroSubCuenta != null)
                {
                    txtCtaConsumoAcreedora.Text = f._NroSubCuenta;
                }
            }
        }

        private void txtCtaConsumoVenta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            using (var f = new frmPlanCuentasConsulta(""))
            {
                f.ShowDialog();

                if (f._NroSubCuenta != null)
                {
                    txtCtaConsumoVenta.Text = f._NroSubCuenta;
                }
            }
        }

        private void txtCtaConsumoCompra_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            using (var f = new frmPlanCuentasConsulta(""))
            {
                f.ShowDialog();

                if (f._NroSubCuenta != null)
                {
                    txtCtaConsumoCompra.Text = f._NroSubCuenta;
                }
            }
        }

        private void grdDataLinea_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            txtLineaCodigo.Text = string.Empty;
            txtLineaNombre.Text = string.Empty;
            txtCtaConsumoAcreedora.Clear();
            txtCtaConsumoCompra.Clear();
            txtCtaConsumoVenta.Clear();
            txtCtaConsumoDeudora.Clear();
            _modeDetalleLinea = "New";
        }

        private void grdDataLinea_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count == 0) return;

            if (grdData.ActiveRow != null) //Facturado
            {

                btnLineaEditar.Enabled = _btnEditarLinea;
                btnLineaEliminar.Enabled = _btnEliminarLinea;

            }
            else
            {
                btnLineaEditar.Enabled = false;
                btnLineaEliminar.Enabled = false;
            }

        }

        private void TbsProducto_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Key.Equals("tabReceta"))
                LoadRecetasForProduct(_productoDto.v_IdProductoDetalle);

            if (e.Tab.Key.Equals("tabAtributos"))
                LoadRecetasSalida(_productoDto.v_IdProductoDetalle);

        }
        #region Busqueda Keypress
        private void OnTimedEvent(Object myObject, EventArgs myEventArgs)
        {
            _myTimer.Stop();
            LabelContador(Utils.Windows.FiltrarGrillaPorColumnas(grdData, txtDescripción.Text, new List<string> { "v_CodInterno", "v_Descripcion" }));
        }
        void LabelContador(int Cantidad)
        {
            lblContadorFilas.Text = String.Format("Se encontraron {0} registros", Cantidad);
        }
        private void txtDescripción_KeyPress(object sender, KeyPressEventArgs e)
        {
            _myTimer.Stop();
            _myTimer.Start();
        }

        private void UltraCombo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Down)
                (sender as Infragistics.Win.UltraWinGrid.UltraCombo).PerformAction(Infragistics.Win.UltraWinGrid.UltraComboAction.Dropdown);
        }
        #endregion

        #region Imagen Producto
        private void btnBuscarImagen_Click(object sender, EventArgs e)
        {
            var byteArr = OpenImage(ref pictureProduct);
            if (byteArr != null)
                _productoDto.b_Foto = byteArr;
        }

        public Image CambiarTamanoImagen(Image pImagen, int pAncho, int pAlto)
        {
            //creamos un bitmap con el nuevo tamaño
            Bitmap vBitmap = new Bitmap(pAncho, pAlto);
            //creamos un graphics tomando como base el nuevo Bitmap
            using (Graphics vGraphics = Graphics.FromImage((Image)vBitmap))
            {
                //especificamos el tipo de transformación, se escoge esta para no perder calidad.
                vGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                //Se dibuja la nueva imagen
                vGraphics.DrawImage(pImagen, 0, 0, pAncho, pAlto);
            }
            //retornamos la nueva imagen
            return (Image)vBitmap;
        }

        private void btnEliminarImagen_Click(object sender, EventArgs e)
        {
            pictureProduct.Image = null;
            _productoDto.b_Foto = null;
        }

        private void pictureProduct_MouseEnter(object sender, EventArgs e)
        {
            PictureBox picture = (sender as PictureBox);
            if (picture.Image != null)
            {
                PictureExpand.Image = picture.Image;
                popupImagen.Show();
            }
            btnBuscarImagen.Visible = true;
            btnEliminarImagen.Visible = true;
            pictureProduct.MouseEnter -= pictureProduct_MouseEnter;
        }
        private void pictureProduct_MouseLeave(object sender, EventArgs e)
        {
            //popupImagen.Close();
            //btnBuscarImagen.Visible = false;
            //btnEliminarImagen.Visible = false;
        }
        /// <summary>
        /// Obtiene una imagen en forma de array y la muestra en el PictureBox
        /// </summary>
        /// <param name="p">PictureBox donde se mostrara la imagen</param>
        /// <returns>Array de Bytes que contiene la imagen o null si no se habre</returns>
        private byte[] OpenImage(ref PictureBox p)
        {
            var _f = new OpenFileDialog();
            _f.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
            _f.Multiselect = false;
            _f.Filter = "Image Files (*.jpg;*.gif;*.jpeg;*.png)|*.jpg;*.gif;*.jpeg;*.png";
            if (_f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var ImagePath = _f.FileName;
                var info = new FileInfo(ImagePath);
                if (info.Length > 20480)
                {
                    // Mayor que 20kb
                    var imageOriginal = Image.FromFile(ImagePath);
                    int ancho = (imageOriginal.Width * 100) / imageOriginal.Height;
                    if (ancho > 150) ancho = 150;
                    var imagen = CambiarTamanoImagen(Image.FromFile(ImagePath), ancho, 100);
                    p.Image = imagen;

                    System.Drawing.Imaging.ImageFormat format;
                    switch (info.Extension.ToUpper())
                    {
                        case ".PNG": format = System.Drawing.Imaging.ImageFormat.Png;
                            break;
                        case ".GIF": format = System.Drawing.Imaging.ImageFormat.Gif;
                            break;
                        default:
                            format = System.Drawing.Imaging.ImageFormat.Jpeg;
                            break;
                    }
                    using (var memory = new MemoryStream())
                    {
                        imagen.Save(memory, format);
                        var arr = memory.ToArray();
                        return arr;
                    }
                }
                else
                {
                    p.Load(ImagePath);
                    return File.ReadAllBytes(ImagePath);
                }
            }
            return null;
        }
        #endregion

        #region GenCodigo

        private void LaodEventsHormigita()
        {
            cboDatosLinea.ValueChanged += FieldsDatos_Changed;
            cboDatosMarca.ValueChanged += FieldsDatos_Changed;
            cboDatosTalla.ValueChanged += FieldsDatos_Changed;
            cboDatosColor.ValueChanged += FieldsDatos_Changed;
            txtDatosModelo.ValueChanged += FieldsDatos_Changed;
            txtDatosNroModel.ValueChanged += FieldsDatos_Changed;

            this.uvDatos.GetValidationSettings(this.txtDatosModelo).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.txtDatosModelo).IsRequired = true;
            this.uvDatos.GetValidationSettings(this.txtDatosNroModel).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.txtDatosNroModel).IsRequired = true;
        }

        private void FieldsDatos_Changed(object sender, EventArgs e)
        {
            if (_modeDatos.Equals("")) return;
            string[] codigo = new string[5];
            string[] descripcion = new string[3];

            if (cboDatosLinea.Value == null || cboDatosLinea.Value.ToString() == "-1")
            {
                codigo[0] = string.Empty;
            }
            else
            {
                codigo[0] = cboDatosLinea.ActiveRow.Cells["Value2"].Value.ToString().TrimEnd();
            }
            if (cboDatosMarca.Value == null || cboDatosMarca.Value.ToString() == "-1")
            {
                codigo[1] = string.Empty;
            }
            else
            {
                codigo[1] = cboDatosMarca.ActiveRow.Cells["Value2"].Value.ToString().TrimEnd();
            }

            if (cboDatosTalla.Value == null || cboDatosTalla.Value.ToString() == "-1")
            {
                codigo[3] = string.Empty;
                descripcion[1] = string.Empty;
            }
            else
            {
                codigo[3] = cboDatosTalla.ActiveRow.Cells["Value2"].Value.ToString().TrimEnd();
                descripcion[1] = cboDatosTalla.ActiveRow.Cells["Value1"].Value.ToString();
            }
            if (cboDatosColor.Value == null || cboDatosColor.Value.ToString() == "-1")
            {
                codigo[4] = string.Empty;
                descripcion[2] = string.Empty;
            }
            else
            {
                codigo[4] = cboDatosColor.ActiveRow.Cells["Value2"].Value.ToString().TrimEnd();
                descripcion[2] = cboDatosColor.ActiveRow.Cells["Value1"].Value.ToString();
            }
            codigo[2] = txtDatosNroModel.Text;
            descripcion[0] = txtDatosModelo.Text;
            txtDatosCodigo.Text = string.Concat(codigo);
            txtDatosDescripcion.Text = string.Join(" ", descripcion);

        }

        #endregion

        #region Receta
        private bool IsLoadReceta = false;
        private List<int> IdsUpdates = new List<int>();
        private void btnAddReceta_Click(object sender, EventArgs e)
        {
            var frm = new frmProductoReceta();
            frm.ShowDialog();
            var IdsInsumos = grdRecetas.Rows.Select(r => r.Cells["v_IdProdInsumo"].Value.ToString()).ToList();
            if (frm.Resultado != null)
            {
                var r = grdRecetas.DisplayLayout.Override.AllowAddNew;
                foreach (var item in frm.Resultado)
                {
                    var idProdInsumo = item.Cells["Id"].Value.ToString();
                    if (IdsInsumos.Contains(idProdInsumo)) continue;
                    var row = grdRecetas.DisplayLayout.Bands[0].AddNew();
                    grdRecetas.Rows.Move(row, grdRecetas.Rows.Count() - 1);
                    row.Cells["v_IdProdInsumo"].Value = idProdInsumo;
                    row.Cells["CodInternoInsumo"].Value = item.Cells["Value1"].Value;
                    row.Cells["NombreInsumo"].Value = item.Cells["Value2"].Value;
                    row.Cells["d_Cantidad"].Value = "1.00";
                    row.Cells["i_IdAlmacen"].Value = ((List<GridKeyValueDTO>)_ucAlmacen.DataSource).FirstOrDefault().Id;
                }
            }
        }

        private void btnEliminarReceta_Click(object sender, EventArgs e)
        {
            if (grdRecetas.ActiveRow != null)
            {
                var idVal = int.Parse(grdRecetas.ActiveRow.Cells["i_IdReceta"].Value.ToString());
                if (idVal == 0)
                {
                    grdRecetas.ActiveRow.Delete();
                }
                else
                    if (MessageBox.Show("¿Esta seguro de eliminar este item?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        var result = new OperationResult();
                        _objProductoBl.EliminarRecetaById(ref result, idVal, Globals.ClientSession.GetAsList());
                        if (result.Success == 0)
                            MessageBox.Show(result.ExceptionMessage, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            IsLoadReceta = false;
                            LoadRecetasForProduct(_productoDto.v_IdProductoDetalle);
                        }
                    }
            }
        }

        private void btnGuardarReceta_Click(object sender, EventArgs e)
        {
            OperationResult objResult = new OperationResult();
            bool sucess = true;
            short news = 0, updates = 0;
            foreach (var row in grdRecetas.Rows)
            {
                var itemReceta = (row.ListObject as productorecetaDto);
                objResult.Success = 1;
                if (itemReceta.i_IdReceta == 0)
                {
                    itemReceta.v_IdProdTerminado = _productoDto.v_IdProductoDetalle;
                    _objProductoBl.InsertarReceta(ref objResult, itemReceta, Globals.ClientSession.GetAsList());
                    news++;
                }
                else if (IdsUpdates.Contains(itemReceta.i_IdReceta))
                {
                    _objProductoBl.ActualizaReceta(ref objResult, itemReceta, Globals.ClientSession.GetAsList());
                    updates++;
                }
                if (objResult.Success == 0)
                {
                    UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    sucess = false;
                    break;
                }
            }
            if (sucess)
            {
                UltraMessageBox.Show(string.Concat("Se Guardaron todos los cambios!",
                                        System.Environment.NewLine,
                                        news, " Nuevos",
                                        System.Environment.NewLine,
                                        updates, " Actualizados"), "Resumen");
                IsLoadReceta = false;
                LoadRecetasForProduct(_productoDto.v_IdProductoDetalle);
            }
        }

        private void LoadRecetasForProduct(string pstrIdProductTerminado)
        {
            if (IsLoadReceta) return;
            var _objOpertationResult = new OperationResult();
            var objSource = _objProductoBl.GetRecetasByCodeProduct(ref _objOpertationResult, "", "", pstrIdProductTerminado);
            if (_objOpertationResult.Success == 0)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + _objOpertationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            grdRecetas.DataSource = objSource;
            grdRecetas.DisplayLayout.Bands[0].Columns["CodInternoInsumo"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);
            IdsUpdates.Clear();
            IsLoadReceta = true;
        }




        private void grdRecetas_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (grdRecetas.ActiveRow != null)
            {
                var productorecetaDto = grdRecetas.ActiveRow.ListObject as productorecetaDto;
                if (productorecetaDto != null)
                {
                    var id = productorecetaDto.i_IdReceta;
                    if (!IdsUpdates.Contains(id))
                        IdsUpdates.Add(id);
                }
            }
        }
        private void grdRecetas_BeforeCellActivate(object sender, Infragistics.Win.UltraWinGrid.CancelableCellEventArgs e)
        {
            if (!(e.Cell.Column.Key.Equals("d_Cantidad") || e.Cell.Column.Key.Equals("v_Observacion") || e.Cell.Column.Key.Equals("i_IdAlmacen"))) { e.Cancel = true; }
        }
        private void FormatoDecimalesGrilla(int DecimalesCantidad)
        {
            UltraGridColumn _Cantidad = this.grdRecetas.DisplayLayout.Bands[0].Columns["d_Cantidad"];
            _Cantidad.MaskDataMode = MaskMode.IncludeLiterals;
            _Cantidad.MaskDisplayMode = MaskMode.IncludeLiterals;
            _Cantidad.MaskClipMode = MaskMode.IncludeLiterals;

            string FormatoCantidad;
            if (DecimalesCantidad > 0)
            {
                string sharp = "n";
                FormatoCantidad = "nnnnnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad += sharp;
                }
            }
            else
            {
                FormatoCantidad = "nnnnnn";
            }
            _Cantidad.MaskInput = FormatoCantidad;
        }

        #endregion

        private void grdRecetas_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdAlmacen"].EditorComponent = _ucAlmacen;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void CargarCombosDetalleReceta()
        {
            OperationResult objOperationResult = new OperationResult();

            #region Configura Combo Almacén
            var ultraGridBanda = new UltraGridBand("Band 0", -1);
            var ultraGridColumnaId = new UltraGridColumn("Id");
            var ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            ultraGridColumnaId.Header.Caption = @"Cod.";
            ultraGridColumnaDescripcion.Header.Caption = @"Descripción";
            ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            ultraGridColumnaId.Width = 50;
            ultraGridColumnaDescripcion.Width = 327;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaDescripcion, ultraGridColumnaId });
            _ucAlmacen.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            _ucAlmacen.DropDownWidth = 380;
            _ucAlmacen.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion

            _ucAlmacen.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(_ucAlmacen, "Id", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaComboGrid(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento ?? 0), DropDownListAction.Select);
        }
        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }


        #region Siostema ISC
        private void LoadDataIsc()
        {
            if (_productoDto.i_EsAfectoIsc == null || _productoDto.i_EsAfectoIsc == 0)
            {
                ucTipoIsc.Tag = null;
                return;
            }
            var prodIscBl = new ProductoIscBL();
            var result = new OperationResult();
            var obj = prodIscBl.FromProduct(ref result, _productoDto.v_IdProducto, Globals.ClientSession.i_Periodo.ToString());
            if (result.Success == 0)
            {
                UltraMessageBox.Show(result.ExceptionMessage, Icono: MessageBoxIcon.Error);
                return;
            }
            if (obj != null)
            {
                ucTipoIsc.Value = obj.i_IdSistemaIsc;
                txtTasaIsc.Text = obj.i_IdSistemaIsc == 1 || obj.i_IdSistemaIsc == 3
                    ? (obj.d_Porcentaje * 100).ToString() : string.Empty;
                txtPrecioIsc.Text = (obj.i_IdSistemaIsc == 2 || obj.i_IdSistemaIsc == 3)
                    ? obj.d_Monto.ToString() : string.Empty;
            }
            else
            {
                ucTipoIsc.Value = "-1";
                txtTasaIsc.Clear();
                txtPrecioIsc.Clear();
            }
            ucTipoIsc.Tag = obj;
        }
        private void uchAfectoIsc_CheckedChanged(object sender, EventArgs e)
        {
            btnShowIsc.Visible = uchAfectoIsc.Checked;
        }
        private void ucTipoIsc_ValueChanged(object sender, EventArgs e)
        {
            var tipoIsc = (string)ucTipoIsc.Value;
            uvColor.GetValidationSettings(txtPrecioIsc).IsRequired = ulMontoIsc.Enabled = txtPrecioIsc.Enabled = tipoIsc == "2" || tipoIsc == "3";
            uvColor.GetValidationSettings(txtTasaIsc).IsRequired = ulTasaIsc.Enabled = txtTasaIsc.Enabled = tipoIsc == "1" || tipoIsc == "3";
        }
        private void btnShowIsc_Click(object sender, EventArgs e)
        {
            if (_productoDto.i_EsAfectoIsc == null || _productoDto.i_EsAfectoIsc == 0)
            {
                ucTipoIsc.Value = "-1";
                txtTasaIsc.Clear();
                txtPrecioIsc.Clear();
            }
            popupIsc.Show();
        }
        private bool GuardarIsc()
        {
            if (!uchAfectoIsc.Checked) return true;
            var isNew = ucTipoIsc.Tag == null;
            var obj = (productoiscDto)ucTipoIsc.Tag ?? new productoiscDto();
            obj.i_IdSistemaIsc = int.Parse((string)ucTipoIsc.Value);
            if (obj.i_IdSistemaIsc == 1 || obj.i_IdSistemaIsc == 3)
                obj.d_Porcentaje = decimal.Parse(txtTasaIsc.Text) / 100M;
            if (obj.i_IdSistemaIsc == 2 || obj.i_IdSistemaIsc == 3)
                obj.d_Monto = decimal.Parse(txtPrecioIsc.Text);
            obj.v_IdProducto = _productoDto.v_IdProducto;
            obj.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
            var objResult = new OperationResult();
            var prodIscBl = new ProductoIscBL();
            if (isNew)
                prodIscBl.Add(ref objResult, obj, Globals.ClientSession.GetAsList());
            else
                prodIscBl.Update(ref objResult, obj, Globals.ClientSession.GetAsList());
            if (objResult.Success == 1)
                ucTipoIsc.Tag = obj;
            else
                UltraMessageBox.Show(objResult.ExceptionMessage, "Error", Icono: MessageBoxIcon.Error);
            return objResult.Success == 1;
        }
        #endregion

        private void chkEdicionMultiple_CheckedChanged(object sender, EventArgs e)
        {
            btnDatosGuardarVarios.Visible = chkEdicionMultiple.Checked;
            btnDatosGrabar.Visible = !chkEdicionMultiple.Checked;
            ActivarEdicionMultiple(!chkEdicionMultiple.Checked);
            ultraLabel5.Visible = chkEdicionMultiple.Checked;
        }

        private void ActivarEdicionMultiple(bool estado)
        {
            txtDatosCodigo.Enabled = estado;
            txtDatosDescripcion.Enabled = estado;
            txtDatosPrecioCosto.Enabled = estado;
            txtDatosDescripcion2.Enabled = estado;
            uchAfectoIsc.Enabled = estado;
            grdData.DisplayLayout.Override.SelectTypeRow = chkEdicionMultiple.Checked ? SelectType.Extended : SelectType.Single;
            LimpiarDatos();
            if (!estado)
            {
                chkDatosServicio.ThreeState = true;
                chkDatosServicio.CheckState = CheckState.Indeterminate;
                chkDatosActivo.ThreeState = true;
                chkDatosActivo.CheckState = CheckState.Indeterminate;
                chkDatosLote.ThreeState = true;
                chkDatosLote.CheckState = CheckState.Indeterminate;
                chkPrecioEditable.ThreeState = true;
                chkPrecioEditable.CheckState = CheckState.Indeterminate;
                chkAfectoDetraccion.ThreeState = true;
                chkAfectoDetraccion.CheckState = CheckState.Indeterminate;
                chkAfectoPercepcion.ThreeState = true;
                chkAfectoPercepcion.CheckState = CheckState.Indeterminate;
                chkNoStock.ThreeState = true;
                chkNoStock.CheckState = CheckState.Indeterminate;
            }
            else
            {
                chkDatosServicio.ThreeState = false;
                chkDatosActivo.ThreeState = false;
                chkDatosLote.ThreeState = false;
                chkPrecioEditable.ThreeState = false;
                chkAfectoDetraccion.ThreeState = false;
                chkAfectoPercepcion.ThreeState = false;
                chkNoStock.ThreeState = false;
                grdData.Selected.Rows.Clear();
            }

        }

        private void btnDatosGuardarVarios_Click(object sender, EventArgs e)
        {
            #region Entidad modelo
            _productoDto.v_CodInterno = txtDatosCodigo.Text.Trim();
            _productoDto.v_IdLinea = cboDatosLinea.Value.ToString() == "-1" ? null : cboDatosLinea.Value.ToString();
            _productoDto.v_IdMarca = cboDatosMarca.Value.ToString() == "-1" ? null : cboDatosMarca.Value.ToString();
            _productoDto.v_Modelo = txtDatosModelo.Text.Trim();
            _productoDto.v_IdTalla = cboDatosTalla.Value.ToString() == "-1" ? null : cboDatosTalla.Value.ToString();
            _productoDto.v_IdColor = cboDatosColor.Value.ToString() == "-1" ? null : cboDatosColor.Value.ToString();
            _productoDto.v_Descripcion = txtDatosDescripcion.Text;
            _productoDto.d_Empaque = txtDatosEmpaque.Text == string.Empty ? 0 : decimal.Parse(txtDatosEmpaque.Text);
            _productoDto.i_IdUnidadMedida = int.Parse(cboUndMedida.Value.ToString());
            _productoDto.d_Peso = txtDatosPeso.Text == string.Empty ? 0 : decimal.Parse(txtDatosPeso.Text);
            _productoDto.v_Ubicacion = txtDatosUbicacion.Text;
            _productoDto.v_Caracteristica = txtDatosCaracteristicas.Text;
            _productoDto.v_CodProveedor = txtDatosCodProveedor.Text;
            _productoDto.v_Descripcion2 = txtDatosDescripcion2.Text;
            _productoDto.i_IdTipoProducto = int.Parse(cboDatosTipoProducto.Value.ToString());
            _productoDto.i_EsServicio = chkDatosServicio.CheckState != CheckState.Indeterminate ? (int?)(chkDatosServicio.Checked ? 1 : 0) : null;
            _productoDto.i_EsLote = chkDatosLote.CheckState != CheckState.Indeterminate ? (int?)(chkDatosLote.Checked ? 1 : 0) : null;
            _productoDto.i_EsAfectoDetraccion = chkAfectoDetraccion.CheckState != CheckState.Indeterminate ? (int?)(chkAfectoDetraccion.Checked ? 1 : 0) : null;
            _productoDto.i_EsActivo = chkDatosActivo.CheckState != CheckState.Indeterminate ? (int?)(chkDatosActivo.Checked ? 1 : 0) : null;
            _productoDto.d_PrecioVenta = txtDatosPrecioVenta.Text == string.Empty ? 0 : decimal.Parse(txtDatosPrecioVenta.Text);
            _productoDto.d_PrecioCosto = txtDatosPrecioCosto.Text == string.Empty ? 0 : decimal.Parse(txtDatosPrecioCosto.Text);
            _productoDto.d_StockMinimo = txtDatosStockMin.Text == string.Empty ? 0 : decimal.Parse(txtDatosStockMin.Text);
            _productoDto.d_StockMaximo = 0;
            _productoDto.i_NombreEditable = chkDescripcionEditable.Checked ? 1 : 0;
            _productoDto.i_ValidarStock = chkNoStock.CheckState != CheckState.Indeterminate ? (int?)(chkNoStock.Checked ? 1 : 0) : null;
            _productoDto.i_Insumo = chkInsumo.CheckState != CheckState.Indeterminate ? (int?)(chkInsumo.Checked ? 1 : 0) : null;
            _productoDto.i_IdProveedor = -1;
            _productoDto.i_IdTipo = -1;
            _productoDto.i_IdUsuario = -1;
            _productoDto.i_IdTela = -1;
            _productoDto.i_IdEtiqueta = -1;
            _productoDto.i_IdCuello = -1;
            _productoDto.i_IdAplicacion = -1;
            _productoDto.i_IdArte = -1;
            _productoDto.i_IdColeccion = -1;
            _productoDto.i_IdTemporada = -1;
            _productoDto.i_Anio = 0;
            _productoDto.i_EsAfectoPercepcion = chkAfectoPercepcion.CheckState != CheckState.Indeterminate ? (int?)(chkAfectoPercepcion.Checked ? 1 : 0) : null;
            _productoDto.d_TasaPercepcion = string.IsNullOrEmpty(txtTasaPercepcion.Text) ? 0 : decimal.Parse(txtTasaPercepcion.Text.Trim());
            _productoDto.i_PrecioEditable = chkPrecioEditable.CheckState != CheckState.Indeterminate ? (int?)(chkPrecioEditable.Checked ? 1 : 0) : null;
            _productoDto.i_EsActivoFijo = 0;
            _productoDto.i_EsAfectoIsc = (short)(uchAfectoIsc.Checked ? 1 : 0);
            _productoDto.i_CantidadFabricacionMensual = string.IsNullOrEmpty(txtCantidadFabricacionMensual.Text) ? 0 : int.Parse(txtCantidadFabricacionMensual.Text.Trim());
            _productoDto.v_NroParte = txtNroParte.Text.Trim();
            _productoDto.d_Utilidad = decimal.Parse(txtUtilidad.Text);

            #endregion

            var productIdsToModify =
                grdData.Selected.Rows.Cast<UltraGridRow>().Select(p => p.Cells["v_IdProducto"].Value.ToString()).ToList();

            if (productIdsToModify.Any())
            {
                var pobjOperationResult = new OperationResult();
                _objProductoBl.OnProcesoTerminado += _objProductoBl_OnProcesoTerminado;
                _objProductoBl.ActualizarProductosAsync(ref pobjOperationResult, _productoDto, productIdsToModify);
                btnDatosGuardarVarios.Appearance.Image = Resource.ajax_loaderMin;
                btnDatosGuardarVarios.Text = @"Guardando...";
                btnDatosGuardarVarios.Enabled = false;
            }
            LimpiarDatos();

        }

        private void _objProductoBl_OnProcesoTerminado(int result, OperationResult pobjOperationResult)
        {
            Invoke((MethodInvoker)delegate
                {
                    if (pobjOperationResult == null)
                    {
                        btnDatosGuardarVarios.Appearance.Image = Resource.disk_multiple;
                        btnDatosGuardarVarios.Text = @"&Actualizar Productos";
                        btnDatosGuardarVarios.Enabled = true;
                        chkEdicionMultiple.Checked = false;
                        MessageBox.Show(string.Format("Se actualizaron {0} artículos.", result), @"Sistema",
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                    {
                        MessageBox.Show(
                            pobjOperationResult.ErrorMessage + @"\n" + pobjOperationResult.ExceptionMessage, @"Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    btnBuscar_Click(this, new EventArgs());
                    _objProductoBl.OnProcesoTerminado -= _objProductoBl_OnProcesoTerminado;
                });
        }

        private void grdData_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            ultraLabel5.Text = string.Format("Productos seleccionados: {0}", grdData.Selected.Rows.Count);
        }

        private void txtDatosCodigo_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            var t = (Infragistics.Win.UltraWinEditors.UltraTextEditor)sender;
            t.ReadOnly = false;
            t.Enabled = true;
            t.SelectAll();
        }

        private void btnBuscarImagen_MouseLeave(object sender, EventArgs e)
        {

        }



        private void cboEstadoProducto_ValueChanged(object sender, EventArgs e)
        {
            IngresoFormulario = false;
            btnBuscar_Click(sender, e);
        }

        private void cboDatosTipoProducto_ValueChanged(object sender, EventArgs e)
        {
            //Globals.ClientSession.v_RucEmpresa = Constants.RucAndesAlimentosBebidas;
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucAndesAlimentosBebidas)
            {
                if (cboDatosTipoProducto.Text.Contains("PRIMA"))
                {
                    chkSolicitarNroLoteIngreso.Checked = true;
                    chkSolicitarNroLoteSalida.Checked = true;
                    chkSolicitarNroOrdenProduccionSalida.Checked = true;
                }
                else if (cboDatosTipoProducto.Text.Contains("TERMINADO"))
                {
                    chkSolicitarNroOrdenProduccionIngreso.Checked = true;
                }
            }
        }

        #region RecetaSalida

        private bool IsLoadRecetaSalida = false;
        private List<int> IdsUpdatesSalida = new List<int>();

        private void btnAñadirRecetaSalida_Click(object sender, EventArgs e)
        {
            var frm = new frmProductoReceta("S");
            frm.ShowDialog();
            var IdsTributos = grdRecetaSalida.Rows.Select(r => r.Cells["v_IdProductoTributo"].Value.ToString()).ToList();
            if (frm.Resultado != null)
            {
                var r = grdRecetaSalida.DisplayLayout.Override.AllowAddNew;
                foreach (var item in frm.Resultado)
                {
                    var IdProductoTributo = item.Cells["Id"].Value.ToString();
                    if (IdsTributos.Contains(IdProductoTributo)) continue;
                    UltraGridRow row = grdRecetaSalida.DisplayLayout.Bands[0].AddNew();
                    grdRecetaSalida.Rows.Move(row, grdRecetaSalida.Rows.Count() - 1);
                    this.grdRecetaSalida.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["v_IdProductoTributo"].Value = IdProductoTributo;
                    row.Cells["v_CodInterno"].Value = item.Cells["Value1"].Value;
                    row.Cells["v_Descripcion"].Value = item.Cells["Value2"].Value;

                }
            }
        }

        private void btnGuardarRecetaSalida_Click(object sender, EventArgs e)
        {
            OperationResult objResult = new OperationResult();
            bool sucess = true;
            short news = 0, updates = 0;
            foreach (var row in grdRecetaSalida.Rows)
            {
                var itemReceta = (row.ListObject as productorecetasalidaDto);
                objResult.Success = 1;
                if (itemReceta.i_IdRecetaSalida == 0)
                {
                    itemReceta.v_IdProductoExportacion = _productoDto.v_IdProductoDetalle;
                    _objProductoBl.InsertarRecetaSalida(ref objResult, itemReceta, Globals.ClientSession.GetAsList());
                    news++;
                }
                else if (IdsUpdatesSalida.Contains(itemReceta.i_IdRecetaSalida))
                {
                    _objProductoBl.ActualizaRecetaSalida(ref objResult, itemReceta, Globals.ClientSession.GetAsList());
                    updates++;
                }
                if (objResult.Success == 0)
                {
                    UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    sucess = false;
                    break;
                }
            }
            if (sucess)
            {
                UltraMessageBox.Show(string.Concat("Se Guardaron todos los cambios!",
                                        System.Environment.NewLine,
                                        news, " Nuevos",
                                        System.Environment.NewLine,
                                        updates, " Actualizados"), "Resumen");
                IsLoadRecetaSalida = false;

                LoadRecetasSalida(_productoDto.v_IdProductoDetalle);
            }
        }
        private void LoadRecetasSalida(string pstrIdProductoExportacion)
        {
            if (IsLoadRecetaSalida) return;
            var _objOpertationResult = new OperationResult();
            var objSource = _objProductoBl.ObtenerRecetaSalida(ref _objOpertationResult, "", "", pstrIdProductoExportacion);
            if (_objOpertationResult.Success == 0)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + _objOpertationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            grdRecetaSalida.DataSource = objSource;
            grdRecetaSalida.DisplayLayout.Bands[0].Columns["v_CodInterno"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);
            IdsUpdatesSalida.Clear();
            IsLoadRecetaSalida = true;
        }

        private void grdRecetaSalida_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (grdRecetaSalida.ActiveRow != null)
            {
                var productorecetasalidaDto = grdRecetaSalida.ActiveRow.ListObject as productorecetasalidaDto;
                if (productorecetasalidaDto != null)
                {
                    var id = productorecetasalidaDto.i_IdRecetaSalida;
                    if (!IdsUpdatesSalida.Contains(id))
                        IdsUpdatesSalida.Add(id);
                }
            }
        }

        #endregion

        private void btnEliminarRecetaSalida_Click(object sender, EventArgs e)
        {

            if (grdRecetaSalida.ActiveRow != null)
            {
                var idVal = int.Parse(grdRecetaSalida.ActiveRow.Cells["i_IdRecetaSalida"].Value.ToString());
                if (idVal == 0)
                {
                    grdRecetas.ActiveRow.Delete();
                }
                else
                    if (MessageBox.Show("¿Esta seguro de eliminar este item?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        var result = new OperationResult();
                        _objProductoBl.EliminarRecetaSalidaById(ref result, idVal, Globals.ClientSession.GetAsList());
                        if (result.Success == 0)
                            MessageBox.Show(result.ExceptionMessage, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            IsLoadRecetaSalida = false;
                            LoadRecetasSalida(_productoDto.v_IdProductoDetalle);

                        }
                    }
            }
        }

        private void txtUtilidad_ValueChanged(object sender, EventArgs e)
        {

            //decimal Utilidad = string.IsNullOrEmpty(txtUtilidad.Text) ? 0 : decimal.Parse(txtUtilidad.Text);
            //decimal PrecioCosto = string.IsNullOrEmpty(txtDatosPrecioCosto.Text) ? 0 : decimal.Parse(txtDatosPrecioCosto.Text);
            //txtDatosPrecioVenta.Text = Utilidad == 0 ? "0" : (PrecioCosto + ((PrecioCosto * Utilidad) / 100)).ToString();

        }

        private void txtUtilidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }

        private void txtDatosPrecioCosto_ValueChanged(object sender, EventArgs e)
        {
            //decimal Utilidad = string.IsNullOrEmpty(txtUtilidad.Text) ? 0 : decimal.Parse(txtUtilidad.Text);
            //if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1 && Utilidad != 0)
            //{
            //    txtUtilidad_ValueChanged(sender, e);
            //}
            txtDatosPrecioVenta_ValueChanged(sender, e);

        }

        private void txtDatosPrecioVenta_ValueChanged(object sender, EventArgs e)
        {
            if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
            {
                var Uti = CalcularUtilidad();
                txtUtilidad.Text = Uti.ToString();
            }
        }

        private decimal CalcularUtilidad()
        {
            decimal Utilidad = 0;
            decimal PrecioVenta = string.IsNullOrEmpty(txtDatosPrecioVenta.Text) ? 0 : decimal.Parse(txtDatosPrecioVenta.Text);
            decimal PrecioCosto = string.IsNullOrEmpty(txtDatosPrecioCosto.Text) ? 0 : decimal.Parse(txtDatosPrecioCosto.Text);
            Utilidad = PrecioVenta != 0 && PrecioCosto != 0 ? (PrecioVenta - PrecioCosto / PrecioCosto) * 100 : 0;
            return Utilidad;
            //Fila.Cells["d_Utilidad"].Value = decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0 && decimal.Parse(Fila.Cells["Costo"].Value.ToString()) != 0 ? (((decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) - decimal.Parse(Fila.Cells["Costo"].Value.ToString())) / decimal.Parse(Fila.Cells["Costo"].Value.ToString())) * 100).ToString() : Fila.Cells["d_Utilidad"].Value.ToString();

        }

        private void popupImagen_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void txtPrecioMayorista_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(sender as UltraTextEditor, e);
        }

        private void chkAfectoDetraccion_CheckedChanged(object sender, EventArgs e)
        {
            cboPerfilDetraccion.Enabled = chkAfectoDetraccion.Checked;
            if (!chkAfectoDetraccion.Checked)
                cboPerfilDetraccion.Value = "-1";
        }

        private void chkSolicitarNroSerie_CheckedValueChanged(object sender, EventArgs e)

           
        {
            if (chkSolicitarNroSerieIngreso.Checked)

                chkSolicitarNroLoteIngreso.Checked = false;
        }

        private void chkSolicitarNroLote_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSolicitarNroSerieIngreso.Checked)
                chkSolicitarNroLoteIngreso.Checked = false;
        }

        private void chkSolicitarNroSerieSalida_CheckedValueChanged(object sender, EventArgs e)
        {
            if (chkSolicitarNroSerieSalida.Checked)
                chkSolicitarNroLoteSalida.Checked = false;
        }

        private void chkSolicitarNroLoteSalida_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSolicitarNroSerieSalida.Checked)
                chkSolicitarNroLoteSalida.Checked = false;
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var banda = e.Row.Band.Index.ToString();
            var row = e.Row;
            if (banda == "0")
            {
                if (row.Band.Index.ToString() == "0")
                {
                    if (e.Row.Cells["v_CodInterno"].Value.ToString().Contains("FNC-") )
                    {
                        e.Row.Appearance.BackColor = Color.Yellow;
                        e.Row.Appearance.BackColor2 = Color.White;
                        //Y doy el efecto degradado vertical
                        e.Row.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
                    }
                }
            }
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            string dummyFileName = "LISTADO DE PRODUCTOS";
            SaveFileDialog sf = new SaveFileDialog();
            var ultraGridExcelExporter1 = new UltraGridExcelExporter();
            sf.DefaultExt = "xlsx";
            sf.Filter = "xlsx files (*.xlsx)|*.xlsx";

            sf.FileName = dummyFileName;


            if (sf.ShowDialog() == DialogResult.OK)
            {
                using (new PleaseWait(this.Location, "Exportando excel..."))
                {
                    ultraGridExcelExporter1.Export(grdData, sf.FileName);

                }
                UltraMessageBox.Show("Exportación Finalizada", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
