using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using System.Transactions;
using LoadingClass;

using SAMBHS.Tesoreria.BL;



namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRegistroImportacion : Form
    {
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        ImportacionBL _objImportacionBL = new ImportacionBL();
        DiarioBL _objDiarioBL = new DiarioBL();
        List<KeyValueDTO> _ListadoImportacion = new List<KeyValueDTO>();
        List<KeyValueDTO> _ListadoImportacionCambioFecha = new List<KeyValueDTO>();
        AgenciaTransporteBL _objAgenciaTransporteBL = new AgenciaTransporteBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        SecurityBL _obSecurityBL = new SecurityBL();
        UltraCombo ucbDocumento = new UltraCombo();
        UltraCombo ucbAlmacen = new UltraCombo();
        UltraCombo ucbUnidadMedida = new UltraCombo();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        public string strModo = "Nuevo", strIdImportacion, _Mode, _pstrIdMovimiento_Nuevo, _idImportacion, newIdImportacion;
        int _MaxV, _ActV, Scroll = 0;
        importacionDto _objImportacionDto = new importacionDto();
        importaciondetallefobDto _objImportaciondetallefobDto = new importaciondetallefobDto();
        importaciondetallegastosDto _objImportaciondetallegastosDto = new importaciondetallegastosDto();
        importaciondetalleproductoDto _objImportaciondetalleproductoDto = new importaciondetalleproductoDto();
        List<string> ListaProveedores = new List<string>();
        movimientoDto _movimientoDto = new movimientoDto();
        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
        bool bActualizarNotaIngreso = false, EliminadoGastos = false;
        public string NroFacturaG, NroPedidoG, CodigoProveedorG, IdClienteG, Modulo = "";
        public bool UtilizadoBefore = false, actualizarCorrelativoNotaIngreso = false;
        List<importaciondetallegastosDto> _ListaDetalleGastosDto_Grilla = new List<importaciondetallegastosDto>();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        decimal TotalSoles = 0, TotalDolares = 0;
        bool _btnGuardar, _btnImprimir;
        bool ImportacionGuardada = false;

        #region Temporales DetallesFob
        List<importaciondetallefobDto> _TempDetalleFobDto_Agregar = new List<importaciondetallefobDto>();
        List<importaciondetallefobDto> _TempDetalleFobDto_Modificar = new List<importaciondetallefobDto>();
        List<importaciondetallefobDto> _TempDetalleFobDto_Eliminar = new List<importaciondetallefobDto>();

        #endregion

        #region Temporales DetallesProdcutos

        List<importaciondetalleproductoDto> _TempDetalleProdDto_Agregar = new List<importaciondetalleproductoDto>();
        List<importaciondetalleproductoDto> _TempDetalleProdDto_Modificar = new List<importaciondetalleproductoDto>();
        List<importaciondetalleproductoDto> _TempDetalleProdDto_Eliminar = new List<importaciondetalleproductoDto>();

        #endregion

        #region Temporales DetallesGastos

        List<importaciondetallegastosDto> _TempDetalleGastosDto_Agregar = new List<importaciondetallegastosDto>();
        List<importaciondetallegastosDto> _TempDetalleGastosDto_Modificar = new List<importaciondetallegastosDto>();
        List<importaciondetallegastosDto> _TempDetalleGastosDto_Eliminar = new List<importaciondetallegastosDto>();

        #endregion

        #region Temporales MovimientoDetalle
        List<movimientodetalleDto> _TempMovimientodetalle_Agregar = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempMovimientodetalle_Modificar = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempMovimientodetalle_Eliminar = new List<movimientodetalleDto>();

        #endregion

        public frmRegistroImportacion(string Modo, string pstrIdImportacion, string pstrModulo)
        {
            InitializeComponent();
            strModo = Modo;
            strIdImportacion = pstrIdImportacion;
            Modulo = pstrModulo;
        }
        private void frmRegistroImportacion_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString();
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
            #region ControlAcciones
            if (_objCierreMensualBL.VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(), DateTime.Now.Month.ToString("00").Trim(), (int)ModulosSistema.Compras))
            {

                btnGuardar.Visible = false;
                this.Text = "Registro de  Importación [MES CERRADO]";
            }
            else
            {
                btnGuardar.Visible = true;
                this.Text = "Registro de  Importación";
            };

            if (Modulo == "C") // Modulo Contable
            {
                btnGuardar.Enabled = true;
            }
            else
            {
                var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmRegistroImportacion", Globals.ClientSession.i_RoleId);
                _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroImportacion_Save", _formActions);
                _btnImprimir = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroImportacion_Print", _formActions);
                btnGuardar.Enabled = _btnGuardar;
                btnImprimir.Enabled = _btnImprimir;
            }
            #endregion
            CargarCombos();
            CargarCombosDetalles();
            ObtenerListadoImportacion(txtPeriodo.Text, txtMes.Text);
            FormatoDecimalesGrillaProductos((int)Globals.ClientSession.i_CantidadDecimales, 6);
            _ListaDetalleGastosDto_Grilla = new List<importaciondetallegastosDto>();
            ValidarFechas();
            Scroll = int.Parse(dtpFechaRegistro.Value.Month.ToString());
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();

            #region txtRucProveedor
            txtRucProveedor1.LoadConfig("V");
            txtRucProveedor1.ItemSelectedAfterDropClosed += delegate
            {
                txtRucProveedor1_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            txtRucProveedor2.LoadConfig("V");
            txtRucProveedor2.ItemSelectedAfterDropClosed += delegate
            {
                txtRucProveedor2_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            txtRucProveedor3.LoadConfig("V");
            txtRucProveedor3.ItemSelectedAfterDropClosed += delegate
            {
                txtRucProveedor3_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            txtRucProveedor4.LoadConfig("V");
            txtRucProveedor4.ItemSelectedAfterDropClosed += delegate
            {
                txtRucProveedor4_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            #endregion

            if (Modulo == "KARDEX")
            {
                btnGuardar.Visible = false;
                btnImprimir.Visible = false;
                btnSalir.Visible = false;
            }
            ConfigurarGrillas();
        }
        private void ConfigurarGrillas()
        {
            this.grdDataFob.DisplayLayout.Bands[0].Columns["v_NroPedido"].Hidden =
                         Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1 ? false : true;
            this.grdDataProductos.DisplayLayout.Bands[0].Columns["v_NroPedido"].Hidden =
                 Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1 ? false : true;
        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            var _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridImportaciones(ref objOperationResult), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboVia, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 49, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboDestino, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchyForComboWithIDValueDto(ref objOperationResult, 24, null), DropDownListAction.Select);
            // Utils.Windows.LoadDropDownList(cboEstablecimiento, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchyForComboWithIDValueDto(ref objOperationResult, 25, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAgencia, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchyForComboWithIDValueDto(ref objOperationResult, 50, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumento1, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumento2, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumento3, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumento4, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult), DropDownListAction.Select);
            // Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumentoReferencia, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);

            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 30, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 27, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboSerieDoc, "Value2", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 53, null).OrderBy(x => x.Value2).ToList(), DropDownListAction.Select);
            cboDocumento.Value = "50";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboEstablecimiento.Enabled = false;
            cboDestino.Value = "1";
            cboDocumentoReferencia.Value = "-1";

        }
        #region Cabecera-Detalles
        private void FormatoDecimalesCajasTexto(int CantidadDecimales, string modo)
        {

            decimal flete = txtFlete.Text == string.Empty ? 0 : decimal.Parse(txtFlete.Text.ToString());
            decimal seguro = txtSeguro.Text == string.Empty ? 0 : decimal.Parse(txtSeguro.Text.ToString());
            decimal utilidad = txtUtilidad.Text == string.Empty ? 0 : decimal.Parse(txtUtilidad.Text.ToString());
            decimal tax = txtTax.Text == string.Empty ? 0 : decimal.Parse(txtTax.Text.ToString());
            decimal prom = txtProm.Text == string.Empty ? 0 : decimal.Parse(txtProm.Text.ToString());
            decimal tasa = txtTasaDespacho.Text == string.Empty ? 0 : decimal.Parse(txtTasaDespacho.Text.ToString());
            decimal percepcion = txtPercepcion.Text == string.Empty ? 0 : decimal.Parse(txtPercepcion.Text.ToString());
            decimal otrosgastos = txtOtrosGastos.Text == string.Empty ? 0 : decimal.Parse(txtOtrosGastos.Text.ToString());
            decimal intereses = txtIntereses.Text == string.Empty ? 0 : decimal.Parse(txtIntereses.Text.ToString());

            string FormatoDecimales;

            if (CantidadDecimales > 0)
            {
                string sharp = "0";
                FormatoDecimales = "0.";
                for (int i = 0; i < CantidadDecimales; i++)
                {
                    FormatoDecimales = FormatoDecimales + sharp;
                }
            }
            else
            {
                FormatoDecimales = "0";
            }
            if (modo == "a")
            {
                txtFlete.Text = flete.ToString(FormatoDecimales);
            }
            //txtSeguro.Text =seguro.ToString(FormatoDecimales);
            txtUtilidad.Text = utilidad.ToString(FormatoDecimales);
            txtTax.Text = tax.ToString(FormatoDecimales);
            txtProm.Text = prom.ToString(FormatoDecimales);
            txtTasaDespacho.Text = tasa.ToString(FormatoDecimales);
            txtPercepcion.Text = percepcion.ToString(FormatoDecimales);
            txtOtrosGastos.Text = otrosgastos.ToString(FormatoDecimales);
            txtIntereses.Text = intereses.ToString(FormatoDecimales);

        }
        private void CargarCombosDetalles()
        {
            OperationResult objOperationResult = new OperationResult();

            #region DetallesFob

            //Configura Combo TipoDocumento
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);
            UltraGridBand UltraGridBanda0 = new UltraGridBand("Banda 0", -1);
            UltraGridColumn ultraGridColumna0 = new UltraGridColumn("Id");
            ultraGridColumna0.Width = 40;
            UltraGridColumn ultraGridColumnaDescripcion0 = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion0.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion0.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion0.Width = 130;
            UltraGridColumn ultraGridColumna1 = new UltraGridColumn("Value2");
            ultraGridColumna1.Header.Caption = "Siglas";
            ultraGridColumna1.Width = 100;
            // ultraGridColumnaDescripcion0.Hidden = true;
            UltraGridBanda0.Columns.AddRange(new object[] { ultraGridColumnaDescripcion0, ultraGridColumna0, ultraGridColumna1 });
            ucbDocumento.DisplayLayout.BandsSerializer.Add(UltraGridBanda0);
            ucbDocumento.DropDownWidth = 250;
            ucbDocumento.DropDownStyle = UltraComboStyle.DropDownList;
            ucbDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucbDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select); //17-Unidad Medida 

            #endregion

            #region DetallesProducto

            //Configura Combo U.M.


            UltraGridBand ultraGridBanda1 = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumna10 = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion11 = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion11.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion11.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion11.Width = 267;
            ultraGridColumna10.Hidden = true;
            ultraGridBanda1.Columns.AddRange(new object[] { ultraGridColumna10, ultraGridColumnaDescripcion11 });
            ucbUnidadMedida.DisplayLayout.BandsSerializer.Add(ultraGridBanda1);
            ucbUnidadMedida.DropDownWidth = 270;
            ucbUnidadMedida.DropDownStyle = UltraComboStyle.DropDownList;


            Utils.Windows.LoadUltraComboList(ucbUnidadMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select); //17-Unidad Medida 


            #endregion

        }
        private void ObtenerListadoImportacion(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoImportacion = _objImportacionBL.ObtenerListadoImportaciones(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdImportacion);
                    btnImprimir.Enabled = Modulo == "C" ? true : _btnImprimir;
                    linkAsiento.Visible = true;
                    break;

                case "Nuevo":
                    if (_ListadoImportacion.Count != 0)
                    {
                        _MaxV = _ListadoImportacion.Count() - 1;
                        _ActV = _MaxV;
                        CargarDetalles("");
                        txtCorrelativo.Text = (int.Parse(_ListadoImportacion[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        _objImportacionDto = new importacionDto();
                        _movimientoDto = new movimientoDto();
                        EdicionBarraNavegacion(false);
                        LimpiarCabecera();


                    }
                    else
                    {
                        _movimientoDto = new movimientoDto();
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        CargarDetalles("");
                        _MaxV = 1;
                        _ActV = 1;
                        _objImportacionDto = new importacionDto();
                        btnNuevoMovimiento.Enabled = false;
                        EdicionBarraNavegacion(false);
                        LimpiarCabecera();

                    }
                    linkAsiento.Visible = false;
                    break;

                case "Guardado":
                    _MaxV = _ListadoImportacion.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdImportacion == "" | strIdImportacion == null)
                    {
                        CargarCabecera(_ListadoImportacion[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdImportacion);
                    }
                    btnNuevoMovimiento.Enabled = true;
                    EdicionBarraNavegacion(false);
                    cboAlmacen.Enabled = true;
                    btnImprimir.Enabled = Modulo == "C" ? true : _btnImprimir;
                    linkAsiento.Visible = true;
                    break;

                case "Consulta":

                    if (_ListadoImportacion.Count != 0)
                    {
                        _MaxV = _ListadoImportacion.Count() - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoImportacion[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoImportacion[_MaxV].Value2);
                        _Mode = "Edit";
                        EdicionBarraNavegacion(true);
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalles("");
                        _MaxV = 1;
                        _ActV = 1;
                        _objImportacionDto = new importacionDto();
                        btnNuevoMovimiento.Enabled = false;
                        _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                        EdicionBarraNavegacion(false);
                        txtMes.Enabled = true;
                    }
                    btnImprimir.Enabled = Modulo == "C" ? true : _btnImprimir;
                    linkAsiento.Visible = true;
                    break;

            }

        }

        private void CargarDetalles(string pstridImportacion)
        {
            OperationResult objOperationResult = new OperationResult();
            #region CargarDetalleFOB
            try
            {
                grdDataFob.DataSource = _objImportacionBL.ObtenerImportacionDetallesFob(ref objOperationResult, pstridImportacion);
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Hubo un error al cargar datos Fob", "Sistema", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (grdDataFob.Rows.Count() > 0)
            {

              //  BuscarProveedor();

                for (int i = 0; i < grdDataFob.Rows.Count(); i++)
                {

                    grdDataFob.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";

                }
               
            }
            #endregion
            #region CargarDetalleProducto
            try
            {

                grdDataProductos.DataSource = _objImportacionBL.ObtenerImportacionDetallesProducto(ref objOperationResult, pstridImportacion);
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Hubo un error al cargar datos Productos", "Sistema", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (grdDataProductos.Rows.Count() > 0)
            {
                BuscarDetallesGrillaProductos();
                for (int i = 0; i < grdDataProductos.Rows.Count(); i++)
                {
                    grdDataProductos.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                }
                cboAlmacen.Enabled = false;
            }
            else
            {
                cboAlmacen.Enabled = true;
            }

            #endregion


        }
        private void BuscarProveedor()
        {
            OperationResult objOperationResult = new OperationResult();
            List<string[]> ListaCadena = new List<string[]>();
            ListaCadena = _objImportacionBL.DevolverNombreProveedores(grdDataFob.Rows[0].Cells["v_IdImportacion"].Value.ToString());
            for (int i = 0; i < ListaCadena.Count; i++)
            {
                UtilizadoBefore = true;
                grdDataFob.Rows[i].Cells["v_RazonSocial"].Value = ((string[])ListaCadena[i])[0];
                grdDataFob.Rows[i].Cells["v_CodCliente"].Value = ((string[])ListaCadena[i])[1];
            }
        }

        private void BuscarDetallesGrillaProductos()
        {
            OperationResult objOperationResult = new OperationResult();
            List<string[]> ListaCadena = new List<string[]>();
            ListaCadena = _objImportacionBL.DevolverNombreGrillaProductos(grdDataFob.Rows[0].Cells["v_IdImportacion"].Value.ToString());
            for (int i = 0; i < ListaCadena.Count; i++)
            {
                grdDataProductos.Rows[i].Cells["v_NombreProducto"].Value = ((string[])ListaCadena[i])[1];
                grdDataProductos.Rows[i].Cells["v_CodCliente"].Value = ((string[])ListaCadena[i])[2];
                grdDataProductos.Rows[i].Cells["v_CodigoInterno"].Value = ((string[])ListaCadena[i])[0];
                grdDataProductos.Rows[i].Cells["EsServicio"].Value = ((string[])ListaCadena[i])[3];
                grdDataProductos.Rows[i].Cells["Empaque"].Value = ((string[])ListaCadena[i])[4];
                grdDataProductos.Rows[i].Cells["UMEmpaque"].Value = ((string[])ListaCadena[i])[5];
                grdDataProductos.Rows[i].Cells["i_IdUnidadMedidaProducto"].Value = ((string[])ListaCadena[i])[6];

            }
        }
        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            txtCorrelativo.Enabled = ON_OFF;
            btnAnterior.Enabled = ON_OFF;
            btnSiguiente.Enabled = ON_OFF;
            txtMes.Enabled = ON_OFF;
            btnNuevoMovimiento.Enabled = ON_OFF;
        }
        private void LimpiarCabecera()
        {
            OperationResult objOperationResult = new OperationResult();

            object sender = new object();
            EventArgs e = new EventArgs();
            cboMoneda.Value = Globals.ClientSession.i_IdMonedaImportacion.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaLlegada.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaArrivo.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaVencimiento.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaDoc1.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaDoc2.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaDoc3.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaDoc4.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            cboDocumento.Value = "50";
            cboEstado.Value = "1";

            cboIGV.Value = Globals.ClientSession.i_IdIgv.ToString();

            txtTipoCambio.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
            txtTipoCambioDoc1.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaDoc1.Value.Date);
            txtTipoCambioDoc2.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaDoc2.Value.Date);
            txtTipoCambioDoc3.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaDoc3.Value.Date);
            txtTipoCambioDoc4.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaDoc4.Value.Date);



            txtUtilidad.Text = 0.ToString("0.0000");
            txtFlete.Text = 0.ToString("0.0000");
            txtTax.Text = 0.ToString("0.0000");
            txtProm.Text = 0.ToString("0.0000");
            txtTotalIgv.Text = 0.ToString("0.0000");
            txtTasaDespacho.Text = 0.ToString("0.0000");
            txtPercepcion.Text = 0.ToString("0.0000");
            txtOtrosGastos.Text = 0.ToString("0.0000");

            cboDocumento1.Value = "-1";
            cboDocumento2.Value = "-1";
            cboDocumento3.Value = "-1";
            cboDocumento4.Value = "-1";
            cboSerieDoc.Value = "-1";

            rbSiAdValorem.Checked = false;
            rbSiSeguro.Checked = false;

            rbNoAdValorem.Checked = true;
            rbNoSeguro.Checked = true;

            rbSiAdValorem_CheckedChanged(sender, e);
            rbSiSeguro_CheckedChanged(sender, e);

            btnEliminarDetalleFob.Enabled = false;
            btnEliminarDetalleProducto.Enabled = false;
            txtSerieDoc1.Text = string.Empty;
            btnImprimir.Enabled = false;
            txtSeguro.Enabled = false;
            txtAdValorem.Enabled = false;
            cboDestino.Value = "-1";
            cboVia.Value = "-1";

            cboDestino.Value = "1";
            cboAgencia.Value = "-1";

        }

        private void btnAgregarDetalleFob_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();


            if (txtTipoCambio.Text == string.Empty || decimal.Parse(txtTipoCambio.Text) <= 0)
            {
                UltraMessageBox.Show("Ingrese un tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTipoCambio.Focus();
                return;
            }

            else
            {
                if (grdDataFob.ActiveRow != null)
                {
                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value != null)
                    {

                        UltraGridRow row = grdDataFob.DisplayLayout.Bands[0].AddNew();
                        grdDataFob.Rows.Move(row, grdDataFob.Rows.Count() - 1);
                        this.grdDataFob.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        //row.Cells["i_RegistroEstado"].Value = "Agregado";
                        row.Cells["i_RegistroEstado"].Value = "Modificado";
                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                        row.Cells["i_IdTipoDocumento"].Value = "-1";
                        row.Cells["t_FechaEmision"].Value = DateTime.Today;
                        DateTime Fechas = Convert.ToDateTime(row.Cells["t_FechaEmision"].Value.ToString());
                        row.Cells["d_TipoCambio"].Value = decimal.Parse(_objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, Fechas.Date).ToString());


                    }
                }
                else
                {
                    UltraGridRow row = grdDataFob.DisplayLayout.Bands[0].AddNew();
                    grdDataFob.Rows.Move(row, grdDataFob.Rows.Count() - 1);
                    this.grdDataFob.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    //row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdTipoDocumento"].Value = "-1";
                    row.Cells["t_FechaEmision"].Value = DateTime.Today;
                    DateTime Fechas = Convert.ToDateTime(row.Cells["t_FechaEmision"].Value.ToString());
                    row.Cells["d_TipoCambio"].Value = decimal.Parse(_objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, Fechas.Date).ToString());


                }

                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["v_codCliente"];
                this.grdDataFob.ActiveCell = aCell;
                grdDataFob.PerformAction(UltraGridAction.EnterEditMode, false, false);
                grdDataFob.Focus();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            }


        }
        private void CargarCabecera(string pstrIdImportacion)
        {

            OperationResult objOperationResult = new OperationResult();
            _objImportacionDto = new importacionDto();
            _objImportacionDto = _objImportacionBL.ObtenerImportacionCabecera(ref objOperationResult, pstrIdImportacion);
            if (_objImportacionDto != null)
            {
                _Mode = "Edit";
                txtPeriodo.Text = _objImportacionDto.v_Periodo;
                txtMes.Text = _objImportacionDto.v_Mes;
                txtCorrelativo.Text = _objImportacionDto.v_Correlativo;
                cboIGV.Value = _objImportacionDto.i_Igv.ToString();
                cboDocumento.Value = _objImportacionDto.i_IdTipoDocumento.Value.ToString();
                cboSerieDoc.Value = _objImportacionDto.i_IdSerieDocumento.Value.ToString();
                txtNumeroDoc.Text = _objImportacionDto.v_CorrelativoDocumento;
                cboVia.Value = _objImportacionDto.i_IdTipoVia.ToString();
                cboDestino.Value = _objImportacionDto.i_IdDestino.ToString();
                dtpFechaRegistro.Value = _objImportacionDto.t_FechaRegistro.Value;
                dtpFechaEmision.Value = _objImportacionDto.t_FechaEmision.Value;
                dtpFechaVencimiento.Value = _objImportacionDto.t_FechaPagoVencimiento.Value;
                txtTipoCambio.Text = _objImportacionDto.d_TipoCambio.ToString();
                cboEstablecimiento.Value = _objImportacionDto.i_IdEstablecimiento.ToString();
                txtNroOrden.Text = _objImportacionDto.v_NroOrden;
                txtBl.Text = _objImportacionDto.v_Bl;
                dtpFechaArrivo.Value = _objImportacionDto.t_FechaArrivo.Value;
                dtpFechaLlegada.Value = _objImportacionDto.t_FechaLLegada.Value;
                cboAgencia.Value = _objImportacionDto.i_IdAgencia.ToString();
                txtTerminal.Text = _objImportacionDto.v_Terminal;
                txtEnt11.Text = _objImportacionDto.v_Ent1Serie;
                txtEnt12.Text = _objImportacionDto.v_Ent1Correlativo;
                txtEnt21.Text = _objImportacionDto.v_Ent2Serie;
                txtEnt22.Text = _objImportacionDto.v_Ent2Correlativo;
                cboAlmacen.Value = _objImportacionDto.i_IdAlmacen.ToString();
                txtUtilidad.Text = decimal.Parse(_objImportacionDto.d_Utilidad.ToString()).ToString("0.00");
                txtTotalValorFob.Text = decimal.Parse(_objImportacionDto.d_TotalValorFob.ToString()).ToString("0.00");
                txtFlete.Text = decimal.Parse(_objImportacionDto.d_Flete.ToString()).ToString("0.0000");
                cboDocumento1.Value = _objImportacionDto.i_IdTipoDocumento1.Value.ToString();
                txtSerieDoc1.Text = _objImportacionDto.v_SerieDocumento1.Trim();
                txtCorrelativoDoc1.Text = _objImportacionDto.v_CorrelativoDocumento1.Trim();
                dtpFechaDoc1.Value = _objImportacionDto.t_FechaEmisionDoc1.Value;
                txtTipoCambioDoc1.Text = _objImportacionDto.d_TipoCambioDoc1.ToString();
                txtRucProveedor1.Text = _objImportacionDto.RucProveedor1;
                txtRazonSocialProveedor1.Text = _objImportacionDto.NombreProveedor1;
                rbSiSeguro.Checked = _objImportacionDto.i_PagaSeguro == 1 ? true : false;
                rbNoSeguro.Checked = _objImportacionDto.i_PagaSeguro == 0 ? true : false;
                txtSeguro.Text = decimal.Parse(_objImportacionDto.d_PagoSeguro.ToString()).ToString("0.0000");
                cboDocumento2.Value = _objImportacionDto.i_IdTipoDocumento2.Value.ToString();
                txtSerieDoc2.Text = _objImportacionDto.v_SerieDocumento2.Trim();
                txtCorrelativoDoc2.Text = _objImportacionDto.v_CorrelativoDocumento2.Trim();
                dtpFechaDoc2.Value = _objImportacionDto.t_FechaEmisionDoc2.Value;
                txtTipoCambioDoc2.Text = _objImportacionDto.d_TipoCambioDoc2.ToString();
                txtRucProveedor2.Text = _objImportacionDto.RucProveedor2;
                txtRazonSocialProveedor2.Text = _objImportacionDto.NombreProveedor2;
                rbSiAdValorem.Checked = _objImportacionDto.i_AdValorem == 1 ? true : false;
                rbNoAdValorem.Checked = _objImportacionDto.i_AdValorem == 0 ? true : false;
                txtAdValorem.Text = decimal.Parse(_objImportacionDto.d_AdValorem.ToString()).ToString("0.0000");
                cboDocumento3.Value = _objImportacionDto.i_IdTipoDocumento3.Value.ToString();
                txtSerieDoc3.Text = _objImportacionDto.v_SerieDocumento3.Trim();
                txtCorrelativoDoc3.Text = _objImportacionDto.v_CorrelativoDocumento3.Trim();
                dtpFechaDoc3.Value = _objImportacionDto.t_FechaEmisionDoc3.Value;
                txtTipoCambioDoc3.Text = _objImportacionDto.d_TipoCambioDoc3.ToString();
                txtRucProveedor3.Text = _objImportacionDto.RucProveedor3;
                txtRazonSocialProveedor3.Text = _objImportacionDto.NombreProveedor3;
                txtSubTotal.Text = decimal.Parse(_objImportacionDto.d_SubTotal.ToString()).ToString("0.0000");
                txtTax.Text = decimal.Parse(_objImportacionDto.d_Tax.ToString()).ToString("0.0000");
                cboDocumento4.Value = _objImportacionDto.i_IdTipoDocumento4.Value.ToString();
                txtSerieDoc4.Text = _objImportacionDto.v_SerieDocumento4.Trim();
                txtCorrelativoDoc4.Text = _objImportacionDto.v_CorrelativoDoc4.Trim();
                dtpFechaDoc4.Value = _objImportacionDto.t_FechaEmisionDoc4.Value;
                txtTipoCambioDoc4.Text = _objImportacionDto.d_TipoCambioDoc4.ToString();
                txtRucProveedor4.Text = _objImportacionDto.RucProveedor4;
                txtRazonSocialProveedor4.Text = _objImportacionDto.NombreProveedor4;
                txtProm.Text = decimal.Parse(_objImportacionDto.d_Prom.ToString()).ToString();
                txtTasaDespacho.Text = decimal.Parse(_objImportacionDto.d_TasaDespacho.ToString()).ToString("0.0000");
                txtPercepcion.Text = decimal.Parse(_objImportacionDto.d_Percepcion.ToString()).ToString("0.0000");
                cboMoneda.Value = _objImportacionDto.i_IdMoneda.ToString();
                txtTotalIgv.Text = decimal.Parse(_objImportacionDto.d_Igv.ToString()).ToString("0.0000");
                txtIntereses.Text = decimal.Parse(_objImportacionDto.d_Intereses.ToString()).ToString("0.0000");
                txtOtrosGastos.Text = decimal.Parse(_objImportacionDto.d_OtrosGastos.ToString()).ToString("0.0000");

                if (int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles)
                {
                    TotalSoles = decimal.Parse(txtOtrosGastos.Text);
                    TotalDolares = CalcularCambioOtrosGastos(int.Parse(cboMoneda.Value.ToString()));
                }
                else
                {
                    TotalDolares = decimal.Parse(txtOtrosGastos.Text);
                    TotalSoles = CalcularCambioOtrosGastos(int.Parse(cboMoneda.Value.ToString()));
                }
                cboEstado.Value = _objImportacionDto.i_IdEstado.ToString();
                txtValorFob.Text = decimal.Parse(_objImportacionDto.d_ValorFob.ToString()).ToString("0.000000");
                txtSeguroResultado.Text = decimal.Parse(_objImportacionDto.d_TotalSeguro.ToString()).ToString("0.000000");
                txtIgvResultado.Text = decimal.Parse(_objImportacionDto.d_TotalIgv.ToString()).ToString("0.000000");
                txtFleteResultado.Text = decimal.Parse(_objImportacionDto.d_TotalFlete.ToString()).ToString("0.000000");
                txtAdValoremResultado.Text = decimal.Parse(_objImportacionDto.d_TotalAdValorem.ToString()).ToString("0.000000");
                txtOtrosGastosResultado.Text = decimal.Parse(_objImportacionDto.d_TotalOtrosGastos.ToString()).ToString("0.000000");
                cboDocumentoReferencia.Value = _objImportacionDto.i_IdTipoDocRerefencia;
                txtNumeroDocumentoReferencia.Text = _objImportacionDto.v_NumeroDocRerefencia;
                CargarDetalles(_objImportacionDto.v_IdImportacion);

            }

        }


        private decimal CalcularCambioOtrosGastos(int Moneda)
        {
            decimal CambioOtrosGastos = 0;
            OperationResult objOperationResult = new OperationResult();
            var DetallesGastos = _objImportacionBL.ObtenerImportacionDetallesGastos(ref objOperationResult, _objImportacionDto.v_IdImportacion);
            if (DetallesGastos.Any())
            {
                decimal sol = 0, dol = 0;
                foreach (var item in DetallesGastos)
                {
                    switch (item.i_IdMoneda.Value)
                    {
                        case 1://Soles
                            sol = item.d_ValorVenta.Value + item.d_NAfectoDetraccion.Value + sol;
                            dol = Utils.Windows.DevuelveValorRedondeado(((item.d_ValorVenta.Value + item.d_NAfectoDetraccion.Value) / item.d_TipoCambio.Value), 2) + dol;

                            break;
                        case 2: //Dolares
                            dol = item.d_ValorVenta.Value + item.d_NAfectoDetraccion.Value + dol;
                            sol = Utils.Windows.DevuelveValorRedondeado(((item.d_ValorVenta.Value + item.d_NAfectoDetraccion.Value) * item.d_TipoCambio.Value), 2) + sol;
                            break;
                    }
                }
                return CambioOtrosGastos = Moneda == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado(dol, 2) : Utils.Windows.DevuelveValorRedondeado(sol, 2);


            }
            else
            {
                return 0;
            }

            //TotalSoles = Utils.Windows.DevuelveValorRedondeado(sol, 2);
            //TotalDolares = Utils.Windows.DevuelveValorRedondeado(dol, 2);


        }
        private void LlenarTemporalesFobProducto()
        {
            #region Fob
            if (grdDataFob.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdDataFob.Rows)
                {


                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {

                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {

                                _objImportaciondetallefobDto = new importaciondetallefobDto();
                                _objImportaciondetallefobDto.v_IdImportacion = _objImportacionDto.v_IdImportacion;
                                _objImportaciondetallefobDto.v_IdCliente = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();
                                _objImportaciondetallefobDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _objImportaciondetallefobDto.v_SerieDocumento = Fila.Cells["v_SerieDocumento"].Value == null ? null : Fila.Cells["v_SerieDocumento"].Value.ToString();
                                _objImportaciondetallefobDto.v_CorrelativoDocumento = Fila.Cells["v_CorrelativoDocumento"].Value == null ? null : Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                                _objImportaciondetallefobDto.t_FechaEmision = DateTime.Parse(Fila.Cells["t_FechaEmision"].Value.ToString());
                                _objImportaciondetallefobDto.d_TipoCambio = Fila.Cells["d_TipoCambio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_TipoCambio"].Value.ToString());
                                _objImportaciondetallefobDto.d_ValorFob = Fila.Cells["d_ValorFob"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorFob"].Value.ToString());
                                _objImportaciondetallefobDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _TempDetalleFobDto_Agregar.Add(_objImportaciondetallefobDto);

                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _objImportaciondetallefobDto = new importaciondetallefobDto();
                                _objImportaciondetallefobDto.v_IdImportacion = Fila.Cells["v_IdImportacion"].Value == null ? null : Fila.Cells["v_IdImportacion"].Value.ToString();
                                _objImportaciondetallefobDto.v_IdImportacionDetalleFob = Fila.Cells["v_IdImportacionDetalleFob"].Value == null ? null : Fila.Cells["v_IdImportacionDetalleFob"].Value.ToString();
                                _objImportaciondetallefobDto.v_IdCliente = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();
                                _objImportaciondetallefobDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _objImportaciondetallefobDto.v_SerieDocumento = Fila.Cells["v_SerieDocumento"].Value == null ? null : Fila.Cells["v_SerieDocumento"].Value.ToString();
                                _objImportaciondetallefobDto.v_CorrelativoDocumento = Fila.Cells["v_CorrelativoDocumento"].Value == null ? null : Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                                _objImportaciondetallefobDto.t_FechaEmision = DateTime.Parse(Fila.Cells["t_FechaEmision"].Value.ToString());
                                _objImportaciondetallefobDto.d_TipoCambio = Fila.Cells["d_TipoCambio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_TipoCambio"].Value.ToString());
                                _objImportaciondetallefobDto.d_ValorFob = Fila.Cells["d_ValorFob"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorFob"].Value.ToString());
                                _objImportaciondetallefobDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _objImportaciondetallefobDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _objImportaciondetallefobDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value.ToString());
                                _objImportaciondetallefobDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _TempDetalleFobDto_Modificar.Add(_objImportaciondetallefobDto);



                            }
                            break;
                    }
                }
            }
            #endregion
            #region Productos
            if (grdDataProductos.Rows.Count != 0)
            {
                foreach (UltraGridRow Fila in grdDataProductos.Rows)
                {

                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                bActualizarNotaIngreso = true;
                                _objImportaciondetalleproductoDto = new importaciondetalleproductoDto();
                                _objImportaciondetalleproductoDto.v_IdImportacion = _objImportacionDto.v_IdImportacion;
                                _objImportaciondetalleproductoDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _objImportaciondetalleproductoDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _objImportaciondetalleproductoDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Text);
                                _objImportaciondetalleproductoDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Text);
                                _objImportaciondetalleproductoDto.d_ValorFob = Fila.Cells["d_ValorFob"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorFob"].Text);
                                _objImportaciondetalleproductoDto.d_Flete = Fila.Cells["d_Flete"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Flete"].Text);
                                _objImportaciondetalleproductoDto.d_Seguro = Fila.Cells["d_Seguro"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Seguro"].Text);
                                _objImportaciondetalleproductoDto.d_AdValorem = Fila.Cells["d_AdValorem"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_AdValorem"].Text);
                                _objImportaciondetalleproductoDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Text);
                                _objImportaciondetalleproductoDto.d_OtrosGastos = Fila.Cells["d_OtrosGastos"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_OtrosGastos"].Text);
                                _objImportaciondetalleproductoDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Total"].Text);
                                _objImportaciondetalleproductoDto.d_CostoUnitario = Fila.Cells["d_CostoUnitario"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CostoUnitario"].Text);

                                _objImportaciondetalleproductoDto.d_CostoUnitarioCambio = Fila.Cells["d_CostoUnitarioCambio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CostoUnitarioCambio"].Text);
                                _objImportaciondetalleproductoDto.v_IdCliente = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();
                                _objImportaciondetalleproductoDto.v_NroFactura = Fila.Cells["v_NroFactura"].Value == null ? null : Fila.Cells["v_NroFactura"].Value.ToString();
                                _objImportaciondetalleproductoDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _objImportaciondetalleproductoDto.EsServicio = Fila.Cells["EsServicio"].Value == null | Fila.Cells["EsServicio"].Value.ToString() == "0" ? 0 : 1;
                                _objImportaciondetalleproductoDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _objImportaciondetalleproductoDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _TempDetalleProdDto_Agregar.Add(_objImportaciondetalleproductoDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                bActualizarNotaIngreso = true;
                                _objImportaciondetalleproductoDto = new importaciondetalleproductoDto();
                                _objImportaciondetalleproductoDto.v_IdImportacion = Fila.Cells["v_IdImportacion"].Value == null ? null : Fila.Cells["v_IdImportacion"].Value.ToString();
                                _objImportaciondetalleproductoDto.v_IdImportacionDetalleProducto = Fila.Cells["v_IdImportacionDetalleProducto"].Value == null ? null : Fila.Cells["v_IdImportacionDetalleProducto"].Value.ToString();
                                _objImportaciondetalleproductoDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _objImportaciondetalleproductoDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _objImportaciondetalleproductoDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _objImportaciondetalleproductoDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Text);
                                _objImportaciondetalleproductoDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Text);
                                _objImportaciondetalleproductoDto.d_ValorFob = Fila.Cells["d_ValorFob"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorFob"].Text);
                                _objImportaciondetalleproductoDto.d_Flete = Fila.Cells["d_Flete"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Flete"].Text);
                                _objImportaciondetalleproductoDto.d_Seguro = Fila.Cells["d_Seguro"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Seguro"].Text);
                                _objImportaciondetalleproductoDto.d_AdValorem = Fila.Cells["d_AdValorem"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_AdValorem"].Text);
                                _objImportaciondetalleproductoDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Text);
                                _objImportaciondetalleproductoDto.d_OtrosGastos = Fila.Cells["d_OtrosGastos"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_OtrosGastos"].Text);
                                _objImportaciondetalleproductoDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Total"].Text);
                                _objImportaciondetalleproductoDto.d_CostoUnitario = Fila.Cells["d_CostoUnitario"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CostoUnitario"].Text);
                                _objImportaciondetalleproductoDto.d_CostoUnitarioCambio = Fila.Cells["d_CostoUnitarioCambio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CostoUnitarioCambio"].Text);
                                _objImportaciondetalleproductoDto.v_IdCliente = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();
                                _objImportaciondetalleproductoDto.v_NroFactura = Fila.Cells["v_NroFactura"].Value == null ? null : Fila.Cells["v_NroFactura"].Value.ToString();
                                _objImportaciondetalleproductoDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _objImportaciondetalleproductoDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _objImportaciondetalleproductoDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value.ToString());
                                _objImportaciondetalleproductoDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _objImportaciondetalleproductoDto.EsServicio = Fila.Cells["EsServicio"].Value == null | Fila.Cells["EsServicio"].Value.ToString() == "0" ? 0 : 1;
                                _objImportaciondetalleproductoDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _objImportaciondetalleproductoDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _TempDetalleProdDto_Modificar.Add(_objImportaciondetalleproductoDto);

                            }
                            break;

                    }

                }
            }
            #endregion


        }


        #endregion

        #region Comportamiento Controles

        private void txtSerieDoc_Click(object sender, EventArgs e)
        {
            //frmAduanas formAduanas = new frmAduanas();
            //formAduanas.ShowDialog();
            //txtSerieDoc.Text = formAduanas._codigoAduana;
            //txtNumeroDoc.Focus();
        }

        private void cboDocumento_Leave(object sender, EventArgs e)
        {

            if (cboDocumento.Text.Trim() == "")
            {
                cboDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() | p.Id == cboDocumento.Text);
                if (x == null)
                {
                    cboDocumento.Value = "-1";
                }

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

        private void dtpFechaEmision_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtTipoCambio.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
        }

        private void dtpFechaDoc1_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtTipoCambioDoc1.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaDoc1.Value.Date);
        }

        private void dtpFechaDoc2_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtTipoCambioDoc2.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaDoc2.Value.Date);
        }

        private void dtpFechaDoc3_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtTipoCambioDoc3.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaDoc3.Value.Date);
        }

        private void dtpFechaDoc4_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtTipoCambioDoc4.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaDoc4.Value.Date);
        }

        private void btnOtrosGastos_Click(object sender, EventArgs e)
        {

            if (cboIGV.Value.ToString() == "-1")
            {
                UltraMessageBox.Show("Por favor seleccione el IGV.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            frmGastos formGastos = new frmGastos(_objImportacionDto, _objImportacionDto.v_IdImportacion, strModo, _ListaDetalleGastosDto_Grilla, EliminadoGastos, decimal.Parse(cboIGV.Text), dtpFechaRegistro.Value.Date, ImportacionGuardada);
            formGastos.ShowDialog();

            if (cboMoneda.Value.ToString() == "1")
            {

                //TotalSoles = formGastos.TotalSoles;
                //TotalDolares = formGastos.TotalDolares;
                TotalSoles = formGastos.TotalSoles;
                TotalDolares = formGastos.TotalDolares;
                txtOtrosGastos.Text = TotalSoles.ToString("0.0000");
            }
            else if (cboMoneda.Value.ToString() == "2")
            {
                TotalDolares = formGastos.TotalDolares;
                TotalSoles = formGastos.TotalSoles;
                txtOtrosGastos.Text = TotalDolares.ToString("0.0000");

            }
            _TempDetalleGastosDto_Agregar = formGastos._TempDetalleGastosDto_Agregar;
            _TempDetalleGastosDto_Modificar = formGastos._TempDetalleGastosDto_Modificar;
            _TempDetalleGastosDto_Eliminar = formGastos._TempDetalleGastosDto_Eliminar;
            _ListaDetalleGastosDto_Grilla = formGastos._ListaDetalleGastosDto_Grilla;
            ImportacionGuardada = formGastos.ImportacionGuardada;
            EliminadoGastos = formGastos.Eliminado;
            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();

            }


        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEntero(txtSerieDoc, e);
        }

        private void txtNumeroDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNumeroDoc, e);
        }

        private void txtFlete_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroDecimal(txtFlete, e);

        }
        private void txtSeguro_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroDecimal(txtSeguro, e);
        }

        private void txtAdValorem_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroDecimal(txtAdValorem, e);
        }

        private void rbSiSeguro_CheckedChanged(object sender, EventArgs e)
        {
            txtSeguro.Enabled = true;
            txtSeguro.Text = "0.0000";
        }

        private void rbNoSeguro_CheckedChanged(object sender, EventArgs e)
        {
            txtSeguro.Enabled = false;

        }

        private void rbSiAdValorem_CheckedChanged(object sender, EventArgs e)
        {
            txtAdValorem.Enabled = true;
            txtAdValorem.Text = "0.0000";
        }

        private void rbNoAdValorem_CheckedChanged(object sender, EventArgs e)
        {
            txtAdValorem.Enabled = false;

        }
        private void cboDocumento1_Leave(object sender, EventArgs e)
        {
            if (strModo == "Nuevo")
            {
                if (cboDocumento1.Text.Trim() == "")
                {
                    cboDocumento1.Value = "-1";
                }
                else
                {
                    var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento1.Value.ToString() || p.Id == cboDocumento1.Text.Trim());
                    if (x == null)
                    {
                        cboDocumento1.Value = "-1";
                    }
                }
                // txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
                //txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
                //ComprobarExistenciaCorrelativoDocumento();
            }
        }

        private void cboDocumento1_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento1.Rows)
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
                        if (row.Cells[column].Text.Contains(cboDocumento1.Text.Trim().ToUpper()))
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

        private void cboDocumento2_Leave(object sender, EventArgs e)
        {
            if (strModo == "Nuevo")
            {
                if (cboDocumento2.Text.Trim() == "")
                {
                    cboDocumento2.Value = "-1";
                }
                else
                {
                    var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento2.Value.ToString() || p.Id == cboDocumento2.Text.Trim());
                    if (x == null)
                    {
                        cboDocumento2.Value = "-1";
                    }
                }
                // txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
                //txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
                //ComprobarExistenciaCorrelativoDocumento();
            }
        }

        private void cboDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            {
                if (cboDocumento.Value == null) return;
                if (cboDocumento.Value.ToString() == "-1") cboDocumento.Text = string.Empty;
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

        private void cboDocumento1_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento1.Rows)
            {
                if (cboDocumento1.Value == null) return;
                if (cboDocumento1.Value.ToString() == "-1") cboDocumento1.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento1.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento1.Text.Trim().ToUpper()))
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

        private void cboDocumento2_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento2.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento2.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento2.Text.Trim().ToUpper()))
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

        private void cboDocumento2_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento2.Rows)
            {
                if (cboDocumento2.Value == null) return;
                if (cboDocumento2.Value.ToString() == "-1") cboDocumento2.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento2.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento2.Text.Trim().ToUpper()))
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

        private void cboDocumento3_Leave(object sender, EventArgs e)
        {
            if (strModo == "Nuevo")
            {
                if (cboDocumento3.Text.Trim() == "")
                {
                    cboDocumento3.Value = "-1";
                }
                else
                {
                    var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento3.Value.ToString() || p.Id == cboDocumento3.Text.Trim());
                    if (x == null)
                    {
                        cboDocumento3.Value = "-1";
                    }
                }

            }
        }

        private void cboDocumento3_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento3.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento3.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento3.Text.Trim().ToUpper()))
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

        private void cboDocumento3_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento3.Rows)
            {
                if (cboDocumento3.Value == null) return;
                if (cboDocumento3.Value.ToString() == "-1") cboDocumento3.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento3.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento3.Text.Trim().ToUpper()))
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

        private void cboDocumento4_Leave(object sender, EventArgs e)
        {
            if (strModo == "Nuevo")
            {
                if (cboDocumento4.Text.Trim() == "")
                {
                    cboDocumento4.Value = "-1";
                }
                else
                {
                    var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento4.Value.ToString() || p.Id == cboDocumento4.Text.Trim());
                    if (x == null)
                    {
                        cboDocumento4.Value = "-1";
                    }
                }

            }
        }

        private void cboDocumento4_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento4.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento4.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento4.Text.Trim().ToUpper()))
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

        private void cboDocumento4_AfterDropDown(object sender, EventArgs e)
        {

            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento4.Rows)
            {
                if (cboDocumento4.Value == null) return;
                if (cboDocumento4.Value.ToString() == "-1") cboDocumento4.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento4.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento4.Text.Trim().ToUpper()))
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

        private void txtRucProveedor1_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (!txtRucProveedor1.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucProveedor1.Text.Trim() != "" & txtRucProveedor1.TextLength <= 7)
                {


                    Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor1.Text, "RUC");
                    frm.ShowDialog();
                    if (frm._IdProveedor != null)
                    {

                        txtRazonSocialProveedor1.Text = frm._RazonSocial;
                        txtRucProveedor1.Text = frm._NroDocumento;
                        _objImportacionDto.v_IdClienteDoc1 = frm._IdProveedor;

                    }
                }


                else
                {
                    #region BusquedaCliente
                    if (txtRucProveedor1.TextLength == 8 | txtRucProveedor1.TextLength == 11)
                    {
                        string[] DatosCliente = new string[4];

                        DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, (txtRucProveedor1.Text.Trim()));
                        if (DatosCliente != null)
                        {
                            _objImportacionDto.v_IdClienteDoc1 = DatosCliente[0];
                            txtRazonSocialProveedor1.Text = DatosCliente[2].Trim();
                            txtRucProveedor1.Text = DatosCliente[3].Trim();


                        }
                        else // LLamo a Cliente rápido
                        {
                            #region Cliente Rapido

                            Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucProveedor1.Text.Trim(), "V");
                            frm.ShowDialog();
                            if (frm._Guardado == true)
                            {
                                txtRucProveedor1.Text = frm._NroDocumentoReturn;

                                DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor1.Text.Trim());
                                if (DatosCliente != null)
                                {

                                    _objImportacionDto.v_IdClienteDoc1 = DatosCliente[0];
                                    txtRucProveedor1.Text = DatosCliente[3];
                                    txtRazonSocialProveedor1.Text = DatosCliente[2];
                                }

                                else
                                {
                                    _objImportacionDto.v_IdClienteDoc1 = string.Empty;
                                    txtRucProveedor1.Clear();
                                    txtRazonSocialProveedor1.Clear();
                                }

                            }

                            #endregion
                        }

                    }
                    #endregion

                }

            }
        }

        private void txtRucProveedor1_TextChanged(object sender, EventArgs e)
        {
            if (txtRucProveedor1.Text == string.Empty)
            {
                _objImportacionDto.v_IdClienteDoc1 = null;
                txtRazonSocialProveedor1.Clear();
            }
        }

        private void btnBuscarCliente1_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtRazonSocialProveedor1.Text = frm._RazonSocial;
                txtRucProveedor1.Text = frm._NroDocumento;
                _objImportacionDto.v_IdClienteDoc1 = frm._IdProveedor;
            }
        }

        private void txtRucProveedor2_TextChanged(object sender, EventArgs e)
        {
            if (txtRucProveedor2.Text == string.Empty)
            {
                _objImportacionDto.v_IdClienteDoc2 = null;
                txtRazonSocialProveedor2.Clear();
            }
        }

        private void txtRucProveedor2_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (!txtRucProveedor2.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucProveedor2.Text.Trim() != "" & txtRucProveedor2.TextLength <= 7)
                {


                    Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor2.Text, "RUC");
                    frm.ShowDialog();
                    if (frm._IdProveedor != null)
                    {

                        txtRazonSocialProveedor2.Text = frm._RazonSocial;
                        txtRucProveedor2.Text = frm._NroDocumento;
                        _objImportacionDto.v_IdClienteDoc2 = frm._IdProveedor;

                    }
                }


                else
                {
                    #region BusquedaCliente
                    if (txtRucProveedor2.TextLength == 8 | txtRucProveedor2.TextLength == 11)
                    {
                        string[] DatosCliente = new string[4];

                        DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor2.Text.Trim());
                        if (DatosCliente != null)
                        {
                            _objImportacionDto.v_IdClienteDoc2 = DatosCliente[0];
                            txtRazonSocialProveedor2.Text = DatosCliente[2].Trim();
                            txtRucProveedor2.Text = DatosCliente[3].Trim();


                        }
                        else // LLamo a Cliente rápido
                        {
                            #region Cliente Rapido

                            Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucProveedor2.Text.Trim(), "V");
                            frm.ShowDialog();
                            if (frm._Guardado == true)
                            {
                                txtRucProveedor2.Text = frm._NroDocumentoReturn;

                                DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor2.Text.Trim());
                                if (DatosCliente != null)
                                {

                                    _objImportacionDto.v_IdClienteDoc2 = DatosCliente[0];
                                    txtRucProveedor2.Text = DatosCliente[3];
                                    txtRazonSocialProveedor2.Text = DatosCliente[2];
                                }

                                else
                                {
                                    _objImportacionDto.v_IdClienteDoc2 = string.Empty;
                                    txtRucProveedor2.Clear();
                                    txtRazonSocialProveedor2.Clear();
                                }

                            }

                            #endregion
                        }

                    }
                    #endregion

                }

            }
        }

        private void btnBuscarCliente2_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtRazonSocialProveedor2.Text = frm._RazonSocial;
                txtRucProveedor2.Text = frm._NroDocumento;
                _objImportacionDto.v_IdClienteDoc2 = frm._IdProveedor;
            }
        }

        private void txtRucProveedor3_TextChanged(object sender, EventArgs e)
        {
            if (txtRucProveedor3.Text == string.Empty)
            {
                _objImportacionDto.v_IdClienteDoc3 = null;
                txtRazonSocialProveedor3.Clear();
            }
        }

        private void txtRucProveedor3_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (!txtRucProveedor3.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucProveedor3.Text.Trim() != "" & txtRucProveedor3.TextLength <= 7)
                {


                    Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor3.Text, "RUC");
                    frm.ShowDialog();
                    if (frm._IdProveedor != null)
                    {

                        txtRazonSocialProveedor3.Text = frm._RazonSocial;
                        txtRucProveedor3.Text = frm._NroDocumento;
                        _objImportacionDto.v_IdClienteDoc3 = frm._IdProveedor;

                    }
                }


                else
                {
                    #region BusquedaCliente
                    if (txtRucProveedor3.TextLength == 8 | txtRucProveedor3.TextLength == 11)
                    {
                        string[] DatosCliente = new string[4];

                        DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor3.Text.Trim());
                        if (DatosCliente != null)
                        {
                            _objImportacionDto.v_IdClienteDoc3 = DatosCliente[0];
                            txtRazonSocialProveedor3.Text = DatosCliente[2].Trim();
                            txtRucProveedor3.Text = DatosCliente[3].Trim();


                        }
                        else // LLamo a Cliente rápido
                        {
                            #region Cliente Rapido

                            Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucProveedor3.Text.Trim(), "V");
                            frm.ShowDialog();
                            if (frm._Guardado == true)
                            {
                                txtRucProveedor3.Text = frm._NroDocumentoReturn;

                                DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor3.Text.Trim());
                                if (DatosCliente != null)
                                {

                                    _objImportacionDto.v_IdClienteDoc3 = DatosCliente[0];
                                    txtRucProveedor3.Text = DatosCliente[3];
                                    txtRazonSocialProveedor3.Text = DatosCliente[2];
                                }

                                else
                                {
                                    _objImportacionDto.v_IdClienteDoc3 = string.Empty;
                                    txtRucProveedor3.Clear();
                                    txtRazonSocialProveedor3.Clear();
                                }

                            }

                            #endregion
                        }

                    }
                    #endregion

                }

            }

        }

        private void btnBuscarCliente3_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtRazonSocialProveedor3.Text = frm._RazonSocial;
                txtRucProveedor3.Text = frm._NroDocumento;
                _objImportacionDto.v_IdClienteDoc3 = frm._IdProveedor;
            }
        }

        private void txtRucProveedor4_TextChanged(object sender, EventArgs e)
        {
            if (txtRucProveedor4.Text == string.Empty)
            {
                _objImportacionDto.v_IdClienteDoc4 = null;
                txtRazonSocialProveedor4.Clear();
            }
        }

        private void txtRucProveedor4_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (!txtRucProveedor4.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucProveedor4.Text.Trim() != "" & txtRucProveedor4.TextLength <= 7)
                {


                    Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor4.Text, "RUC");
                    frm.ShowDialog();
                    if (frm._IdProveedor != null)
                    {

                        txtRazonSocialProveedor4.Text = frm._RazonSocial;
                        txtRucProveedor4.Text = frm._NroDocumento;
                        _objImportacionDto.v_IdClienteDoc4 = frm._IdProveedor;

                    }
                }


                else
                {
                    #region BusquedaCliente
                    if (txtRucProveedor4.TextLength == 8 | txtRucProveedor4.TextLength == 11)
                    {
                        string[] DatosCliente = new string[4];

                        DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor4.Text.Trim());
                        if (DatosCliente != null)
                        {
                            _objImportacionDto.v_IdClienteDoc4 = DatosCliente[0];
                            txtRazonSocialProveedor4.Text = DatosCliente[2].Trim();
                            txtRucProveedor4.Text = DatosCliente[3].Trim();


                        }
                        else // LLamo a Cliente rápido
                        {
                            #region Cliente Rapido

                            Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucProveedor4.Text.Trim(), "V");
                            frm.ShowDialog();
                            if (frm._Guardado == true)
                            {
                                txtRucProveedor4.Text = frm._NroDocumentoReturn;

                                DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor4.Text.Trim());
                                if (DatosCliente != null)
                                {

                                    _objImportacionDto.v_IdClienteDoc4 = DatosCliente[0];
                                    txtRucProveedor4.Text = DatosCliente[3];
                                    txtRazonSocialProveedor4.Text = DatosCliente[2];
                                }

                                else
                                {
                                    _objImportacionDto.v_IdClienteDoc4 = string.Empty;
                                    txtRucProveedor4.Clear();
                                    txtRazonSocialProveedor4.Clear();
                                }

                            }

                            #endregion
                        }

                    }
                    #endregion

                }

            }


        }

        private void btnBuscarCliente4_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtRazonSocialProveedor4.Text = frm._RazonSocial;
                txtRucProveedor4.Text = frm._NroDocumento;
                _objImportacionDto.v_IdClienteDoc4 = frm._IdProveedor;
            }
        }

        private void txtTax_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroDecimal(txtTax, e);
        }

        private void txtProm_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroDecimal(txtProm, e);
        }

        private void txtTasaDespacho_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroDecimal(txtTasaDespacho, e);
        }

        private void txtPercepcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroDecimal(txtPercepcion, e);
        }
        private void chkDuplicar_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDuplicar.Checked == true)
            {

                if (grdDataProductos.ActiveRow != null)
                {
                    if (grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodigoInterno"].Value != null)
                    {

                        UltraGridRow row = grdDataProductos.DisplayLayout.Bands[0].AddNew();
                        grdDataProductos.Rows.Move(row, grdDataProductos.Rows.Count() - 1);
                        this.grdDataProductos.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        row.Cells["i_RegistroEstado"].Value = "Modificado";
                        //row.Cells["i_RegistroEstado"].Value = "Agregado";
                        row.Cells["i_RegistroTipo"].Value = "Temporal";


                    }
                }
                //else
                //{
                //    UltraGridRow row = grdDataProductos.DisplayLayout.Bands[0].AddNew();
                //    grdDataProductos.Rows.Move(row, grdDataProductos.Rows.Count() - 1);
                //    this.grdDataProductos.ActiveRowScrollRegion.ScrollRowIntoView(row);
                //    //row.Cells["i_RegistroEstado"].Value = "Agregado";
                //    row.Cells["i_RegistroEstado"].Value = "Modificado";
                //    row.Cells["i_RegistroTipo"].Value = "Temporal";



                //}

                UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["v_CodigoInterno"];
                this.grdDataProductos.ActiveCell = aCell;
                grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                grdDataProductos.Focus();
                grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);


            }
        }

        private void txtTotalValorFob_TextChanged(object sender, EventArgs e)
        {
            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();

            }
            CalcularSubTotal();
        }
        private void txtFlete_TextChanged(object sender, EventArgs e)
        {

            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();

            }

            CalcularSubTotal();

        }

        private void txtSeguro_TextChanged(object sender, EventArgs e)
        {
            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();

            }

            CalcularSubTotal();
        }

        private void txtAdValorem_TextChanged(object sender, EventArgs e)
        {

            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();

            }
            CalcularSubTotal();
        }

        private void txtTotalIgv_TextChanged(object sender, EventArgs e)
        {
            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();

            }
        }

        private void txtOtrosGastos_TextChanged(object sender, EventArgs e)
        {
            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();

            }
        }

        private void txtEnt11_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtEnt11, e);
        }

        private void txtEnt12_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtEnt12, e);
        }

        private void txtEnt21_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtEnt21, e);
        }

        private void txtEnt22_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtEnt22, e);
        }

        private void txtSerieDoc1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc1, e);
            e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Tab) && e.KeyChar != Convert.ToChar(Keys.Delete) && e.KeyChar != Convert.ToChar(Keys.Back);
            if (!e.Handled && e.KeyChar == 46)
            {
                e.Handled = true;
            }
        }

        private void txtCorrelativoDoc1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc1, e);
        }

        private void txtSerieDoc2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc2, e);
            e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Tab) && e.KeyChar != Convert.ToChar(Keys.Delete) && e.KeyChar != Convert.ToChar(Keys.Back);
            if (!e.Handled && e.KeyChar == 46)
            {
                e.Handled = true;
            }
        }

        private void txtCorrelativoDoc2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc2, e);
        }

        private void txtSerieDoc3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc3, e);
            e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Tab) && e.KeyChar != Convert.ToChar(Keys.Delete) && e.KeyChar != Convert.ToChar(Keys.Back);
            if (!e.Handled && e.KeyChar == 46)
            {
                e.Handled = true;
            }
        }

        private void txtCorrelativoDoc3_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc3, e);
        }

        private void txtSerieDoc4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc4, e);
            e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Tab) && e.KeyChar != Convert.ToChar(Keys.Delete) && e.KeyChar != Convert.ToChar(Keys.Back);
            if (!e.Handled && e.KeyChar == 46)
            {
                e.Handled = true;
            }
        }

        private void txtCorrelativoDoc4_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc4, e);
        }

        private void txtFlete_Validating(object sender, CancelEventArgs e)
        {

            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtFlete);

        }

        private void txtFlete_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtNumeroDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtNumeroDoc, "{0:00000000}");
        }

        private void txtSerieDoc1_Validated(object sender, EventArgs e)
        {
            // Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc1, "{0:0000}");


            if (txtSerieDoc1.Text != string.Empty)
            {
                int Leng = txtSerieDoc1.Text.Trim().Length, i = 2;
                string CadenaCeros = "0";
                if (Leng < 4)
                {
                    while (i <= (4 - Leng))
                    {
                        CadenaCeros = CadenaCeros + "0";
                        i = i + 1;
                    }
                    txtSerieDoc1.Text = CadenaCeros + txtSerieDoc1.Text.Trim();
                }
            }



        }

        private void txtCorrelativoDoc1_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc1, "{0:00000000}");
        }

        private void txtSerieDoc2_Validated(object sender, EventArgs e)
        {
            //Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc2, "{0:0000}");

            if (txtSerieDoc2.Text != string.Empty)
            {
                int Leng = txtSerieDoc2.Text.Trim().Length, i = 2;
                string CadenaCeros = "0";
                if (Leng < 4)
                {
                    while (i <= (4 - Leng))
                    {
                        CadenaCeros = CadenaCeros + "0";
                        i = i + 1;
                    }
                    txtSerieDoc2.Text = CadenaCeros + txtSerieDoc2.Text.Trim();
                }
            }
        }

        private void txtCorrelativoDoc2_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc2, "{0:00000000}");
        }
        private void txtSerieDoc3_Validated(object sender, EventArgs e)
        {
            //Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc3, "{0:0000}");

            if (txtSerieDoc3.Text != string.Empty)
            {
                int Leng = txtSerieDoc3.Text.Trim().Length, i = 2;
                string CadenaCeros = "0";
                if (Leng < 4)
                {
                    while (i <= (4 - Leng))
                    {
                        CadenaCeros = CadenaCeros + "0";
                        i = i + 1;
                    }
                    txtSerieDoc3.Text = CadenaCeros + txtSerieDoc3.Text.Trim();
                }
            }
        }

        private void txtCorrelativoDoc3_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc3, "{0:00000000}");
        }

        private void txtSerieDoc4_Validated(object sender, EventArgs e)
        {
            // Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc4, "{0:0000}");

            if (txtSerieDoc4.Text != string.Empty)
            {
                int Leng = txtSerieDoc4.Text.Trim().Length, i = 2;
                string CadenaCeros = "0";
                if (Leng < 4)
                {
                    while (i <= (4 - Leng))
                    {
                        CadenaCeros = CadenaCeros + "0";
                        i = i + 1;
                    }
                    txtSerieDoc4.Text = CadenaCeros + txtSerieDoc4.Text.Trim();
                }
            }

        }

        private void txtCorrelativoDoc4_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc4, "{0:00000000}");
        }

        private void cboIGV_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal pSoles = 0, pDolares = 0, Igv, pVentaAfectoSoles = 0, pVentaAfectoDolares = 0;
            Igv = cboIGV.Value.ToString() == "-1" ? 0 : decimal.Parse(cboIGV.Text);

            if (_ListaDetalleGastosDto_Grilla.Count() > 0)
            {
                foreach (var Fila in _ListaDetalleGastosDto_Grilla)
                {
                    if (Igv != 0)
                    {
                        Fila.d_Igv = Utils.Windows.DevuelveValorRedondeado(Fila.d_ValorVenta.Value * (Igv / 100), 2);
                    }
                    Fila.i_RegistroEstado = "Modificado";
                    switch (Fila.i_IdMoneda.Value.ToString())
                    {


                        case "1":   //Soles

                            Fila.d_ImporteSoles = Utils.Windows.DevuelveValorRedondeado(Fila.d_ValorVenta.Value + Fila.d_NAfectoDetraccion.Value + Fila.d_Igv.Value, 2);
                            Fila.d_ImporteDolares = Fila.d_TipoCambio == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(Fila.d_ImporteSoles.Value / Fila.d_TipoCambio.Value, 2);
                            pSoles = pSoles + decimal.Parse(Fila.d_ImporteSoles.Value.ToString());
                            pDolares = pDolares + decimal.Parse(Fila.d_ImporteDolares.ToString());

                            pVentaAfectoSoles = decimal.Parse(Fila.d_ValorVenta.Value.ToString()) + decimal.Parse(Fila.d_NAfectoDetraccion.Value.ToString()) + pVentaAfectoSoles;
                            pVentaAfectoDolares = Fila.d_TipoCambio.Value.ToString() == "0" ? pVentaAfectoDolares : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.d_ValorVenta.Value.ToString()) + decimal.Parse(Fila.d_NAfectoDetraccion.Value.ToString())) / decimal.Parse(Fila.d_TipoCambio.Value.ToString()), 2) + pVentaAfectoDolares;
                            pVentaAfectoSoles = Utils.Windows.DevuelveValorRedondeado(pVentaAfectoSoles, 2);


                            break;

                        case "2": //Dolares

                            //igv = (decimal.Parse(Fila.Cells["d_Igv"].Value.ToString()) * decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()) / 100);
                            Fila.d_ImporteDolares = decimal.Parse(Fila.d_ValorVenta.Value.ToString()) + decimal.Parse(Fila.d_NAfectoDetraccion.Value.ToString()) + decimal.Parse(Fila.d_Igv.Value.ToString());
                            Fila.d_ImporteSoles = decimal.Parse(Fila.d_ImporteDolares.ToString()) * decimal.Parse(Fila.d_TipoCambio.Value.ToString());
                            pSoles = pSoles + decimal.Parse(Fila.d_ImporteSoles.Value.ToString());
                            pDolares = pDolares + decimal.Parse(Fila.d_ImporteDolares.ToString());

                            pVentaAfectoDolares = decimal.Parse(Fila.d_ValorVenta.Value.ToString()) + decimal.Parse(Fila.d_NAfectoDetraccion.Value.ToString()) + pVentaAfectoDolares;
                            pVentaAfectoSoles = Fila.d_TipoCambio.Value.ToString() == "0" ? pVentaAfectoSoles : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.d_ValorVenta.Value.ToString()) + decimal.Parse(Fila.d_NAfectoDetraccion.Value.ToString())) * decimal.Parse(Fila.d_TipoCambio.Value.ToString()), 2) + pVentaAfectoSoles;
                            pVentaAfectoDolares = Utils.Windows.DevuelveValorRedondeado(pVentaAfectoDolares, 2);
                            break;
                    }
                    TotalSoles = pVentaAfectoSoles;
                    TotalDolares = pVentaAfectoDolares;

                    txtOtrosGastos.Text = cboMoneda.Value.ToString() == "1" ? TotalSoles.ToString("0.00") : TotalDolares.ToString("0.00");
                    //Fila.d_ImporteSoles = pTotalSoles;
                    //Fila.d_ImporteDolares = pTotalDolares;
                }

            }
        }
        private void cboSerieDoc_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboSerieDoc.Rows)
            {
                if (cboSerieDoc.Value == null) return;
                if (cboSerieDoc.Value.ToString() == "-1") cboSerieDoc.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboSerieDoc.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboSerieDoc.Text.ToUpper()))
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

        private void cboSerieDoc_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboSerieDoc.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboSerieDoc.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboSerieDoc.Text.ToUpper()))
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

        private void cboSerieDoc_Leave(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            List<GridKeyValueDTO> _ListadoSeries = new List<GridKeyValueDTO>();
            _ListadoSeries = _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 53, null);

            //if (strModo == "Nuevo")
            //{
            if (cboSerieDoc.Text.Trim() == "")
            {

                cboSerieDoc.Value = "-1";
            }
            else
            {
                var x = _ListadoSeries.Find(p => p.Id == cboSerieDoc.Value.ToString() | p.Id == cboSerieDoc.Text);
                if (x == null)
                {

                    cboSerieDoc.Value = "-1";
                }
            }

            //}
        }
        private void txtTipoCambio_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtSeguro_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }

        }

        private void txtAdValorem_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtSubTotal_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtTax_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtProm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtTotalIgv_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtTasaDespacho_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtIntereses_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtPercepcion_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtOtrosGastos_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtTipoCambioDoc1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtTipoCambioDoc2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtTipoCambioDoc3_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }

        private void txtTipoCambioDoc4_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;
                    break;

            }
        }
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            pnlVariosTiposImpresion.Visible = true;
            btnImprimir.Enabled = false;
            btnGuardar.Enabled = false;

        }

        private void txtSeguro_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtSeguro);
        }

        private void txtAdValorem_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtAdValorem);
        }

        private void txtSubTotal_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtSubTotal);
        }

        private void txtTax_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtTax);
        }

        private void txtProm_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtProm);
        }

        private void txtTotalIgv_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtTotalIgv);
        }

        private void txtTasaDespacho_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtTasaDespacho);
        }

        private void txtIntereses_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtIntereses);
        }

        private void txtPercepcion_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtPercepcion);
        }

        private void txtOtrosGastos_Validating(object sender, CancelEventArgs e)
        {
            //Utils.Windows.FormatoDecimalesCajasTexto(4, txtOtrosGastos);
        }
        #endregion
        #region GrillaFOB
        private void btnEliminarDetalleFob_Click(object sender, EventArgs e)
        {

            string pstrIdProveedorEliminar, NroFactura, NroPedido, serie, correlativo;
            pstrIdProveedorEliminar = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();
            serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
            correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
            NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
            NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
            foreach (UltraGridRow Fila in grdDataProductos.Rows)
            {
                string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                string NrofacturaProductos  = Fila.Cells["v_NroFactura"].Value == null ? null : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                string IdClienteProductos = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();
                if (IdClienteProductos == pstrIdProveedorEliminar && NroFactura == NrofacturaProductos && NroPedidoProductos == NroPedido)
                {
                    UltraMessageBox.Show("No se puede eliminar,éste proveedor se encuentra asociado a Detalle de Productos ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }

            if (grdDataFob.ActiveRow == null) return;

            if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _objImportaciondetallefobDto = new importaciondetallefobDto();
                    _objImportaciondetallefobDto.v_IdImportacionDetalleFob = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdImportacionDetalleFob"].Value.ToString();

                    _TempDetalleFobDto_Eliminar.Add(_objImportaciondetallefobDto);
                    grdDataFob.Rows[grdDataFob.ActiveRow.Index].Delete(false);
                }
            }
            else
            {
                grdDataFob.Rows[grdDataFob.ActiveRow.Index].Delete(false);
            }
            CalcularValoresDetalleGrillaFob();

        }
        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].EditorComponent = ucbDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }
        private void grdDataFob_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string NropedidoAnterior, Serie, Correlativo, FacturaAnterior;
            if (e.Cell.Column == null) return;
            switch (e.Cell.Column.Key)
            {

                case "v_CodCliente":
                    UtilizadoBefore = false;
                    string codigoCliente;
                    string idClienteAnterior = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();
                    Serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    Correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    FacturaAnterior = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + Serie + "-" + Correlativo;
                    NropedidoAnterior = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    codigoCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Text == null ? "" : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Text.Trim();

                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Text.Trim() != string.Empty || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value != null)
                    {
                        string[] DatosProveedor = new string[4];
                        DatosProveedor = _objImportacionBL.DevolverProveedorPorCodigo(ref objOperationResult, (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Text.Trim()));
                        if (DatosProveedor != null)
                        {

                            grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value = DatosProveedor[1];
                            grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_RazonSocial"].Value = DatosProveedor[2];
                            grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value = DatosProveedor[0];

                            foreach (UltraGridRow Fila in grdDataProductos.Rows)
                            {
                                string pstrIdProveedorAsignado = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                                string NrofacturaAsignado = Fila.Cells["v_NroFactura"].Value == null ? null : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                                string NropedidoAsignado = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                if (idClienteAnterior == pstrIdProveedorAsignado && FacturaAnterior == NrofacturaAsignado && NropedidoAnterior == NropedidoAsignado)
                                {
                                    Fila.Cells["v_CodCliente"].Value = DatosProveedor[1];
                                    Fila.Cells["v_IdCliente"].Value = DatosProveedor[0];
                                    Fila.Cells["i_RegistroEstado"].Value = "Modificado";

                                }

                            }
                            return;
                        }
                    }

                    Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(codigoCliente, "CODIGO");
                    frm.ShowDialog();
                    if (frm._IdProveedor != null)
                    {
                        grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value = frm._CodigoProveedor;
                        grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_RazonSocial"].Value = frm._RazonSocial;
                        grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value = frm._IdProveedor;

                        foreach (UltraGridRow Fila in grdDataProductos.Rows)
                        {
                            string pstrIdProveedorAsignado = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                            string NrofacturaAsignado = Fila.Cells["v_NroFactura"].Value == null ? null : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                            string NropedidoAsignado = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                            if (idClienteAnterior == pstrIdProveedorAsignado && FacturaAnterior == NrofacturaAsignado && NropedidoAnterior == NropedidoAsignado)
                            {
                                Fila.Cells["v_CodCliente"].Value = frm._CodigoProveedor;
                                Fila.Cells["v_IdCliente"].Value = frm._IdProveedor;
                                Fila.Cells["i_RegistroEstado"].Value = "Modificado";

                            }

                        }

                    }


                    break;
            }

        }
        private void grdDataFob_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }

        private void CalcularValoresFilaFob(UltraGridRow Fila)
        {
            if (Fila.Cells["d_ValorFob"].Value == null) { Fila.Cells["d_ValorFob"].Value = "0"; }
            CalcularTotales();

        }

        private void CalcularTotales()
        {

            decimal TotalFob = 0;
            if (grdDataFob.Rows.Count() > 0)
            {
                foreach (UltraGridRow Fila in grdDataFob.Rows)
                {
                    if (Fila.Cells["d_ValorFob"].Value == null) { Fila.Cells["d_ValorFob"].Value = "0"; }
                    TotalFob = TotalFob + decimal.Parse(Fila.Cells["d_ValorFob"].Value.ToString());
                }

            }
            txtTotalValorFob.Text = TotalFob.ToString("0.00");
        }
        private void grdDataFob_ClickCell(object sender, ClickCellEventArgs e)
        {

            string NroFactura, CodigoProveedor, NroPedido, idCliente, serie, correlativo, tipoDocumento;
            if (grdDataFob.ActiveCell == null || grdDataFob.ActiveCell.Column.Key == null) return;
            switch (grdDataFob.ActiveCell.Column.Key)
            {
                case "i_IdTipoDocumento":
                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value.ToString() == "-1" || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString() == string.Empty)
                    {
                        return;
                    }

                    tipoDocumento = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text.Trim();
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();
                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                        string NroFacturaG = Fila.Cells["v_NroFactura"].Value == null ? null : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        if (CodigoProveedor == Fila.Cells["v_CodCliente"].Value.ToString().Trim() && NroFactura == NroFacturaG && NroPedido == NroPedidoProductos)
                        {
                            NroFacturaG = NroFactura;
                            NroPedidoG = NroPedido;
                            CodigoProveedorG = CodigoProveedor;
                            IdClienteG = idCliente;

                        }


                    }
                    break;
            }


        }

        private void grdDataFob_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            string NroFactura, CodigoProveedor, NroPedido, idCliente, serie, correlativo, tipoDocumento;
            if (e.Cell.Column == null) return;
            switch (e.Cell.Column.Key)
            {
                case "i_IdTipoDocumento":
                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString() == string.Empty)
                    {
                        return;
                    }

                    tipoDocumento = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value.ToString();
                    string TipoDoc = _objImportacionBL.ObtenerSiglasDocumento(int.Parse(tipoDocumento));

                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = TipoDoc + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();
                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();


                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();

                        if (CodigoProveedor == CodProveeCelda && NroFactura == NroFactCelda && NroPedido == NroPedidoProductos)
                        {
                            NroFacturaG = NroFactura;
                            NroPedidoG = NroPedido;
                            CodigoProveedorG = CodigoProveedor;
                            IdClienteG = idCliente;

                        }


                    }
                    break;



                case "v_SerieDocumento":
                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value.ToString() == "-1" || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString() == string.Empty)
                    {
                        return;
                    }
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim().Trim();
                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();

                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();

                        if (CodigoProveedor == CodProveeCelda && NroFactura == NroFactCelda && NroPedido == NroPedidoProductos)
                        {
                            NroFacturaG = NroFactura;
                            NroPedidoG = NroPedido;
                            CodigoProveedorG = CodigoProveedor;
                            IdClienteG = idCliente;

                        }

                    }
                    break;

                case "v_CorrelativoDocumento":
                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value.ToString() == "-1" || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString() == string.Empty)
                    {
                        return;
                    }
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();
                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();


                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();

                        if (CodigoProveedor == CodProveeCelda && NroFactura == NroFactCelda && NroPedido == NroPedidoProductos)
                        {
                            NroFacturaG = NroFactura;
                            NroPedidoG = NroPedido;
                            CodigoProveedorG = CodigoProveedor;
                            IdClienteG = idCliente;

                        }


                    }
                    break;
                case "v_NroPedido":
                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value.ToString() == "-1" || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString() == string.Empty)
                    {
                        return;
                    }
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();
                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();


                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();

                        if (CodigoProveedor == CodProveeCelda && NroFactura == NroFactCelda && NroPedido == NroPedidoProductos)
                        {
                            NroFacturaG = NroFactura;
                            NroPedidoG = NroPedido;
                            CodigoProveedorG = CodigoProveedor;
                            IdClienteG = idCliente;

                        }


                    }
                    break;
                case "v_CodCliente":
                    if (UtilizadoBefore == false)
                    {
                        if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value.ToString() == "-1" || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null || grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString() == string.Empty)
                        {
                            return;
                        }
                        serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                        correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                        NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                        CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                        NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                        idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();
                        foreach (UltraGridRow Fila in grdDataProductos.Rows)
                        {
                            string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();


                            var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                            var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                            var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();

                            if (CodigoProveedor == CodProveeCelda && NroFactura == NroFactCelda && NroPedido == NroPedidoProductos)
                            {
                                NroFacturaG = NroFactura;
                                NroPedidoG = NroPedido;
                                CodigoProveedorG = CodigoProveedor;
                                IdClienteG = idCliente;
                            }

                        }


                    }
                    break;
            }

        }

        private void grdDataFob_AfterCellUpdate(object sender, CellEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string pstrIdProveedor = string.Empty, serie = string.Empty, correlativo = string.Empty;
            string NroFactura = string.Empty, CodigoProveedor = string.Empty, NroPedido = string.Empty, idCliente = string.Empty;
            if (grdDataFob.ActiveRow == null || e.Cell.Column == null) return;

            switch (e.Cell.Column.Key)
            {

                case "t_FechaEmision":
                    DateTime Fechas = Convert.ToDateTime(grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["t_FechaEmision"].Value.ToString());
                    grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["d_TipoCambio"].Value = decimal.Parse(_objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, Fechas.Date).ToString());
                    break;

                case "i_IdTipoDocumento":
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();

                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();
                        if (NroFacturaG == NroFactCelda && NroPedidoG == NroPedidoProductos && CodigoProveedorG == CodProveeCelda && IdClienteG == IdProveeCelda)
                        {
                            Fila.Cells["v_NroFactura"].Value = NroFactura;
                            Fila.Cells["v_NroPedido"].Value = NroPedido;
                            Fila.Cells["v_CodCliente"].Value = CodigoProveedor;
                            Fila.Cells["v_IdCliente"].Value = idCliente;
                            Fila.Cells["i_RegistroEstado"].Value = "Modificado";

                        }

                    }
                    break;
                case "v_CodCliente":
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();

                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();


                        if (NroFacturaG == NroFactCelda && NroPedidoG == NroPedidoProductos && CodigoProveedorG == CodProveeCelda && IdClienteG == IdProveeCelda)
                        {
                            Fila.Cells["v_NroFactura"].Value = NroFactura;
                            Fila.Cells["v_NroPedido"].Value = NroPedido;
                            Fila.Cells["v_CodCliente"].Value = CodigoProveedor;
                            Fila.Cells["v_IdCliente"].Value = idCliente;
                            Fila.Cells["i_RegistroEstado"].Value = "Modificado";

                        }

                    }
                    break;

                case "v_CorrelativoDocumento":
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();

                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();


                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();
                        if (NroFacturaG == NroFactCelda && NroPedidoG == NroPedidoProductos && CodigoProveedorG == CodProveeCelda && IdClienteG == IdProveeCelda)
                        {
                            Fila.Cells["v_NroFactura"].Value = NroFactura;
                            Fila.Cells["v_NroPedido"].Value = NroPedido;
                            Fila.Cells["v_CodCliente"].Value = CodigoProveedor;
                            Fila.Cells["v_IdCliente"].Value = idCliente;
                            Fila.Cells["i_RegistroEstado"].Value = "Modificado";

                        }

                    }

                    break;

                case "v_SerieDocumento":
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();

                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();

                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();


                        if (NroFacturaG == NroFactCelda && NroPedidoG == NroPedidoProductos && CodigoProveedorG == CodProveeCelda && IdClienteG == IdProveeCelda)
                        {
                            Fila.Cells["v_NroFactura"].Value = NroFactura;
                            Fila.Cells["v_NroPedido"].Value = NroPedido;
                            Fila.Cells["v_CodCliente"].Value = CodigoProveedor;
                            Fila.Cells["v_IdCliente"].Value = idCliente;
                            Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                        }

                    }

                    break;

                case "v_NroPedido":
                    serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString().Trim();
                    correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString().Trim();
                    NroFactura = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["i_IdTipoDocumento"].Text + " " + serie + "-" + correlativo;
                    CodigoProveedor = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString().Trim();
                    NroPedido = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_NroPedido"].Value.ToString().Trim();
                    idCliente = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value == null ? null : grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString().Trim();

                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        string NroPedidoProductos = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                        var NroFactCelda = Fila.Cells["v_NroFactura"].Value == null ? "" : Fila.Cells["v_NroFactura"].Value.ToString().Trim();
                        var IdProveeCelda = Fila.Cells["v_IdCliente"].Value == null ? "" : Fila.Cells["v_IdCliente"].Value.ToString().Trim();
                        var CodProveeCelda = Fila.Cells["v_CodCliente"].Value == null ? "" : Fila.Cells["v_CodCliente"].Value.ToString().Trim();

                        if (NroFacturaG == NroFactCelda && NroPedidoG == NroPedidoProductos && CodigoProveedorG == CodProveeCelda && IdClienteG == IdProveeCelda)
                        {
                            Fila.Cells["v_NroFactura"].Value = NroFactura;
                            Fila.Cells["v_NroPedido"].Value = NroPedido;
                            Fila.Cells["v_CodCliente"].Value = CodigoProveedor;
                            Fila.Cells["v_IdCliente"].Value = idCliente;
                            Fila.Cells["i_RegistroEstado"].Value = "Modificado";

                        }

                    }
                    break;

            }
        }

        private void grdDataFob_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.grdDataFob.ActiveCell == null) return;

            if (this.grdDataFob.ActiveCell.Column.Key != "i_IdTipoDocumento")
            {

                switch (e.KeyCode)
                {
                    case Keys.Up:
                        grdDataFob.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataFob.PerformAction(UltraGridAction.AboveCell, false, false);
                        e.Handled = true;
                        grdDataFob.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Down:
                        grdDataFob.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataFob.PerformAction(UltraGridAction.BelowCell, false, false);
                        e.Handled = true;
                        grdDataFob.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Right:
                        grdDataFob.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataFob.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdDataFob.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdDataFob.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataFob.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdDataFob.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Enter:
                        DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdDataFob.ActiveCell);
                        grdDataFob_DoubleClickCell(sender, eventos);
                        e.Handled = true;
                        break;

                }

            }
            else
            {

                switch (e.KeyCode)
                {
                    case Keys.Right:
                        grdDataFob.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataFob.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdDataFob.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdDataFob.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataFob.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdDataFob.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                }


            }



        }

        private void grdDataFob_CellChange(object sender, CellEventArgs e)
        {
            grdDataFob.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";


        }
        private void grdDataFob_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdDataFob.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grdDataFob.ActiveCell.Column.Key)
                {
                    case "d_ValorFob":

                        Celda = grdDataFob.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;
                    case "d_TipoCambio":
                        Celda = grdDataFob.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "v_SerieDocumento":
                        e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Tab) && e.KeyChar != Convert.ToChar(Keys.Delete) && e.KeyChar != Convert.ToChar(Keys.Back);
                        if (!e.Handled && e.KeyChar == 46)
                        {
                            e.Handled = true;
                        }
                        break;

                    case "v_CorrelativoDocumento":
                        e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Tab) && e.KeyChar != Convert.ToChar(Keys.Delete) && e.KeyChar != Convert.ToChar(Keys.Back);
                        if (!e.Handled && e.KeyChar == 46)
                        {
                            e.Handled = true;
                        }
                        break;

                }
            }
        }
        private void grdDataFob_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));


            if (row == null)
            {



                btnEliminarDetalleFob.Enabled = false;

            }
            else
            {


                btnEliminarDetalleFob.Enabled = true;
            }




        }
        #endregion

        #region CRUD
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvImportacion.Validate(true, false).IsValid)
            {
                if (cboDocumento.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de documento  válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cboDocumento.Focus();

                    return;
                }
                if (cboSerieDoc.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Por Favor ingrese la Serie del Documento  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cboSerieDoc.Focus();
                    return;
                }

                if (decimal.Parse(txtNumeroDoc.Text.Trim()) <= 0)
                {

                    UltraMessageBox.Show("Por Favor ingrese un Número Documento Válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNumeroDoc.Focus();
                    return;

                }
                if (txtTipoCambio.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                else if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }

                if (cboSerieDoc.Value.ToString() != "-1" && cboDocumento.Value.ToString() != "-1" && txtNumeroDoc.Text != string.Empty)
                {

                    if (_objImportacionBL.ValidarNumeroRegistro(int.Parse(cboDocumento.Value.ToString()), int.Parse(cboSerieDoc.Value.ToString()), txtNumeroDoc.Text.Trim(), _objImportacionDto.v_IdImportacion))
                    {
                        UltraMessageBox.Show("Este Documento ya ha sido Registrado ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtNumeroDoc.Focus();
                        return;
                    }

                }

                else if (decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }


                if (decimal.Parse(txtFlete.Text) > 0)
                {

                    if (cboDocumento1.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Tipo de Documento Válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboDocumento1.Focus();
                        return;

                    }

                    if (txtSerieDoc1.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de serie válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSerieDoc1.Focus();
                        return;
                    }
                    if (txtCorrelativoDoc1.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de correlativo válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtCorrelativoDoc1.Focus();
                        return;
                    }
                    if (txtTipoCambioDoc1.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de correlativo válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambioDoc1.Focus();
                        return;
                    }
                    if (decimal.Parse(txtTipoCambioDoc1.Text.ToString()) <= 0)
                    {

                        UltraMessageBox.Show("Por favor ingrese un tipo de cambio  válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambioDoc1.Focus();
                        return;
                    }
                    if (_objImportacionDto.v_IdClienteDoc1 == null)
                    {
                        UltraMessageBox.Show("Por favor ingrese un Proveedor válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtRucProveedor1.Focus();
                        return;
                    }

                }
                if (decimal.Parse(txtSeguro.Text) > 0)
                {
                    if (cboDocumento2.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Tipo de Documento Válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboDocumento2.Focus();
                        return;

                    }

                    if (txtSerieDoc2.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de serie válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSerieDoc2.Focus();
                        return;
                    }
                    if (txtCorrelativoDoc2.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de correlativo válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtCorrelativoDoc2.Focus();
                        return;
                    }
                    if (txtTipoCambioDoc2.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de correlativo válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambioDoc2.Focus();
                        return;
                    }
                    if (decimal.Parse(txtTipoCambioDoc2.Text.ToString()) <= 0)
                    {

                        UltraMessageBox.Show("Por favor ingrese un tipo de cambio  válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambioDoc2.Focus();
                        return;
                    }
                    if (_objImportacionDto.v_IdClienteDoc2 == null)
                    {
                        UltraMessageBox.Show("Por favor ingrese un Proveedor válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtRucProveedor2.Focus();
                        return;
                    }

                }

                if (decimal.Parse(txtAdValorem.Text) > 0)
                {

                    if (cboDocumento3.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Tipo de Documento Válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboDocumento3.Focus();
                        return;

                    }

                    if (txtSerieDoc3.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de serie válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSerieDoc3.Focus();
                        return;
                    }
                    if (txtCorrelativoDoc3.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de correlativo válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtCorrelativoDoc3.Focus();
                        return;
                    }
                    if (txtTipoCambioDoc3.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de correlativo válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambioDoc3.Focus();
                        return;
                    }
                    if (decimal.Parse(txtTipoCambioDoc3.Text.ToString()) <= 0)
                    {

                        UltraMessageBox.Show("Por favor ingrese un tipo de cambio  válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambioDoc3.Focus();
                        return;
                    }
                    if (_objImportacionDto.v_IdClienteDoc3 == null)
                    {
                        UltraMessageBox.Show("Por favor ingrese un Proveedor válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtRucProveedor3.Focus();
                        return;
                    }

                }


                if (decimal.Parse(txtTotalIgv.Text) > 0)
                {


                    if (cboDocumento4.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Tipo de Documento Válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboDocumento4.Focus();
                        return;

                    }

                    if (txtSerieDoc4.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de serie válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSerieDoc4.Focus();
                        return;
                    }
                    if (txtCorrelativoDoc4.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de correlativo válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtCorrelativoDoc4.Focus();
                        return;
                    }
                    if (txtTipoCambioDoc4.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese el número de correlativo válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambioDoc4.Focus();
                        return;
                    }
                    else if (decimal.Parse(txtTipoCambioDoc4.Text.ToString()) <= 0)
                    {

                        UltraMessageBox.Show("Por favor ingrese un tipo de cambio  válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambioDoc4.Focus();
                        return;
                    }

                    if (_objImportacionDto.v_IdClienteDoc4 == null)
                    {
                        UltraMessageBox.Show("Por favor ingrese un Proveedor válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtRucProveedor4.Focus();
                        return;
                    }
                }

                if (rbSiSeguro.Checked == true && decimal.Parse(txtSeguro.Text) <= 0)
                {
                    UltraMessageBox.Show("Por favor ingrese valor válido para el Seguro", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSeguro.Focus();
                    return;

                }

                if (rbSiAdValorem.Checked == true && decimal.Parse(txtAdValorem.Text) <= 0)
                {
                    UltraMessageBox.Show("Por favor ingrese valor válido para el AdValorem", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAdValorem.Focus();
                    return;

                }

                if (ValidaCamposNulosVacios() == true)
                {

                    if (decimal.Parse(txtTotalValorFob.Text.ToString()) < 0)
                    {
                        if (UltraMessageBox.Show(" Es necesario un valor para Valor FOB de la factura " + "\n" + " ¿Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }

                    }

                    if (!ValidarProveedores())
                    {

                        if (UltraMessageBox.Show("Algunos proveedores de 'Valor FOB' de la factura no están asignados en  Detalles de Productos" + "\n" + " ¿Desea Continuar Guardando?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1)
                    {

                        if (!ValidarNroPedidos())
                        {
                            if (UltraMessageBox.Show("Algunos Nro Pedidos de 'Valor FOB' de la factura no están asignados en  Detalles de Productos" + "\n" + " ¿Desea Continuar Guardando?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                return;
                            }
                        }

                    }
                    //if (ValidarCuentasGeneracionLibroDiario())
                    //{
                    if (ValidarCuentasGeneracionLibroDiario())
                    {
                        if (_Mode == "New")
                        {
                            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                            {

                                while (_objImportacionBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                                {
                                    txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                                }
                                _objImportacionDto.v_Periodo = txtPeriodo.Text;
                                _objImportacionDto.v_Mes = txtMes.Text;
                                _objImportacionDto.v_Correlativo = txtCorrelativo.Text;
                                _objImportacionDto.i_Igv = cboVia.Value == null ? -1 : int.Parse(cboIGV.Value.ToString());
                                _objImportacionDto.i_IdTipoDocumento = cboDocumento.Value == null ? -1 : int.Parse(cboDocumento.Value.ToString());
                                _objImportacionDto.i_IdSerieDocumento = cboSerieDoc.Value == null ? -1 : int.Parse(cboSerieDoc.Value.ToString());
                                _objImportacionDto.v_CorrelativoDocumento = txtNumeroDoc.Text;
                                _objImportacionDto.i_IdTipoVia = cboVia.Value == null ? -1 : int.Parse(cboVia.Value.ToString());
                                _objImportacionDto.i_IdDestino = cboDestino.Value == null ? -1 : int.Parse(cboDestino.Value.ToString());
                                _objImportacionDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                                _objImportacionDto.t_FechaEmision = dtpFechaEmision.Value.Date;
                                _objImportacionDto.t_FechaPagoVencimiento = dtpFechaVencimiento.Value.Date;
                                _objImportacionDto.d_TipoCambio = txtTipoCambio.Text == null ? 0 : decimal.Parse(txtTipoCambio.Text);
                                _objImportacionDto.d_TotalValorFob = txtTotalValorFob.Text == null ? 0 : decimal.Parse(txtTotalValorFob.Text);
                                _objImportacionDto.i_IdEstablecimiento = cboEstablecimiento.Value == null ? -1 : int.Parse(cboEstablecimiento.Value.ToString());
                                _objImportacionDto.v_NroOrden = txtNroOrden.Text;
                                _objImportacionDto.v_Bl = txtBl.Text;
                                _objImportacionDto.t_FechaArrivo = dtpFechaArrivo.Value.Date;
                                _objImportacionDto.t_FechaLLegada = dtpFechaLlegada.Value.Date;
                                _objImportacionDto.i_IdAgencia = cboAgencia.Value == null ? -1 : int.Parse(cboAgencia.Value.ToString());
                                _objImportacionDto.v_Terminal = txtTerminal.Text;
                                _objImportacionDto.v_Ent1Serie = txtEnt11.Text;
                                _objImportacionDto.v_Ent1Correlativo = txtEnt12.Text;
                                _objImportacionDto.v_Ent2Serie = txtEnt21.Text;
                                _objImportacionDto.v_Ent2Correlativo = txtEnt22.Text;
                                _objImportacionDto.i_IdAlmacen = cboAlmacen.Value == null ? -1 : int.Parse(cboAlmacen.Value.ToString());
                                _objImportacionDto.d_Utilidad = txtUtilidad.Text == string.Empty ? 0 : decimal.Parse(txtUtilidad.Text);
                                _objImportacionDto.d_Flete = txtFlete.Text == string.Empty ? 0 : decimal.Parse(txtFlete.Text);
                                _objImportacionDto.d_TotalValorFob = txtTotalValorFob.Text == string.Empty ? 0 : decimal.Parse(txtTotalValorFob.Text);
                                _objImportacionDto.i_IdTipoDocumento1 = cboDocumento1.Value == null ? -1 : int.Parse(cboDocumento1.Value.ToString());
                                _objImportacionDto.v_SerieDocumento1 = txtSerieDoc1.Text;
                                _objImportacionDto.v_CorrelativoDocumento1 = txtCorrelativoDoc1.Text;
                                _objImportacionDto.t_FechaEmisionDoc1 = dtpFechaDoc1.Value.Date;
                                _objImportacionDto.d_TipoCambioDoc1 = txtTipoCambioDoc1.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambioDoc1.Text);
                                //_objImportacionDto.d_ IDCliente se llena cuando se Busca
                                _objImportacionDto.i_PagaSeguro = rbSiSeguro.Checked == true ? 1 : 0;
                                _objImportacionDto.d_PagoSeguro = txtSeguro.Text == string.Empty ? 0 : decimal.Parse(txtSeguro.Text);
                                _objImportacionDto.i_IdTipoDocumento2 = cboDocumento2.Value == null ? -1 : int.Parse(cboDocumento2.Value.ToString());
                                _objImportacionDto.v_SerieDocumento2 = txtSerieDoc2.Text;
                                _objImportacionDto.v_CorrelativoDocumento2 = txtCorrelativoDoc2.Text;
                                _objImportacionDto.t_FechaEmisionDoc2 = dtpFechaDoc2.Value.Date;
                                _objImportacionDto.d_TipoCambioDoc2 = txtTipoCambioDoc2.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambioDoc2.Text);
                                //_objImportacionDto.d_ IDCliente se llena cuando se Busca
                                _objImportacionDto.i_AdValorem = rbSiAdValorem.Checked == true ? 1 : 0;
                                _objImportacionDto.d_AdValorem = txtAdValorem.Text == string.Empty ? 0 : decimal.Parse(txtAdValorem.Text);
                                _objImportacionDto.i_IdTipoDocumento3 = cboDocumento3.Value == null ? -1 : int.Parse(cboDocumento3.Value.ToString());
                                _objImportacionDto.v_SerieDocumento3 = txtSerieDoc3.Text;
                                _objImportacionDto.v_CorrelativoDocumento3 = txtCorrelativoDoc3.Text;
                                _objImportacionDto.t_FechaEmisionDoc3 = dtpFechaDoc3.Value.Date;
                                _objImportacionDto.d_TipoCambioDoc3 = txtTipoCambioDoc3.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambioDoc3.Text);
                                _objImportacionDto.d_SubTotal = txtSubTotal.Text == string.Empty ? 0 : decimal.Parse(txtSubTotal.Text);
                                _objImportacionDto.d_Tax = txtTax.Text == string.Empty ? 0 : decimal.Parse(txtTax.Text);
                                _objImportacionDto.i_IdTipoDocumento4 = cboDocumento4.Value == null ? 0 : int.Parse(cboDocumento4.Value.ToString());
                                _objImportacionDto.v_SerieDocumento4 = txtSerieDoc4.Text;
                                _objImportacionDto.v_CorrelativoDoc4 = txtCorrelativoDoc4.Text;
                                _objImportacionDto.t_FechaEmisionDoc4 = dtpFechaDoc4.Value.Date;
                                _objImportacionDto.d_TipoCambioDoc4 = txtTipoCambioDoc4.Text == null ? 0 : decimal.Parse(txtTipoCambioDoc4.Text);
                                //_objImportacionDto.d_ IDCliente se llena cuando se Busca
                                _objImportacionDto.d_Prom = txtProm.Text == string.Empty ? 0 : decimal.Parse(txtProm.Text);
                                _objImportacionDto.d_TasaDespacho = txtTasaDespacho.Text == string.Empty ? 0 : decimal.Parse(txtTasaDespacho.Text);
                                _objImportacionDto.d_Percepcion = txtPercepcion.Text == string.Empty ? 0 : decimal.Parse(txtPercepcion.Text);
                                _objImportacionDto.i_IdMoneda = cboMoneda.Value == null ? -1 : int.Parse(cboMoneda.Value.ToString());
                                _objImportacionDto.d_Igv = txtTotalIgv.Text == string.Empty ? 0 : decimal.Parse(txtTotalIgv.Text);
                                _objImportacionDto.d_Intereses = txtIntereses.Text == string.Empty ? 0 : decimal.Parse(txtIntereses.Text);
                                _objImportacionDto.d_OtrosGastos = txtOtrosGastos.Text == string.Empty ? 0 : decimal.Parse(txtOtrosGastos.Text);
                                _objImportacionDto.i_IdEstado = cboEstado.Value == null ? -1 : int.Parse(cboEstado.Value.ToString());
                                _objImportacionDto.d_ValorFob = txtValorFob.Text == string.Empty ? 0 : decimal.Parse(txtValorFob.Text);
                                _objImportacionDto.d_TotalSeguro = txtSeguroResultado.Text == string.Empty ? 0 : decimal.Parse(txtSeguroResultado.Text);
                                _objImportacionDto.d_TotalIgv = txtIgvResultado.Text == string.Empty ? 0 : decimal.Parse(txtIgvResultado.Text);
                                _objImportacionDto.d_TotalFlete = txtFleteResultado.Text == string.Empty ? 0 : decimal.Parse(txtFleteResultado.Text);
                                _objImportacionDto.d_TotalAdValorem = txtAdValoremResultado.Text == string.Empty ? 0 : decimal.Parse(txtAdValoremResultado.Text);
                                _objImportacionDto.d_TotalOtrosGastos = txtOtrosGastosResultado.Text == string.Empty ? 0 : decimal.Parse(txtOtrosGastosResultado.Text);
                                _objImportacionDto.i_IdTipoDocRerefencia = int.Parse(cboDocumentoReferencia.Value.ToString());
                                _objImportacionDto.v_NumeroDocRerefencia = txtNumeroDocumentoReferencia.Text;
                                LlenarTemporalesFobProducto();
                                newIdImportacion = _objImportacionBL.InsertarImportacion(ref objOperationResult, _objImportacionDto, Globals.ClientSession.GetAsList(), _TempDetalleFobDto_Agregar, _TempDetalleProdDto_Agregar, _TempDetalleGastosDto_Agregar);
                            }

                        }
                        else if (_Mode == "Edit")
                        {


                            if (_objImportacionBL.ConsultarSiTieneTesorerias(_objImportacionBL.NroDiario(_objImportacionDto.v_IdImportacion)) || _objImportacionBL.ConsultaSiTieneTesoreriaDetalleGastos(_objImportacionDto.v_IdImportacion))
                            {
                                if (UltraMessageBox.Show("Éste documento  ya  tiene un pago en  tesoreria .¿Desea aún editarlo?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)

                                    return;
                            }
                            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                            {
                                LlenarTemporalesFobProducto();
                                if (bActualizarNotaIngreso)
                                {
                                    ActualizaNotadeIngreso(ref objOperationResult);

                                }

                                if (objOperationResult.Success == 1)
                                {
                                    _objImportacionDto.v_Periodo = txtPeriodo.Text;
                                    _objImportacionDto.v_Mes = txtMes.Text;
                                    _objImportacionDto.v_Correlativo = txtCorrelativo.Text;
                                    _objImportacionDto.i_Igv = cboIGV.Value == null ? -1 : int.Parse(cboIGV.Value.ToString());
                                    _objImportacionDto.i_IdTipoDocumento = cboDocumento.Value == null ? -1 : int.Parse(cboDocumento.Value.ToString());
                                    _objImportacionDto.i_IdSerieDocumento = cboSerieDoc.Value == null ? -1 : int.Parse(cboSerieDoc.Value.ToString());
                                    _objImportacionDto.v_CorrelativoDocumento = txtNumeroDoc.Text;
                                    _objImportacionDto.i_IdTipoVia = cboVia.Value == null ? -1 : int.Parse(cboVia.Value.ToString());
                                    _objImportacionDto.i_IdDestino = cboDestino.Value == null ? -1 : int.Parse(cboDestino.Value.ToString());
                                    _objImportacionDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                                    _objImportacionDto.t_FechaEmision = dtpFechaEmision.Value.Date;
                                    _objImportacionDto.t_FechaPagoVencimiento = dtpFechaVencimiento.Value.Date;
                                    _objImportacionDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                                    _objImportacionDto.i_IdEstablecimiento = cboEstablecimiento == null ? -1 : int.Parse(cboEstablecimiento.Value.ToString());
                                    _objImportacionDto.v_NroOrden = txtNroOrden.Text;
                                    _objImportacionDto.v_Bl = txtBl.Text;
                                    _objImportacionDto.t_FechaArrivo = dtpFechaArrivo.Value.Date;
                                    _objImportacionDto.t_FechaLLegada = dtpFechaLlegada.Value.Date;
                                    _objImportacionDto.i_IdAgencia = cboAgencia.Value == null ? -1 : int.Parse(cboAgencia.Value.ToString());
                                    _objImportacionDto.v_Terminal = txtTerminal.Text;
                                    _objImportacionDto.v_Ent1Serie = txtEnt11.Text;
                                    _objImportacionDto.v_Ent1Correlativo = txtEnt12.Text;
                                    _objImportacionDto.v_Ent2Serie = txtEnt21.Text;
                                    _objImportacionDto.v_Ent2Correlativo = txtEnt22.Text;
                                    _objImportacionDto.i_IdAlmacen = cboAlmacen.Value == null ? -1 : int.Parse(cboAlmacen.Value.ToString());
                                    _objImportacionDto.d_Utilidad = txtUtilidad.Text == string.Empty ? 0 : decimal.Parse(txtUtilidad.Text);
                                    _objImportacionDto.d_Flete = txtFlete.Text == string.Empty ? 0 : decimal.Parse(txtFlete.Text);
                                    _objImportacionDto.d_TotalValorFob = txtTotalValorFob.Text == string.Empty ? 0 : decimal.Parse(txtTotalValorFob.Text);
                                    _objImportacionDto.i_IdTipoDocumento1 = cboDocumento1.Value == null ? -1 : int.Parse(cboDocumento1.Value.ToString());
                                    _objImportacionDto.v_SerieDocumento1 = txtSerieDoc1.Text;
                                    _objImportacionDto.v_CorrelativoDocumento1 = txtCorrelativoDoc1.Text;
                                    _objImportacionDto.t_FechaEmisionDoc1 = dtpFechaDoc1.Value.Date;
                                    _objImportacionDto.d_TipoCambioDoc1 = txtTipoCambioDoc1.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambioDoc1.Text);
                                    _objImportacionDto.i_PagaSeguro = rbSiSeguro.Checked == true ? 1 : 0;
                                    _objImportacionDto.d_PagoSeguro = txtSeguro.Text == string.Empty ? 0 : decimal.Parse(txtSeguro.Text);
                                    _objImportacionDto.i_IdTipoDocumento2 = int.Parse(cboDocumento2.Value.ToString());
                                    _objImportacionDto.v_SerieDocumento2 = txtSerieDoc2.Text;
                                    _objImportacionDto.v_CorrelativoDocumento2 = txtCorrelativoDoc2.Text;
                                    _objImportacionDto.t_FechaEmisionDoc2 = dtpFechaDoc2.Value.Date;
                                    _objImportacionDto.d_TipoCambioDoc2 = txtTipoCambioDoc2.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambioDoc2.Text);
                                    _objImportacionDto.i_AdValorem = rbSiAdValorem.Checked == true ? 1 : 0;
                                    _objImportacionDto.d_AdValorem = txtAdValorem.Text == string.Empty ? 0 : decimal.Parse(txtAdValorem.Text);
                                    _objImportacionDto.i_IdTipoDocumento3 = cboDocumento3.Value == null ? -1 : int.Parse(cboDocumento3.Value.ToString());
                                    _objImportacionDto.v_SerieDocumento3 = txtSerieDoc3.Text;
                                    _objImportacionDto.v_CorrelativoDocumento3 = txtCorrelativoDoc3.Text;
                                    _objImportacionDto.t_FechaEmisionDoc3 = dtpFechaDoc3.Value.Date;
                                    _objImportacionDto.d_TipoCambioDoc3 = txtTipoCambioDoc3.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambioDoc3.Text);
                                    _objImportacionDto.d_SubTotal = txtSubTotal.Text == string.Empty ? 0 : decimal.Parse(txtSubTotal.Text);
                                    _objImportacionDto.d_Tax = txtTax.Text == string.Empty ? 0 : decimal.Parse(txtTax.Text);
                                    _objImportacionDto.d_Igv = txtTotalIgv.Text == string.Empty ? 0 : decimal.Parse(txtTotalIgv.Text);
                                    _objImportacionDto.i_IdTipoDocumento4 = cboDocumento4.Value == null ? -1 : int.Parse(cboDocumento4.Value.ToString());
                                    _objImportacionDto.v_SerieDocumento4 = txtSerieDoc4.Text;
                                    _objImportacionDto.v_CorrelativoDoc4 = txtCorrelativoDoc4.Text;
                                    _objImportacionDto.t_FechaEmisionDoc4 = dtpFechaDoc4.Value.Date;
                                    _objImportacionDto.d_TipoCambioDoc4 = txtTipoCambioDoc4.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambioDoc4.Text);
                                    _objImportacionDto.d_Prom = txtProm.Text == string.Empty ? 0 : decimal.Parse(txtProm.Text);
                                    _objImportacionDto.d_TasaDespacho = txtTasaDespacho.Text == string.Empty ? 0 : decimal.Parse(txtTasaDespacho.Text);
                                    _objImportacionDto.d_Percepcion = txtPercepcion.Text == string.Empty ? 0 : decimal.Parse(txtPercepcion.Text);
                                    _objImportacionDto.i_IdMoneda = cboMoneda.Value == null ? -1 : int.Parse(cboMoneda.Value.ToString());
                                    _objImportacionDto.d_Intereses = txtIntereses.Text == string.Empty ? 0 : decimal.Parse(txtIntereses.Text);
                                    _objImportacionDto.d_OtrosGastos = txtOtrosGastos.Text == string.Empty ? 0 : decimal.Parse(txtOtrosGastos.Text);
                                    _objImportacionDto.i_IdEstado = cboEstado.Value == null ? -1 : int.Parse(cboEstado.Value.ToString());
                                    // Totales Pie Formulario
                                    _objImportacionDto.d_ValorFob = txtValorFob.Text == string.Empty ? 0 : decimal.Parse(txtValorFob.Text);
                                    _objImportacionDto.d_TotalSeguro = txtSeguroResultado.Text == string.Empty ? 0 : decimal.Parse(txtSeguroResultado.Text);
                                    _objImportacionDto.d_TotalIgv = txtIgvResultado.Text == string.Empty ? 0 : decimal.Parse(txtIgvResultado.Text);
                                    _objImportacionDto.d_TotalFlete = txtFleteResultado.Text == string.Empty ? 0 : decimal.Parse(txtFleteResultado.Text);
                                    _objImportacionDto.d_TotalAdValorem = txtAdValoremResultado.Text == string.Empty ? 0 : decimal.Parse(txtAdValoremResultado.Text);
                                    _objImportacionDto.d_TotalOtrosGastos = txtOtrosGastosResultado.Text == string.Empty ? 0 : decimal.Parse(txtOtrosGastosResultado.Text);
                                    _objImportacionDto.i_IdTipoDocRerefencia = int.Parse(cboDocumentoReferencia.Value.ToString());
                                    _objImportacionDto.v_NumeroDocRerefencia = txtNumeroDocumentoReferencia.Text;

                                    newIdImportacion = _objImportacionBL.ActualizarImportacion(ref objOperationResult, _objImportacionDto, Globals.ClientSession.GetAsList(), _TempDetalleFobDto_Agregar, _TempDetalleFobDto_Modificar, _TempDetalleFobDto_Eliminar, _TempDetalleProdDto_Agregar, _TempDetalleProdDto_Modificar, _TempDetalleProdDto_Eliminar, _TempDetalleGastosDto_Agregar, _TempDetalleGastosDto_Modificar, _TempDetalleGastosDto_Eliminar);

                                }
                                //else
                                //{
                                //    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //    _TempDetalleFobDto_Agregar = new List<importaciondetallefobDto>();
                                //    _TempDetalleFobDto_Modificar = new List<importaciondetallefobDto>();
                                //    _TempDetalleFobDto_Eliminar = new List<importaciondetallefobDto>();
                                //    _TempDetalleProdDto_Agregar = new List<importaciondetalleproductoDto>();
                                //    _TempDetalleProdDto_Modificar = new List<importaciondetalleproductoDto>();
                                //    _TempDetalleProdDto_Eliminar = new List<importaciondetalleproductoDto>();
                                //    return;
                                //}
                            }
                        }
                        //}

                        if (objOperationResult.Success == 1)
                        {
                            strModo = "Guardado";
                            EdicionBarraNavegacion(true);
                            //strIdPedido = _objPedido.v_IdPedido; ....y InsertaPedido y ActualizaPedido no devolvia string
                            strIdImportacion = newIdImportacion;
                            ObtenerListadoImportacion(txtPeriodo.Text, txtMes.Text);
                            _pstrIdMovimiento_Nuevo = _objImportacionDto.v_IdImportacion;
                            _idImportacion = _pstrIdMovimiento_Nuevo;
                            UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (Modulo == "C")
                            {
                                btnImprimir.Enabled = true;
                            }
                            else
                            {

                                btnImprimir.Enabled = _btnImprimir;
                            }

                            _TempDetalleFobDto_Agregar = new List<importaciondetallefobDto>();
                            _TempDetalleFobDto_Modificar = new List<importaciondetallefobDto>();
                            _TempDetalleFobDto_Eliminar = new List<importaciondetallefobDto>();
                            _TempDetalleProdDto_Agregar = new List<importaciondetalleproductoDto>();
                            _TempDetalleProdDto_Modificar = new List<importaciondetalleproductoDto>();
                            _TempDetalleProdDto_Eliminar = new List<importaciondetalleproductoDto>();

                            _TempDetalleGastosDto_Agregar = new List<importaciondetallegastosDto>();
                            _TempDetalleGastosDto_Modificar = new List<importaciondetallegastosDto>();
                            _TempDetalleGastosDto_Eliminar = new List<importaciondetallegastosDto>();

                            ImportacionGuardada = true;

                            linkAsiento.Visible = true;


                        }
                        else
                        {
                            UltraMessageBox.Show("Ocurrió un error al Guardar, Contáctese con el Administrador", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _TempDetalleFobDto_Agregar = new List<importaciondetallefobDto>();
                            _TempDetalleFobDto_Modificar = new List<importaciondetallefobDto>();
                            _TempDetalleFobDto_Eliminar = new List<importaciondetallefobDto>();
                            _TempDetalleProdDto_Agregar = new List<importaciondetalleproductoDto>();
                            _TempDetalleProdDto_Modificar = new List<importaciondetalleproductoDto>();
                            _TempDetalleProdDto_Eliminar = new List<importaciondetalleproductoDto>();
                            ImportacionGuardada = false;
                            linkAsiento.Visible = false;


                        }
                        //_TempDetalleFobDto_Agregar = new List<importaciondetallefobDto>();
                        //_TempDetalleFobDto_Modificar = new List<importaciondetallefobDto>();
                        //_TempDetalleFobDto_Eliminar = new List<importaciondetallefobDto>();
                        //_TempDetalleProdDto_Agregar = new List<importaciondetalleproductoDto>();
                        //_TempDetalleProdDto_Modificar = new List<importaciondetalleproductoDto>();
                        //_TempDetalleProdDto_Eliminar = new List<importaciondetalleproductoDto>();

                        //_TempDetalleGastosDto_Agregar = new List<importaciondetallegastosDto>();
                        //_TempDetalleGastosDto_Modificar = new List<importaciondetallegastosDto>();
                        //_TempDetalleGastosDto_Eliminar = new List<importaciondetallegastosDto>();


                    }


                }
            }
        }
        //private bool  ConsultarSiTieneTesoreria()
        //{
        //   _objImportacionDto.v_IdImportacion 


        //}

        #endregion
        #region GrillaProductos
        private void grdDataProductos_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (grdDataProductos.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grdDataProductos.ActiveCell.Column.Key)
                {
                    case "d_Cantidad":

                        Celda = grdDataProductos.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;
                    case "d_Precio":
                        Celda = grdDataProductos.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);

                        break;

                }
            }



        }
        private void CalcularValoresDetalleGrillaProductos()
        {
            CalcularSubTotal();
            if (grdDataProductos.Rows.Count() == 0) return;
            foreach (UltraGridRow Fila in grdDataProductos.Rows)
            {
                CalcularValoresFilaProductos(Fila);
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }



        }
        private void CalcularValoresDetalleGrillaFob()
        {
            if (grdDataFob.Rows.Count() == 0)
            {
                CalcularTotales();
                return;
            }
            foreach (UltraGridRow Fila in grdDataFob.Rows)
            {
                CalcularValoresFilaFob(Fila);
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }

        }
        private void CalcularSubTotal()
        {
            decimal ValorFob = 0, Flete = 0, Seguro = 0, AdValorem = 0, subTotal = 0;
            AdValorem = txtAdValorem.Text == string.Empty ? 0 : decimal.Parse(txtAdValorem.Text.ToString());
            Seguro = txtSeguro.Text == string.Empty ? 0 : decimal.Parse(txtSeguro.Text.ToString());
            Flete = txtFlete.Text == string.Empty ? 0 : decimal.Parse(txtFlete.Text.ToString());
            ValorFob = txtTotalValorFob.Text == string.Empty ? 0 : decimal.Parse(txtTotalValorFob.Text.ToString());
            subTotal = AdValorem + Seguro + Flete + ValorFob;
            txtSubTotal.Text = subTotal.ToString();

        }
        private bool ValidarCuentasGeneracionLibroDiario()
        {

            string ConceptoFlete = ((int)Concepto.Flete).ToString();
            string ConceptoProveedorFlete = ((int)Concepto.ProveedoresFlete).ToString();
            if (decimal.Parse(txtFlete.Text) > 0)
            {
                if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoFlete) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorFlete)) { }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  Flete", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

            }


            string ConceptoFob = ((int)Concepto.ValorFob).ToString();
            string ConceptoProveedoresFob = ((int)Concepto.ProveedoresFob).ToString();
            if (decimal.Parse(txtTotalValorFob.Text) > 0)
            {
                if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoFob) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedoresFob)) { }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  FOB", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            string ConceptoSeguro = ((int)Concepto.Seguro).ToString();
            string ConceptoProveedorSeguro = ((int)Concepto.ProveedoresSeguro).ToString();
            if (decimal.Parse(txtSeguro.Text) > 0)
            {
                if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoSeguro) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorSeguro)) { }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  Seguro", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            string ConceptoAdValorem = ((int)Concepto.AdValorem).ToString();
            string ConceptoProveedorAdvalorem = ((int)Concepto.ProveedoresAdvalorem).ToString();


            if (decimal.Parse(txtAdValorem.Text) > 0)
            {
                if ((_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoAdValorem) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorAdvalorem))) { }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  AdValorem", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }



            string ConceptoIgv = ((int)Concepto.Igv).ToString();
            string ConceptoProveedorIgv = ((int)Concepto.ProveedorIgv).ToString();

            if (decimal.Parse(txtTotalIgv.Text) > 0)
            {
                if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoIgv) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorIgv)) { }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  Igv", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            string Percepcion = ((int)Concepto.Percepcion).ToString();
            string ConceptoProveedorPercepcion = ((int)Concepto.ProveedorPercepcion).ToString();

            if (decimal.Parse(txtPercepcion.Text) > 0)
            {
                if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(Percepcion) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedorPercepcion)) { }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente Administracion de Conceptos  Percepción", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;



        }

        //private bool ValidarCuentasGeneracionLibroDiario()
        //{
        //    //Generacion de Libro de Proveedores Fob
        //    string ConceptoFob = ((int)Concepto.ValorFob).ToString(), ConceptoFlete = ((int)Concepto.Flete).ToString(), ConceptoSeguro = ((int)Concepto.Seguro).ToString(), ConceptoAdValorem = ((int)Concepto.AdValorem).ToString(), ConceptoIgv = ((int)Concepto.Igv).ToString(), ConceptoProveedoresFob = ((int)Concepto.ProveedoresFob).ToString();

        //    if (_objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoFob) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoFlete) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoSeguro) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoAdValorem) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoIgv) && _objImportacionBL.ValidarNroCuentaGeneracionLibro(ConceptoProveedoresFob))
        //    {
        //        return true;
        //    }

        //    return false;
        //    //Generacion de Libro Compra



        //}
      private void FormatoDecimalesGrillaProductos(int DecimalesCantidad, int DecimalesPrecio)
        {
            string FormatoCantidad, FormatoPrecio;
            UltraGridColumn _Cantidad = this.grdDataProductos.DisplayLayout.Bands[0].Columns["d_Cantidad"];
            _Cantidad.MaskDataMode = MaskMode.IncludeLiterals;
            _Cantidad.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _Precio = this.grdDataProductos.DisplayLayout.Bands[0].Columns["d_Precio"];
            _Precio.MaskDataMode = MaskMode.IncludeLiterals;
            _Precio.MaskDisplayMode = MaskMode.IncludeLiterals;

            if (DecimalesCantidad > 0)
            {
                string sharp = "n";
                FormatoCantidad = "nnnnnnnnnnnnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "nnnnnnnnnnnnn";
            }

            if (DecimalesPrecio > 0)
            {
                string sharp = "n";
                FormatoPrecio = "nnnnnnnnnnnnn.";
                for (int i = 0; i < DecimalesPrecio; i++)
                {
                    FormatoPrecio = FormatoPrecio + sharp;
                }
            }
            else
            {
                FormatoPrecio = "nnnnnnnnnnnnn";
            }

            _Cantidad.MaskInput = FormatoCantidad;
            _Precio.MaskInput = "-" + FormatoPrecio;
        }
        private void ActualizaNotadeIngreso(ref OperationResult objOperationResult)
        {
            //using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
            //{
            List<string> Proveedores = new List<string>();
            _movimientoDto = new movimientoDto();
            _movimientoDto = _objMovimientoBL.ObtenerMovimientoCabeceraDesdeImportacion(ref objOperationResult, int.Parse(cboAlmacen.Value.ToString()), "I", _objImportacionDto.v_Periodo, _objImportacionDto.v_Mes.Trim(), _objImportacionDto.v_Correlativo.Trim());
            string IdProveedorFOB = null;
            try
            {
                IdProveedorFOB = grdDataFob.Rows.Any() ? grdDataFob.Rows[0].Cells["v_IdCliente"].Value.ToString() : null;
            }
            catch (Exception ex)
            {
                IdProveedorFOB = null;
            }
            if (cboEstado.Value.ToString() != "0")
            {
                if (_movimientoDto != null)
                {
                    if (_movimientoDto.v_Mes.Trim() != dtpFechaRegistro.Value.Month.ToString("00").Trim() || _movimientoDto.v_Periodo.Trim() != dtpFechaRegistro.Value.Year.ToString()) // Se agrego para la generacion del correlativo por numero Registro
                    {
                        List<KeyValueDTO> _ListaMovimientos = new List<KeyValueDTO>();
                        string Mes;
                        Mes = int.Parse(dtpFechaRegistro.Value.Month.ToString()) < 9 ? ("0" + dtpFechaRegistro.Value.Month.ToString()).Trim() : dtpFechaRegistro.Value.Month.ToString();
                        _ListaMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref objOperationResult, txtPeriodo.Text.Trim(), Mes, (int)Common.Resource.TipoDeMovimiento.NotadeIngreso);
                        if (_ListaMovimientos.Count != 0)
                        {
                            int MaxMovimiento;
                            MaxMovimiento = _ListaMovimientos.Count() > 0 ? int.Parse(_ListaMovimientos[_ListaMovimientos.Count() - 1].Value1.ToString()) : 0;
                            MaxMovimiento++;

                            _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                            _movimientoDto.v_Mes = int.Parse(dtpFechaRegistro.Value.Month.ToString()) < 10 ? 0 + dtpFechaRegistro.Value.Month.ToString() : dtpFechaRegistro.Value.Month.ToString();
                            _movimientoDto.v_Periodo = dtpFechaRegistro.Value.Year.ToString();
                            _movimientoDto.v_OrigenRegCorrelativo = txtCorrelativo.Text.Trim();
                            _movimientoDto.v_OrigenRegMes = txtMes.Text.Trim();
                            _movimientoDto.v_OrigenRegPeriodo = txtPeriodo.Text.Trim();


                        }
                        else
                        {
                            _movimientoDto.v_Correlativo = "00000001";
                            _movimientoDto.v_Mes = int.Parse(dtpFechaRegistro.Value.Month.ToString()) < 10 ? 0 + dtpFechaRegistro.Value.Month.ToString() : dtpFechaRegistro.Value.Month.ToString();
                            _movimientoDto.v_Periodo = dtpFechaRegistro.Value.Year.ToString();
                            _movimientoDto.v_OrigenRegCorrelativo = txtCorrelativo.Text.Trim();
                            _movimientoDto.v_OrigenRegMes = txtMes.Text.Trim();
                            _movimientoDto.v_OrigenRegPeriodo = txtPeriodo.Text.Trim();
                        }


                    }

                    _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                    _movimientoDto.i_IdAlmacenOrigen = int.Parse(cboAlmacen.Value.ToString());
                    _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                    _movimientoDto.t_Fecha = dtpFechaRegistro.Value;
                    _movimientoDto.v_Mes = txtMes.Text.Trim();
                    _movimientoDto.i_EsDevolucion = 0;
                    _movimientoDto.v_IdCliente = IdProveedorFOB;
                    _movimientoDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());

                    LlenarTemporalesMovimiento();
                    decimal Cantidad = 0, Total = 0, _Cantidad, _Total;
                    for (int i = 0; i < grdDataProductos.Rows.Count(); i++)
                    {

                        decimal precio, cantidad;
                        _Cantidad = grdDataProductos.Rows[i].Cells["d_Cantidad"].Value != null ? decimal.Parse(grdDataProductos.Rows[i].Cells["d_Cantidad"].Value.ToString()) : 0;
                        Cantidad = Cantidad + _Cantidad;
                        precio = grdDataProductos.Rows[i].Cells["d_CostoUnitario"].Value != null ? Utils.Windows.DevuelveValorRedondeado(decimal.Parse(grdDataProductos.Rows[i].Cells["d_CostoUnitario"].Value.ToString()), 4) : 0;
                        cantidad = grdDataProductos.Rows[i].Cells["d_Cantidad"].Value != null ? decimal.Parse(grdDataProductos.Rows[i].Cells["d_Cantidad"].Value.ToString()) : 0;
                        _Total = precio * cantidad;
                        Total = Total + _Total;

                    }

                    _movimientoDto.d_TotalCantidad = Cantidad;
                    _movimientoDto.d_TotalPrecio = Total;
                    _objMovimientoBL.ActualizarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempMovimientodetalle_Agregar, _TempMovimientodetalle_Modificar, _TempMovimientodetalle_Eliminar);
                    _TempMovimientodetalle_Agregar = new List<movimientodetalleDto>();
                    _TempMovimientodetalle_Modificar = new List<movimientodetalleDto>();
                    _TempMovimientodetalle_Eliminar = new List<movimientodetalleDto>();

                }
                else
                {
                    _movimientoDto = new movimientoDto();
                    List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
                    ListaMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref objOperationResult, txtPeriodo.Text, txtMes.Text, (int)TipoDeMovimiento.NotadeIngreso);

                    int MaxMovimiento;
                    MaxMovimiento = ListaMovimientos.Count() > 0 ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString()) : 0;
                    MaxMovimiento++;

                    _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                    _movimientoDto.i_IdAlmacenOrigen = int.Parse(cboAlmacen.Value.ToString());
                    _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                    _movimientoDto.i_IdTipoMotivo = 6;//Compra nacional
                    _movimientoDto.t_Fecha = dtpFechaRegistro.Value;

                    _movimientoDto.v_Mes = txtMes.Text.Trim();
                    _movimientoDto.v_Periodo = txtPeriodo.Text.Trim();
                    _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                    _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                    _movimientoDto.v_IdCliente = IdProveedorFOB;
                    _movimientoDto.v_OrigenTipo = "I";
                    _movimientoDto.v_OrigenRegCorrelativo = txtCorrelativo.Text;
                    _movimientoDto.v_OrigenRegMes = txtMes.Text.Trim();
                    _movimientoDto.v_OrigenRegPeriodo = txtPeriodo.Text.Trim();
                    _movimientoDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());

                    LlenarTemporalesMovimiento();
                    decimal Cantidad = 0, Total = 0, _Cantidad, _Total;
                    for (int i = 0; i < grdDataProductos.Rows.Count(); i++)
                    {

                        decimal precio, cantidad;
                        _Cantidad = grdDataProductos.Rows[i].Cells["d_Cantidad"].Value != null ? decimal.Parse(grdDataProductos.Rows[i].Cells["d_Cantidad"].Value.ToString()) : 0;
                        Cantidad = Cantidad + _Cantidad;

                        precio = grdDataProductos.Rows[i].Cells["d_Precio"].Value != null ? Utils.Windows.DevuelveValorRedondeado(decimal.Parse(grdDataProductos.Rows[i].Cells["d_Precio"].Value.ToString()), 4) : 0;
                        cantidad = grdDataProductos.Rows[i].Cells["d_Cantidad"].Value != null ? decimal.Parse(grdDataProductos.Rows[i].Cells["d_Cantidad"].Value.ToString()) : 0;
                        _Total = precio * cantidad;
                        Total = Total + _Total;

                    }

                    _movimientoDto.d_TotalCantidad = Cantidad;
                    _movimientoDto.d_TotalPrecio = Total;
                    if (_TempMovimientodetalle_Agregar.Any())
                    {
                        _objMovimientoBL.InsertarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempMovimientodetalle_Agregar);
                    }
                    _TempMovimientodetalle_Agregar = new List<movimientodetalleDto>();
                }
            }
            else
            {
                if (_movimientoDto != null && _movimientoDto.v_IdMovimiento != null)
                {
                    _objMovimientoBL.EliminarMovimiento(ref objOperationResult, _movimientoDto.v_IdMovimiento, Globals.ClientSession.GetAsList());
                }
            }
            //    ts.Complete();
            //}

        }
        private void LlenarTemporalesMovimiento()
        {
            if (grdDataProductos.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdDataProductos.Rows)
                {
                    if (Fila.Cells["EsServicio"].Value.ToString() != "1")
                    {
                        if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                        {
                            switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                            {
                                case "Temporal":

                                    if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                                    {

                                        _movimientodetalleDto = new movimientodetalleDto();
                                        _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                        _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                        _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                                        _movimientodetalleDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());

                                        _movimientodetalleDto.v_NumeroDocumento = cboSerieDoc.Text.Trim() + "-" + txtNumeroDoc.Text.Trim();
                                        _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());


                                        _movimientodetalleDto.d_Precio = Fila.Cells["d_CostoUnitario"].Value == null ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_CostoUnitario"].Text), 6);
                                        _movimientodetalleDto.d_Total = Utils.Windows.DevuelveValorRedondeado((_movimientodetalleDto.d_Precio.Value * _movimientodetalleDto.d_Cantidad.Value), 2);

                                        _movimientodetalleDto.d_PrecioCambio = Fila.Cells["d_CostoUnitarioCambio"].Value == null ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_CostoUnitarioCambio"].Text), 6);
                                        _movimientodetalleDto.d_TotalCambio = Utils.Windows.DevuelveValorRedondeado((_movimientodetalleDto.d_Cantidad.Value * _movimientodetalleDto.d_PrecioCambio.Value), 2);

                                        _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                        _movimientodetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                        _TempMovimientodetalle_Agregar.Add(_movimientodetalleDto);

                                    }
                                    break;

                                case "NoTemporal":
                                    if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                                    {
                                        _movimientodetalleDto = new movimientodetalleDto();
                                        _movimientodetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                        _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                                        _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                        _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                        _movimientodetalleDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());

                                        _movimientodetalleDto.v_NumeroDocumento = cboSerieDoc.Text.Trim() + "-" + txtNumeroDoc.Text.Trim();
                                        _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());

                                        _movimientodetalleDto.d_Precio = Fila.Cells["d_CostoUnitario"].Value == null ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_CostoUnitario"].Text), 6);
                                        _movimientodetalleDto.d_Total = Utils.Windows.DevuelveValorRedondeado((_movimientodetalleDto.d_Precio.Value * _movimientodetalleDto.d_Cantidad.Value), 2); // decimal.Parse(Fila.Cells["d_CostoUnitario"].Text) * decimal.Parse(Fila.Cells["d_Cantidad"].Text);

                                        _movimientodetalleDto.d_PrecioCambio = Fila.Cells["d_CostoUnitarioCambio"].Value == null ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_CostoUnitarioCambio"].Text), 6);
                                        _movimientodetalleDto.d_TotalCambio = Utils.Windows.DevuelveValorRedondeado((_movimientodetalleDto.d_Cantidad.Value * _movimientodetalleDto.d_PrecioCambio.Value), 2);


                                        _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                        _movimientodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                        _movimientodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                        _movimientodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                        _movimientodetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                        _TempMovimientodetalle_Modificar.Add(_movimientodetalleDto);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

        }
        private List<string> ObtenerProveedores()
        {
            List<string> Proveedores = new List<string>();
            string pstrProveedor;
            foreach (UltraGridRow Fila in grdDataProductos.Rows)
            {
                if (Fila.Cells["v_IdCliente"].Value != null)
                {
                    pstrProveedor = Fila.Cells["v_IdCliente"].Value.ToString();
                    if (!Proveedores.Contains(pstrProveedor))
                    {
                        Proveedores.Add(pstrProveedor);
                    }
                }
            }
            return Proveedores;
        }
        private void grdDataProductos_AfterCellUpdate(object sender, CellEventArgs e)
        {
            //grdDataFob.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }
        private void grdDataProductos_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            if (e.Cell.Column == null) return;
            switch (e.Cell.Column.Key)
            {
                case "v_CodigoInterno":

                    if (cboAlmacen.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por Favor seleccione un almacén ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboAlmacen.Focus();
                        return;
                    }
                    Mantenimientos.frmBuscarProducto frmBusquedaProducto = new Mantenimientos.frmBuscarProducto(int.Parse(cboAlmacen.Value.ToString()), null, null, grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodigoInterno"].Text == null ? string.Empty : grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodigoInterno"].Text.ToString());
                    frmBusquedaProducto.ShowDialog();

                    if (frmBusquedaProducto._NombreProducto != null)
                    {

                        if (Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 0)
                        {
                            foreach (UltraGridRow Fila in grdDataProductos.Rows)
                            {

                                if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                                {

                                    if (frmBusquedaProducto._IdProducto == Fila.Cells["v_IdProductoDetalle"].Value.ToString())
                                    {
                                        UltraMessageBox.Show("El producto ya existe en el detalle ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                            }
                        }

                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NombreProducto"].Value = frmBusquedaProducto._NombreProducto.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodigoInterno"].Value = frmBusquedaProducto._CodigoInternoProducto.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["Empaque"].Value = frmBusquedaProducto._Empaque != null ? frmBusquedaProducto._Empaque.ToString() : string.Empty;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["UMEmpaque"].Value = frmBusquedaProducto._UnidadMedida != null ? frmBusquedaProducto._UnidadMedida.ToString() : string.Empty;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = frmBusquedaProducto._UnidadMedidaEmpaque != null ? frmBusquedaProducto._UnidadMedidaEmpaque.ToString() : string.Empty;  //Por defecto ,pero si desea el usuario lo puede cambiar
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["d_Cantidad"].Value = "1";
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["EsServicio"].Value = frmBusquedaProducto._EsServicio != null ? frmBusquedaProducto._EsServicio.ToString() : string.Empty;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = frmBusquedaProducto._IdProducto.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = frmBusquedaProducto._UnidadMedidaEmpaque != null ? frmBusquedaProducto._UnidadMedidaEmpaque.ToString() : null;



                    }


                    break;
            }
        }
        private void btnAgregarDetalleProducto_Click(object sender, EventArgs e)
        {

            if (grdDataFob.Rows.Count() == 0)
            {
                UltraMessageBox.Show("No se permite Agregar Detalle Productos mientras Valor Fob de la Factura esté vacío ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (grdDataFob.Rows.Where(p => p.Cells["v_IdCliente"].Value == null || p.Cells["v_IdCliente"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {

                UltraMessageBox.Show("Por favor elija un proveedor ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["v_IdCliente"].Value == null || x.Cells["v_IdCliente"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataFob.Selected.Cells.Add(Row.Cells["v_CodCliente"]);
                grdDataFob.Focus();
                Row.Activate();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["v_CodCliente"];
                this.grdDataFob.ActiveCell = aCell;
                return;

            }

            if (grdDataFob.Rows.Where(p => p.Cells["i_IdTipoDocumento"].Value == null || p.Cells["i_IdTipoDocumento"].Value.ToString().Trim() == string.Empty || p.Cells["i_IdTipoDocumento"].Value.ToString().Trim() == "-1").Count() != 0)
            {
                UltraMessageBox.Show("Por favor elija un tipo de documento ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataFob.Rows.Where(p => p.Cells["i_IdTipoDocumento"].Value == null || p.Cells["i_IdTipoDocumento"].Value.ToString().Trim() == string.Empty || p.Cells["i_IdTipoDocumento"].Value.ToString().Trim() == "-1").FirstOrDefault();
                grdDataFob.Selected.Cells.Add(Row.Cells["i_IdTipoDocumento"]);
                grdDataFob.Focus();
                Row.Activate();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["i_IdTipoDocumento"];
                this.grdDataFob.ActiveCell = aCell;
                return;



            }

            if (grdDataFob.Rows.Where(p => p.Cells["v_SerieDocumento"].Value == null || p.Cells["v_SerieDocumento"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor registre la serie documento ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["v_SerieDocumento"].Value == null || x.Cells["v_SerieDocumento"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataFob.Selected.Cells.Add(Row.Cells["v_SerieDocumento"]);
                grdDataFob.Focus();
                Row.Activate();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["v_SerieDocumento"];
                this.grdDataFob.ActiveCell = aCell;

                return;

            }

            if (grdDataFob.Rows.Where(p => p.Cells["v_CorrelativoDocumento"].Value == null || p.Cells["v_CorrelativoDocumento"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor registre el Numero Documento  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["v_CorrelativoDocumento"].Value == null || x.Cells["v_CorrelativoDocumento"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataFob.Selected.Cells.Add(Row.Cells["v_CorrelativoDocumento"]);
                grdDataFob.Focus();
                Row.Activate();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["v_CorrelativoDocumento"];
                this.grdDataFob.ActiveCell = aCell;

                return;

            }


            if (cboAlmacen.Value.ToString() == "-1")
            {
                UltraMessageBox.Show("Por Favor seleccione un almacén ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboAlmacen.Focus();
                return;
            }


            if (grdDataProductos.ActiveRow != null)
            {
                if (grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodigoInterno"].Value != null)
                {

                    UltraGridRow row = grdDataProductos.DisplayLayout.Bands[0].AddNew();
                    grdDataProductos.Rows.Move(row, grdDataProductos.Rows.Count() - 1);
                    this.grdDataProductos.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["v_CodigoInterno"];
                    this.grdDataProductos.ActiveCell = aCell;
                    grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    grdDataProductos.Focus();
                    grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["d_ValorFob"].Value = "0";
                    row.Cells["d_Flete"].Value = "0";
                    row.Cells["d_Igv"].Value = "0";
                    row.Cells["d_OtrosGastos"].Value = "0";
                    row.Cells["d_AdValorem"].Value = "0";
                    row.Cells["d_Total"].Value = "0";


                    if (grdDataFob.Rows.Count() == 1)
                    {
                        string NroFactura = string.Empty;
                        string NroPedido = string.Empty;
                        string CodigoProveedor = string.Empty;
                        string IdProveedor = string.Empty;
                        foreach (var Fila in grdDataFob.Rows)
                        {
                            NroFactura = Fila.Cells["i_IdTipoDocumento"].Text + " " + Fila.Cells["v_SerieDocumento"].Value.ToString() + "-" + Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                            NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                            CodigoProveedor = Fila.Cells["v_CodCliente"].Value == null ? null : Fila.Cells["v_CodCliente"].Value.ToString();
                            IdProveedor = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();

                        }

                        row.Cells["v_NroFactura"].Value = NroFactura;
                        row.Cells["v_NroPedido"].Value = NroPedido;
                        row.Cells["v_CodCliente"].Value = CodigoProveedor.Trim();
                        row.Cells["v_IdCliente"].Value = IdProveedor;
                    }


                }
            }
            else
            {
                UltraGridRow row = grdDataProductos.DisplayLayout.Bands[0].AddNew();
                grdDataProductos.Rows.Move(row, grdDataProductos.Rows.Count() - 1);
                this.grdDataProductos.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["v_CodigoInterno"];
                this.grdDataProductos.ActiveCell = aCell;
                grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                grdDataProductos.Focus();
                grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                row.Cells["d_Precio"].Value = "0";
                row.Cells["d_ValorFob"].Value = "0";
                row.Cells["d_Flete"].Value = "0";
                row.Cells["d_Igv"].Value = "0";
                row.Cells["d_OtrosGastos"].Value = "0";
                row.Cells["d_AdValorem"].Value = "0";
                row.Cells["d_Total"].Value = "0";
                if (grdDataFob.Rows.Count() == 1)
                {
                    string NroFactura = string.Empty;
                    string NroPedido = string.Empty;
                    string CodigoProveedor = string.Empty;
                    string IdProveedor = string.Empty;
                    foreach (var Fila in grdDataFob.Rows)
                    {
                        NroFactura = Fila.Cells["i_IdTipoDocumento"].Text + " " + Fila.Cells["v_SerieDocumento"].Value.ToString() + "-" + Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                        NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                        CodigoProveedor = Fila.Cells["v_CodCliente"].Value == null ? null : Fila.Cells["v_CodCliente"].Value.ToString();
                        IdProveedor = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();

                    }

                    row.Cells["v_NroFactura"].Value = NroFactura;
                    row.Cells["v_NroPedido"].Value = NroPedido;
                    row.Cells["v_CodCliente"].Value = CodigoProveedor;
                    row.Cells["v_IdCliente"].Value = IdProveedor;
                }

            }

        }
        private void grdDataProductos_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].EditorComponent = ucbUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }
        private void grdDataProductos_CellChange(object sender, CellEventArgs e)
        {
            grdDataProductos.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }
        private void grdDataProductos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }
        private void grdDataProductos_AfterExitEditMode(object sender, EventArgs e)
        {
            CalcularValoresFilaProductos(grdDataProductos.Rows[grdDataProductos.ActiveRow.Index]);

        }
        private void CalcularValoresFilaProductos(UltraGridRow Fila)
        {
            decimal ValorFob = txtTotalValorFob.Text == string.Empty ? 0 : decimal.Parse(txtTotalValorFob.Text.ToString());
            decimal Seguro = txtSeguro.Text == string.Empty ? 0 : decimal.Parse(txtSeguro.Text.ToString());
            decimal AdValorem = txtAdValorem.Text == string.Empty ? 0 : decimal.Parse(txtAdValorem.Text.ToString());
            decimal Igv = txtTotalIgv.Text == string.Empty ? 0 : decimal.Parse(txtTotalIgv.Text.ToString());
            decimal OtrosGastos = txtOtrosGastos.Text == string.Empty ? 0 : decimal.Parse(txtOtrosGastos.Text.ToString());
            decimal flete = txtFlete.Text == string.Empty ? 0 : decimal.Parse(txtFlete.Text.ToString());

            if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
            if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }
            if (Fila.Cells["d_ValorFob"].Value == null) { Fila.Cells["d_ValorFob"].Value = "0"; }
            if (Fila.Cells["d_Flete"].Value == null) { Fila.Cells["d_Flete"].Value = "0"; }
            if (Fila.Cells["d_Seguro"].Value == null) { Fila.Cells["d_Seguro"].Value = "0"; }
            if (Fila.Cells["d_AdValorem"].Value == null) { Fila.Cells["d_Advalorem"].Value = "0"; }
            if (Fila.Cells["d_Igv"].Value == null) { Fila.Cells["d_Igv"].Value = "0"; }
            if (Fila.Cells["d_OtrosGastos"].Value == null) { Fila.Cells["d_OtrosGastos"].Value = "0"; }
            if (Fila.Cells["d_Total"].Value == null) { Fila.Cells["d_Total"].Value = "0"; }
            if (Fila.Cells["d_CostoUnitario"].Value == null) { Fila.Cells["d_CostoUnitario"].Value = "0"; }
            if (Fila.Cells["d_PrecioVenta"].Value == null) { Fila.Cells["d_PrecioVenta"].Value = "0"; }
            if (Fila.Cells["d_CostoUnitarioCambio"].Value == null) { Fila.Cells["d_CostoUnitarioCambio"].Value = "0"; }
            if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0 && decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0)
            {
                Fila.Cells["d_ValorFob"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Cantidad"].Text.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Text)), 6);
                Fila.Cells["d_Flete"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * flete), 6);
                Fila.Cells["d_Seguro"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * Seguro, 6);
                Fila.Cells["d_AdValorem"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * AdValorem, 6);
                Fila.Cells["d_Igv"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * Igv, 6);
                Fila.Cells["d_OtrosGastos"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * OtrosGastos, 6);
                // Fila.Cells["d_Total"].Value = decimal.Parse(Fila.Cells["d_ValorFob"].Value.ToString()) + decimal.Parse(Fila.Cells["d_Flete"].Value.ToString()) + decimal.Parse(Fila.Cells["d_Seguro"].Value.ToString()) + decimal.Parse(Fila.Cells["d_AdValorem"].Value.ToString()) + decimal.Parse(Fila.Cells["d_Igv"].Value.ToString()) + decimal.Parse(Fila.Cells["d_OtrosGastos"].Value.ToString());
                Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorFob"].Text) + decimal.Parse(Fila.Cells["d_Flete"].Text) + decimal.Parse(Fila.Cells["d_Seguro"].Text) + decimal.Parse(Fila.Cells["d_AdValorem"].Text) + decimal.Parse(Fila.Cells["d_OtrosGastos"].Text) + decimal.Parse(Fila.Cells["d_Igv"].Text), 6);
                Fila.Cells["d_CostoUnitario"].Value = decimal.Parse(Fila.Cells["d_Cantidad"].Text.ToString()) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) + decimal.Parse(Fila.Cells["d_Flete"].Text.ToString()) + decimal.Parse(Fila.Cells["d_Seguro"].Text.ToString()) + decimal.Parse(Fila.Cells["d_AdValorem"].Text.ToString()) + decimal.Parse(Fila.Cells["d_OtrosGastos"].Text.ToString())) / decimal.Parse(Fila.Cells["d_Cantidad"].Text.ToString()), 6);
                Fila.Cells["d_PrecioVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_CostoUnitario"].Text.ToString()) + (decimal.Parse(txtUtilidad.Text.ToString()) * (decimal.Parse(Fila.Cells["d_CostoUnitario"].Text)) / 100), 6);
                Fila.Cells["d_CostoUnitarioCambio"].Value = CalcularCostoCambio(decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()), decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()), Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString(), Fila.Cells["v_NroFactura"].Value == null ? null : Fila.Cells["v_NroFactura"].Value.ToString());

            }
            CalcularTotalesProductos();

        }

        private decimal CalcularCostoCambio(decimal d_Cantidad, decimal d_Precio, string IdProveedor, string NroFactura)
        {

            decimal FleteCambio = 0, FobCambio = 0, AdvaloremCambio = 0, SeguroCambio = 0, IgvCambio = 0;
            decimal CostoCambio = 0, OtrosGastosCambio = 0, CostoUnitarioCambio = 0;
            decimal TotalFobSoles = 0, TotalFobDolares = 0;

            foreach (var Fila in grdDataFob.Rows)
            {

                if (Fila.Cells["d_TipoCambio"].Value == null) { Fila.Cells["d_TipoCambio"].Value = "0"; }
                if (Fila.Cells["d_ValorFob"].Value == null) { Fila.Cells["d_ValorFob"].Value = "0"; }
                switch (int.Parse(cboMoneda.Value.ToString()))
                {
                    case 1:
                        TotalFobSoles = TotalFobSoles + decimal.Parse(Fila.Cells["d_ValorFob"].Value.ToString());
                        TotalFobDolares = decimal.Parse(Fila.Cells["d_TipoCambio"].Value.ToString()) == 0 ? TotalFobDolares : TotalFobDolares + Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Value.ToString()) / decimal.Parse(Fila.Cells["d_TipoCambio"].Value.ToString())), 2);
                        TotalFobDolares = Utils.Windows.DevuelveValorRedondeado(TotalFobDolares, 2);
                        break;
                    case 2:
                        TotalFobDolares = TotalFobDolares + decimal.Parse(Fila.Cells["d_ValorFob"].Value.ToString());
                        TotalFobSoles = TotalFobSoles + Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Value.ToString()) * decimal.Parse(Fila.Cells["d_TipoCambio"].Value.ToString())), 2);
                        TotalFobSoles = Utils.Windows.DevuelveValorRedondeado(TotalFobSoles, 2);
                        break;
                }
            }
            FobCambio = int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles ? TotalFobDolares : TotalFobSoles;
            switch (int.Parse(cboMoneda.Value.ToString())) // Si la modenda de la Importación es en Dolares, su  Cambio es Soles y si la moneda de la Importaciòn es Soles su cambio es en Dolares
            {

                case 1:
                    OtrosGastosCambio = TotalDolares;
                    FleteCambio = txtFlete.Text == String.Empty || decimal.Parse(txtTipoCambioDoc1.Text) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(txtFlete.Text.ToString()) / decimal.Parse(txtTipoCambioDoc1.Text.ToString()), 4);
                    SeguroCambio = txtSeguro.Text == string.Empty || decimal.Parse(txtTipoCambioDoc2.Text) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(txtSeguro.Text.ToString()) / decimal.Parse(txtTipoCambioDoc2.Text.ToString()), 4);
                    AdvaloremCambio = txtAdValorem.Text == string.Empty || decimal.Parse(txtTipoCambioDoc3.Text) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(txtAdValorem.Text) / decimal.Parse(txtTipoCambioDoc3.Text.ToString()), 4);
                    IgvCambio = txtTotalIgv.Text == string.Empty || decimal.Parse(txtTipoCambioDoc4.Text) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(txtTotalIgv.Text.ToString()) / decimal.Parse(txtTipoCambioDoc4.Text.ToString()), 4);
                    break;
                case 2:


                    // var fobPrueba = Utils.Windows.DevuelveValorRedondeado(grdDataFob.Rows.Select(p => Utils.Windows.DevuelveValorRedondeado(decimal.Parse(p.Cells["d_ValorFob"].Value.ToString()) * decimal.Parse(p.Cells["d_TipoCambio"].Value.ToString()), 2)).ToList().Sum(), 2);
                    OtrosGastosCambio = TotalSoles;
                    FleteCambio = txtFlete.Text == String.Empty ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(txtFlete.Text.ToString()) * decimal.Parse(txtTipoCambioDoc1.Text.ToString()), 4);
                    SeguroCambio = txtSeguro.Text == string.Empty ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(txtSeguro.Text.ToString()) * decimal.Parse(txtTipoCambioDoc2.Text.ToString()), 4);
                    AdvaloremCambio = txtAdValorem.Text == string.Empty ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(txtAdValorem.Text) * decimal.Parse(txtTipoCambioDoc3.Text.ToString()), 4);
                    IgvCambio = txtTotalIgv.Text == string.Empty ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(txtTotalIgv.Text.ToString()) * decimal.Parse(txtTipoCambioDoc4.Text.ToString()), 4);
                    break;

                //return CostoUnitarioCambio;

            }

            if (d_Cantidad != 0 && d_Precio != 0)
            {
                decimal PrecioCambio = CalcularPrecioCambio(NroFactura, IdProveedor, int.Parse(cboMoneda.Value.ToString()), d_Precio);

                decimal d_ValorFobCambio = Utils.Windows.DevuelveValorRedondeado(d_Cantidad * PrecioCambio, 6);
                decimal d_FleteCambio = FobCambio == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(((d_ValorFobCambio / FobCambio) * FleteCambio), 6);
                decimal d_SeguroCambio = FobCambio == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_ValorFobCambio / FobCambio) * SeguroCambio, 6);
                decimal d_AdvaloremCambio = FobCambio == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_ValorFobCambio / FobCambio) * AdvaloremCambio, 6);
                decimal d_IgvCambio = FobCambio == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_ValorFobCambio / FobCambio) * IgvCambio, 6);
                decimal d_OtrosGastosCambio = FobCambio == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_ValorFobCambio / FobCambio) * OtrosGastosCambio, 6);
                CostoUnitarioCambio = d_Cantidad == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_ValorFobCambio + d_FleteCambio + d_SeguroCambio + d_AdvaloremCambio + d_OtrosGastosCambio) / d_Cantidad, 6);
            }

            return CostoUnitarioCambio;
        }


        public decimal CalcularPrecioCambio(string NroFactura, string IdProveedor, int Moneda, decimal Precio)
        {
            if (!string.IsNullOrEmpty(NroFactura))
            {
                string[] TipoDocumento;
                string[] SerieCorrelativo;
                TipoDocumento = NroFactura.Split(new Char[] { ' ' });
                string TipoDocumentoSiglas = TipoDocumento[0].Trim();
                SerieCorrelativo = TipoDocumento[1].Trim().Split(new Char[] { '-' });
                string SerieDocumento = SerieCorrelativo[0].Trim();
                string CorrelativoDocumento = SerieCorrelativo[1].Trim();

                var TipoCambio = grdDataFob.Rows.Where(p => p.Cells["v_IdCliente"].Value.ToString() == IdProveedor && p.Cells["i_IdTipoDocumento"].Text == TipoDocumentoSiglas && p.Cells["v_SerieDocumento"].Text == SerieDocumento && p.Cells["v_CorrelativoDocumento"].Text == CorrelativoDocumento).Select(j => j.Cells["d_TipoCambio"].Value.ToString()).FirstOrDefault();
                return TipoCambio != null ? decimal.Parse(TipoCambio) == 0 ? 0 : Moneda == (int)Currency.Dolares ? Utils.Windows.DevuelveValorRedondeado(Precio * decimal.Parse(TipoCambio), 6) : Utils.Windows.DevuelveValorRedondeado(Precio / decimal.Parse(TipoCambio), 6) : 0;
            }
            else
            {
                return 0;

            }

        }
        private void CalcularTotalesProductos()
        {
            decimal FobResultado = 0, SeguroResultado = 0, IgvResultado = 0, FleteResultado = 0, AdValoremResultado = 0, OtrosGastosResultado = 0;
            if (grdDataProductos.Rows.Count() > 0)
            {
                foreach (UltraGridRow Fila in grdDataProductos.Rows)
                {
                    if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
                    if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }
                    if (Fila.Cells["d_ValorFob"].Value == null) { Fila.Cells["d_ValorFob"].Value = "0"; }
                    if (Fila.Cells["d_Flete"].Value == null) { Fila.Cells["d_Flete"].Value = "0"; }
                    if (Fila.Cells["d_Seguro"].Value == null) { Fila.Cells["d_Seguro"].Value = "0"; }
                    if (Fila.Cells["d_Advalorem"].Value == null) { Fila.Cells["d_Advalorem"].Value = "0"; }
                    if (Fila.Cells["d_Igv"].Value == null) { Fila.Cells["d_Igv"].Value = "0"; }
                    if (Fila.Cells["d_OtrosGastos"].Value == null) { Fila.Cells["d_OtrosGastos"].Value = "0"; }
                    if (Fila.Cells["d_Total"].Value == null) { Fila.Cells["d_Total"].Value = "0"; }

                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {

                        FobResultado = FobResultado + decimal.Parse(Fila.Cells["d_ValorFob"].Text);
                        SeguroResultado = SeguroResultado + decimal.Parse(Fila.Cells["d_Seguro"].Text);
                        IgvResultado = IgvResultado + decimal.Parse(Fila.Cells["d_Igv"].Text);
                        FleteResultado = FleteResultado + decimal.Parse(Fila.Cells["d_Flete"].Text);
                        AdValoremResultado = AdValoremResultado + decimal.Parse(Fila.Cells["d_Advalorem"].Text);
                        OtrosGastosResultado = OtrosGastosResultado + decimal.Parse(Fila.Cells["d_OtrosGastos"].Text);


                    }

                    if (Fila.Cells["i_IdUnidadMedida"].Value != null)
                    {
                        if (Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_IdUnidadMedida"].Value.ToString() != "-1" && decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0)
                        {
                            decimal TotalEmpaque = 0;
                            decimal Empaque = decimal.Parse(Fila.Cells["Empaque"].Value.ToString());
                            string Producto = Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                            decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                            int UM = int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                            int UMProducto = int.Parse(Fila.Cells["i_IdUnidadMedidaProducto"].Value.ToString());

                            GridKeyValueDTO _UMProducto = ((List<GridKeyValueDTO>)ucbUnidadMedida.DataSource).Where(p => p.Id == UMProducto.ToString()).FirstOrDefault();
                            GridKeyValueDTO _UM = ((List<GridKeyValueDTO>)ucbUnidadMedida.DataSource).Where(p => p.Id == UM.ToString()).FirstOrDefault();

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

                txtValorFob.Text = FobResultado.ToString("0.000000");
                txtSeguroResultado.Text = SeguroResultado.ToString("0.000000");
                txtIgvResultado.Text = IgvResultado.ToString("0.000000");
                txtFleteResultado.Text = FleteResultado.ToString("0.000000");
                txtAdValoremResultado.Text = AdValoremResultado.ToString("0.000000");
                txtOtrosGastosResultado.Text = OtrosGastosResultado.ToString("0.000000");

            }
            else
            {
                txtValorFob.Text = "0.000000";
                txtSeguroResultado.Text = "0.000000";
                txtIgvResultado.Text = "0.000000";
                txtFleteResultado.Text = "0.000000";
                txtAdValoremResultado.Text = "0.000000";
                txtOtrosGastosResultado.Text = "0.000000";
            }

        }
        private void grdDataProductos_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));


            if (row == null)
            {


                btnEliminarDetalleProducto.Enabled = false;

            }
            else
            {
                //btnEliminar.Enabled = true;

                btnEliminarDetalleProducto.Enabled = true;
                //btnEliminar.Enabled = _btnEliminarDetalle;
            }


        }
        private void grdDataProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.grdDataProductos.ActiveCell == null) return;

            if (this.grdDataProductos.ActiveCell.Column.Key != "i_IdUnidadMedida")
            {

                switch (e.KeyCode)
                {
                    case Keys.Up:
                        grdDataProductos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataProductos.PerformAction(UltraGridAction.AboveCell, false, false);
                        e.Handled = true;
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Down:
                        grdDataProductos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataProductos.PerformAction(UltraGridAction.BelowCell, false, false);
                        e.Handled = true;
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Right:
                        grdDataProductos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataProductos.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdDataProductos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataProductos.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Enter:
                        DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdDataProductos.ActiveCell);
                        grdDataProductos_DoubleClickCell(sender, eventos);
                        e.Handled = true;
                        break;

                }

            }
            else
            {

                switch (e.KeyCode)
                {
                    case Keys.Right:
                        grdDataProductos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataProductos.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdDataProductos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataProductos.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                }


            }
        }
        private void btnEliminarDetalleProducto_Click(object sender, EventArgs e)
        {
            if (grdDataProductos.ActiveRow == null) return;

            if (grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    _objImportaciondetalleproductoDto = new importaciondetalleproductoDto();
                    _objImportaciondetalleproductoDto.v_IdImportacionDetalleProducto = grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdImportacionDetalleProducto"].Value.ToString();
                    _TempDetalleProdDto_Eliminar.Add(_objImportaciondetalleproductoDto);
                    if (grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value != null)
                    {
                        bActualizarNotaIngreso = true;
                        _movimientodetalleDto = new movimientodetalleDto();
                        _movimientodetalleDto.i_IdAlmacen = cboAlmacen.Value.ToString();
                        _movimientodetalleDto.v_IdMovimientoDetalle = grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value.ToString();
                        _TempMovimientodetalle_Eliminar.Add(_movimientodetalleDto);
                    }

                    grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Delete(false);
                }
            }
            else
            {
                grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Delete(false);
            }
            CalcularValoresDetalleGrillaProductos();
        }

        public void RecibirItems(List<UltraGridRow> Filas)
        {


            string NroFactura = null;
            string NroPedido = null;
            string CodigoProveedor = null;
            string IdProveedor = null;
            if (grdDataFob.Rows.Count() == 1)
            {

                foreach (var Fila in grdDataFob.Rows)
                {
                    NroFactura = Fila.Cells["i_IdTipoDocumento"].Text + " " + Fila.Cells["v_SerieDocumento"].Value.ToString() + "-" + Fila.Cells["v_CorrelativoDocumento"].Value.ToString();
                    NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                    CodigoProveedor = Fila.Cells["v_CodCliente"].Value == null ? null : Fila.Cells["v_CodCliente"].Value.ToString().Trim();
                    IdProveedor = Fila.Cells["v_IdCliente"].Value == null ? null : Fila.Cells["v_IdCliente"].Value.ToString();

                }
            }





            bool Repetido = false;
            bool j = false;
            for (int i = 0; i < Filas.Count; i++)
            {
                if (grdDataProductos.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == Filas[i].Cells["v_IdProductoDetalle"].Value.ToString()) != 0)
                {
                    UltraMessageBox.Show("El producto '" + Filas[i].Cells["v_Descripcion"].Value + "' ya se encuentra en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NroFactura"].Value = NroFactura;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NroPedido"].Value = NroPedido;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodCliente"].Value = CodigoProveedor;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdCliente"].Value = IdProveedor;
                        j = true;
                    }
                    else
                    {


                        UltraGridRow row = grdDataProductos.DisplayLayout.Bands[0].AddNew();
                        grdDataProductos.Rows.Move(row, grdDataProductos.Rows.Count() - 1);
                        this.grdDataProductos.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                        //  row.Cells["i_IdUnidad"].Value = "-1";
                        // row.Cells["i_IdTipoDocumento"].Value = "-1";
                        row.Activate();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";

                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value;

                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NroFactura"].Value = NroFactura;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NroPedido"].Value = NroPedido;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodCliente"].Value = CodigoProveedor;
                        grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdCliente"].Value = IdProveedor;


                    }

                }

                CalcularValoresFilaProductos(grdDataProductos.Rows[grdDataProductos.ActiveRow.Index]);
            }

        }


        #endregion

        #region Validaciones /Otros Procedimientos


        private bool ValidarNulosVaciosFob()
        {
            if (grdDataFob.Rows.Where(p => p.Cells["v_IdCliente"].Value == null || p.Cells["v_IdCliente"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todas los proveedores", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["v_IdCliente"].Value == null || x.Cells["v_IdCliente"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdDataFob.Selected.Cells.Add(Row.Cells["v_CodCliente"]);
                grdDataFob.Focus();
                Row.Activate();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["v_CodCliente"];
                this.grdDataFob.ActiveCell = aCell;
                aCell.Activate();
                grdDataFob.PerformAction(UltraGridAction.EnterEditMode);
                return false;
            }

            if (grdDataFob.Rows.Where(p => p.Cells["i_IdTipoDocumento"].Value == null || p.Cells["i_IdTipoDocumento"].Value.ToString() == "-1" || p.Cells["i_IdTipoDocumento"].Value.ToString() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente el Tipo de documento", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["i_IdTipoDocumento"].Value == null || x.Cells["i_IdTipoDocumento"].Value.ToString() == "-1" || x.Cells["i_IdTipoDocumento"].Value.ToString() == string.Empty).FirstOrDefault();
                grdDataFob.Selected.Cells.Add(Row.Cells["i_IdTipoDocumento"]);
                grdDataFob.Focus();
                Row.Activate();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["i_IdTipoDocumento"];
                this.grdDataFob.ActiveCell = aCell;
                aCell.Activate();
                grdDataFob.PerformAction(UltraGridAction.EnterEditMode);
                return false;
            }

            if (grdDataFob.Rows.Where(p => p.Cells["v_SerieDocumento"].Value == null || p.Cells["v_SerieDocumento"].Value.ToString() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente la Serie del documento", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["v_SerieDocumento"].Value == null || x.Cells["v_SerieDocumento"].Value.ToString() == string.Empty).FirstOrDefault();
                grdDataFob.Selected.Cells.Add(Row.Cells["v_SerieDocumento"]);
                grdDataFob.Focus();
                Row.Activate();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["v_SerieDocumento"];
                this.grdDataFob.ActiveCell = aCell;
                aCell.Activate();
                grdDataFob.PerformAction(UltraGridAction.EnterEditMode);
                return false;
            }

            if (grdDataFob.Rows.Where(p => p.Cells["v_CorrelativoDocumento"].Value == null || p.Cells["v_CorrelativoDocumento"].Value.ToString() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente el Número Documento", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["v_CorrelativoDocumento"].Value == null || x.Cells["v_CorrelativoDocumento"].Value.ToString() == string.Empty).FirstOrDefault();
                grdDataFob.Selected.Cells.Add(Row.Cells["v_CorrelativoDocumento"]);
                grdDataFob.Focus();
                Row.Activate();
                grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["v_CorrelativoDocumento"];
                this.grdDataFob.ActiveCell = aCell;
                aCell.Activate();
                grdDataFob.PerformAction(UltraGridAction.EnterEditMode);
                return false;
            }

            return true;

        }


        private bool ValidaCamposNulosVacios()
        {


            if (!ValidarNulosVaciosFob())
            {
                return false;
            }
            else
            {

                if (grdDataFob.Rows.Where(p => p.Cells["d_TipoCambio"].Value == null || p.Cells["d_TipoCambio"].Value.ToString() == string.Empty || decimal.Parse(p.Cells["d_TipoCambio"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese Tipo de Cambio", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["d_TipoCambio"].Value == null || x.Cells["d_TipoCambio"].Value.ToString() == string.Empty || decimal.Parse(x.Cells["d_TipoCambio"].Value.ToString()) <= 0).FirstOrDefault();
                    grdDataFob.Selected.Cells.Add(Row.Cells["d_TipoCambio"]);
                    grdDataFob.Focus();
                    Row.Activate();
                    grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["d_TipoCambio"];
                    this.grdDataFob.ActiveCell = aCell;
                    aCell.Activate();
                    grdDataFob.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }

                if (grdDataFob.Rows.Where(p => p.Cells["d_ValorFob"].Value == null || p.Cells["d_ValorFob"].Value.ToString() == string.Empty || decimal.Parse(p.Cells["d_ValorFob"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese una cantidad para Valor Fob", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataFob.Rows.Where(x => x.Cells["d_ValorFob"].Value == null || x.Cells["d_ValorFob"].Value.ToString() == string.Empty || decimal.Parse(x.Cells["d_ValorFob"].Value.ToString()) <= 0).FirstOrDefault();
                    grdDataFob.Selected.Cells.Add(Row.Cells["d_ValorFob"]);
                    grdDataFob.Focus();
                    Row.Activate();
                    grdDataFob.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["d_ValorFob"];
                    this.grdDataFob.ActiveCell = aCell;
                    aCell.Activate();
                    grdDataFob.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }


                if (grdDataProductos.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value == null || p.Cells["v_IdProductoDetalle"].Value.ToString() == string.Empty).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese una cantidad para Valor Fob", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataProductos.Rows.Where(x => x.Cells["v_IdProductoDetalle"].Value == null || x.Cells["v_IdProductoDetalle"].Value.ToString() == string.Empty).FirstOrDefault();
                    grdDataProductos.Selected.Cells.Add(Row.Cells["v_IdProductoDetalle"]);
                    grdDataProductos.Focus();
                    Row.Activate();
                    grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataFob.ActiveRow.Cells["v_IdProductoDetalle"];
                    this.grdDataProductos.ActiveCell = aCell;
                    aCell.Activate();
                    grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }



                if (grdDataProductos.Rows.Where(p => p.Cells["d_Cantidad"].Value == null || p.Cells["d_Cantidad"].Value.ToString() == string.Empty || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese una cantidad válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataProductos.Rows.Where(x => x.Cells["d_Cantidad"].Value == null || x.Cells["d_Cantidad"].Value.ToString() == string.Empty || decimal.Parse(x.Cells["d_Cantidad"].Value.ToString()) <= 0).FirstOrDefault();
                    grdDataProductos.Selected.Cells.Add(Row.Cells["d_Cantidad"]);
                    grdDataProductos.Focus();
                    Row.Activate();
                    grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["d_Cantidad"];
                    this.grdDataProductos.ActiveCell = aCell;
                    aCell.Activate();
                    grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }

                if (grdDataProductos.Rows.Where(p => p.Cells["d_Precio"].Value == null || p.Cells["d_Precio"].Value.ToString() == string.Empty || decimal.Parse(p.Cells["d_Precio"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese un precio válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataProductos.Rows.Where(x => x.Cells["d_Precio"].Value == null || x.Cells["d_Precio"].Value.ToString() == string.Empty || decimal.Parse(x.Cells["d_Precio"].Value.ToString()) <= 0).FirstOrDefault();
                    grdDataProductos.Selected.Cells.Add(Row.Cells["d_Precio"]);
                    grdDataProductos.Focus();
                    Row.Activate();
                    grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["d_Precio"];
                    //this.grdDataProductos.ActiveCell = aCell;
                    aCell.Activate();
                    grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }


                if (grdDataProductos.Rows.Where(p => p.Cells["d_CostoUnitario"].Value == null || p.Cells["d_CostoUnitario"].Value.ToString() == string.Empty || decimal.Parse(p.Cells["d_CostoUnitario"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Los costos unitarios no se han calculado", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataProductos.Rows.Where(x => x.Cells["d_CostoUnitario"].Value == null || x.Cells["d_CostoUnitario"].Value.ToString() == string.Empty || decimal.Parse(x.Cells["d_CostoUnitario"].Value.ToString()) <= 0).FirstOrDefault();
                    grdDataProductos.Selected.Cells.Add(Row.Cells["d_CostoUnitario"]);
                    grdDataProductos.Focus();
                    Row.Activate();
                    grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["d_CostoUnitario"];
                    //this.grdDataProductos.ActiveCell = aCell;
                    aCell.Activate();
                    grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }


                if (grdDataProductos.Rows.Where(p => p.Cells["d_CostoUnitarioCambio"].Value == null || p.Cells["d_CostoUnitarioCambio"].Value.ToString() == string.Empty || decimal.Parse(p.Cells["d_CostoUnitarioCambio"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Los costos unitarios cambio  no se han calculado", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataProductos.Rows.Where(x => x.Cells["d_CostoUnitarioCambio"].Value == null || x.Cells["d_CostoUnitarioCambio"].Value.ToString() == string.Empty || decimal.Parse(x.Cells["d_CostoUnitarioCambio"].Value.ToString()) <= 0).FirstOrDefault();
                    grdDataProductos.Selected.Cells.Add(Row.Cells["d_CostoUnitarioCambio"]);
                    grdDataProductos.Focus();
                    Row.Activate();
                    grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["d_CostoUnitarioCambio"];
                    //this.grdDataProductos.ActiveCell = aCell;
                    aCell.Activate();
                    grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }

                if (Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1)
                {
                    if (grdDataProductos.Rows.Where(p => p.Cells["v_NroPedido"].Value == null || p.Cells["v_NroPedido"].Value.ToString() == string.Empty).Count() != 0)
                    {
                        UltraMessageBox.Show("Por favor ingrese un número de Pedido válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UltraGridRow Row = grdDataProductos.Rows.Where(x => x.Cells["v_NroPedido"].Value == null || x.Cells["v_NroPedido"].Value.ToString() == string.Empty || decimal.Parse(x.Cells["v_NroPedido"].Value.ToString()) <= 0).FirstOrDefault();
                        grdDataProductos.Selected.Cells.Add(Row.Cells["v_NroPedido"]);
                        grdDataProductos.Focus();
                        Row.Activate();
                        grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["v_NroPedido"];
                        //this.grdDataProductos.ActiveCell = aCell;
                        aCell.Activate();
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                        return false;
                    }
                }

                if (grdDataProductos.Rows.Where(p => p.Cells["v_IdCliente"].Value == null || p.Cells["v_IdCliente"].Value.ToString() == string.Empty).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese un código de Proveedor en  la Grilla Detalle de Productos ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdDataProductos.Rows.Where(x => x.Cells["v_IdCliente"].Value == null || x.Cells["v_IdCliente"].Value.ToString() == string.Empty).FirstOrDefault();
                    grdDataProductos.Selected.Cells.Add(Row.Cells["v_CodCliente"]);
                    grdDataProductos.Focus();
                    Row.Activate();
                    grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["v_CodCliente"];
                    this.grdDataProductos.ActiveCell = aCell;
                    aCell.Activate();
                    grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }

                foreach (var Fila in grdDataProductos.Rows)
                {
                    int Index = Fila.Index;
                    string NroFactura = String.Empty;
                    if (Fila.Cells["v_NroFactura"].Value == null)
                    {
                        NroFactura = "";
                    }
                    else
                    {
                        NroFactura = Fila.Cells["v_NroFactura"].Value.ToString();
                    }
                    string[] split = NroFactura.Split(new Char[] { ' ', ',', '.', ':', '-' });
                    // string[] split = NroFactura.Split(new Char[] { ' ', ',', '.', ':'});
                    if (split.Count() < 3)
                    {
                        UltraMessageBox.Show("Por favor ingrese  Número de Factura en  la Grilla Detalle de Productos ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        grdDataProductos.Selected.Cells.Add(Fila.Cells["v_NroFactura"]);
                        grdDataProductos.Focus();
                        Fila.Activate();
                        grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["v_NroFactura"];
                        Fila.Cells["v_NroFactura"].Activate();
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                        return false;

                    }
                    else if (split[0] == "--Seleccionar--")
                    {
                        UltraMessageBox.Show("Por favor ingrese  Número de Factura en  la Grilla Detalle de Productos ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        grdDataProductos.Selected.Cells.Add(Fila.Cells["v_NroFactura"]);
                        grdDataProductos.Focus();
                        Fila.Activate();
                        grdDataProductos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdDataProductos.ActiveRow.Cells["v_NroFactura"];
                        Fila.Cells["v_NroFactura"].Activate();
                        // this.grdDataProductos.ActiveCell = aCell;
                        grdDataProductos.PerformAction(UltraGridAction.EnterEditMode);
                        return false;
                    }


                    #region ValidarProductosPorProveedorPedido


                    foreach (var itemBusqueda in grdDataProductos.Rows)
                    {

                        if (itemBusqueda.Index != Fila.Index && itemBusqueda.Cells["v_NroPedido"].Value != null && Fila.Cells["v_NroPedido"].Value != null && itemBusqueda.Cells["v_NroPedido"].Value.ToString() == Fila.Cells["v_NroPedido"].Value.ToString() && itemBusqueda.Cells["v_IdProductoDetalle"].Value.ToString() == Fila.Cells["v_IdProductoDetalle"].Value.ToString())
                        {
                            UltraMessageBox.Show("El Producto  : " + itemBusqueda.Cells["v_CodigoInterno"].Value.ToString() + " se encuentra registrado más de una vez para el Pedido : " + itemBusqueda.Cells["v_NroPedido"].Value.ToString(), "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }



                    #endregion
                }


            }


            return true;

        }
        private bool ValidarProveedores()
        {

            bool Existe = true;
            List<string> ListaProveedoresFob = new List<string>();
            List<string> ListaProveedoresProducto = new List<string>();
            string pstrProveedorFob, pstrProveedorProducto;

            foreach (UltraGridRow Fila in grdDataFob.Rows)
            {
                if (Fila.Cells["v_IdCliente"].Value != null)
                {
                    pstrProveedorFob = Fila.Cells["v_IdCliente"].Value.ToString();
                    if (!ListaProveedoresFob.Contains(pstrProveedorFob))
                    {
                        ListaProveedoresFob.Add(pstrProveedorFob);
                    }
                }
            }


            foreach (UltraGridRow Fila in grdDataProductos.Rows)
            {
                if (Fila.Cells["v_IdCliente"].Value != null)
                {
                    pstrProveedorProducto = Fila.Cells["v_IdCliente"].Value.ToString();
                    if (!ListaProveedoresProducto.Contains(pstrProveedorProducto))
                    {
                        ListaProveedoresProducto.Add(pstrProveedorProducto);
                    }
                }
            }

            foreach (var proveedor in ListaProveedoresFob)
            {
                if (!ListaProveedoresProducto.Contains(proveedor))
                {
                    Existe = false;

                }

            }

            return Existe;
        }


        private bool ValidarNroPedidos()
        {

            bool Existe = true;
            List<string> ListaPedidosFob = new List<string>();
            List<string> ListaPedidosProducto = new List<string>();
            string pstrProveedorFob, pstrProveedorProducto;

            foreach (UltraGridRow Fila in grdDataFob.Rows)
            {
                if (Fila.Cells["v_NroPedido"].Value != null)
                {
                    pstrProveedorFob = Fila.Cells["v_NroPedido"].Value.ToString();
                    if (!ListaPedidosFob.Contains(pstrProveedorFob))
                    {
                        ListaPedidosFob.Add(pstrProveedorFob);
                    }
                }
            }


            foreach (UltraGridRow Fila in grdDataProductos.Rows)
            {
                if (Fila.Cells["v_NroPedido"].Value != null)
                {
                    pstrProveedorProducto = Fila.Cells["v_NroPedido"].Value.ToString();
                    if (!ListaPedidosProducto.Contains(pstrProveedorProducto))
                    {
                        ListaPedidosProducto.Add(pstrProveedorProducto);
                    }
                }
            }

            foreach (var proveedor in ListaPedidosFob)
            {
                if (!ListaPedidosProducto.Contains(proveedor))
                {
                    Existe = false;

                }

            }

            return Existe;
        }


        private void ValidarFechas()
        {

            if (DateTime.Now.Year.ToString().Trim() == txtPeriodo.Text.Trim())
            {
                if (strModo == "Nuevo")
                {
                    dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //dtpFechaRegistro.MinDate = dtpFechaEmision.Value;
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());


                    dtpFechaVencimiento.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaVencimiento.MinDate = dtpFechaEmision.Value;
                    //  dtpFechaVencimiento.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    dtpFechaLlegada.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //  dtpFechaLlegada.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaLlegada.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    dtpFechaArrivo.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //   dtpFechaArrivo.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaArrivo.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());


                    dtpFechaDoc1.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //  dtpFechaDoc1.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaDoc1.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    dtpFechaDoc2.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //  dtpFechaDoc2.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaDoc2.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    dtpFechaDoc3.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //  dtpFechaDoc3.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaDoc3.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    dtpFechaDoc4.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //  dtpFechaDoc4.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaDoc4.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                }
                else
                {
                    if (int.Parse(_objImportacionDto.v_Mes) <= int.Parse(txtMes.Text.ToString()))
                    {
                        dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());
                        dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());
                        //  dtpFechaVencimiento.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());
                        dtpFechaLlegada.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());
                        dtpFechaArrivo.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());
                        dtpFechaDoc1.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());
                        dtpFechaDoc2.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());
                        dtpFechaDoc3.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());
                        dtpFechaDoc4.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString().Trim()))).ToString()).ToString());


                    }
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaVencimiento.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    //  dtpFechaLlegada.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    // dtpFechaArrivo.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    //  dtpFechaDoc1.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    // dtpFechaDoc2.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    //dtpFechaDoc3.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    //dtpFechaDoc4.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());


                }


            }
            else
            {
                if (strModo == "Nuevo")
                {


                    dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaVencimiento.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaLlegada.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaArrivo.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaDoc1.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaDoc2.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaDoc3.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaDoc4.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());

                }
                dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                dtpFechaVencimiento.MinDate = dtpFechaEmision.Value;
                //  dtpFechaVencimiento.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                //  dtpFechaLlegada.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaLlegada.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                // dtpFechaArrivo.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaArrivo.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                //dtpFechaDoc1.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaDoc1.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                //dtpFechaDoc2.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaDoc2.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                //dtpFechaDoc3.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaDoc3.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                //dtpFechaDoc4.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaDoc4.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());

            }
        }

        private void GenerarNumeroRegistro()
        {
            OperationResult objOperationResult = new OperationResult();
            string Mes;
            Mes = int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? ("0" + dtpFechaRegistro.Value.Month.ToString()).Trim() : dtpFechaRegistro.Value.Month.ToString();

            _ListadoImportacionCambioFecha = _objImportacionBL.ObtenerListadoImportaciones(ref objOperationResult, txtPeriodo.Text.Trim(), Mes);
            if (_ListadoImportacionCambioFecha.Count != 0)
            {
                int MaxMovimiento;
                MaxMovimiento = _ListadoImportacionCambioFecha.Count() > 0 ? int.Parse(_ListadoImportacionCambioFecha[_ListadoImportacionCambioFecha.Count() - 1].Value1.ToString()) : 0;
                MaxMovimiento++;
                txtCorrelativo.Text = MaxMovimiento.ToString("00000000");
                txtMes.Text = int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? 0 + dtpFechaRegistro.Value.Month.ToString() : dtpFechaRegistro.Value.Month.ToString();

            }
            else
            {
                txtCorrelativo.Text = "00000001";
                txtMes.Text = int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? 0 + dtpFechaRegistro.Value.Month.ToString() : dtpFechaRegistro.Value.Month.ToString();
            }

        }
        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {


            if (Scroll >= int.Parse(dtpFechaRegistro.Value.Month.ToString()))
            {
                dtpFechaVencimiento.MinDate = dtpFechaEmision.MinDate;
                dtpFechaVencimiento.Value = dtpFechaEmision.Value; // Solo esto,antes de agregar periodo
            }
            else
            {
                dtpFechaVencimiento.ResetText();
                dtpFechaVencimiento.MinDate = DateTime.Parse("01" + "/" + "01" + "/" + "1753");
                dtpFechaVencimiento.MaxDate = DateTime.Parse("31" + "/" + "12" + "/" + "9998");
                dtpFechaVencimiento.Value = dtpFechaEmision.Value;

            }
            Scroll = int.Parse(dtpFechaRegistro.Value.Month.ToString());


            if (strModo == "Nuevo")
            {
                GenerarNumeroRegistro();
            }

            else
            {
                string AnioCambiado = dtpFechaRegistro.Value.Year.ToString().Trim();
                string MesCambiado = int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? ("0" + dtpFechaRegistro.Value.Month.ToString()).Trim() : dtpFechaRegistro.Value.Month.ToString();
                if (MesCambiado.Trim() != _objImportacionDto.v_Mes.Trim() || AnioCambiado != _objImportacionDto.v_Periodo.Trim())
                {
                    GenerarNumeroRegistro();
                    foreach (UltraGridRow Fila in grdDataProductos.Rows)
                    {
                        Fila.Cells["i_RegistroEstado"].Value = "Modificado";


                    }
                    bActualizarNotaIngreso = true;
                }
                else
                {
                    txtPeriodo.Text = _objImportacionDto.v_Periodo.Trim();
                    txtMes.Text = _objImportacionDto.v_Mes.Trim();
                    txtCorrelativo.Text = _objImportacionDto.v_Correlativo.Trim();

                }
            }

            if (_objCierreMensualBL.VerificarMesCerrado(txtPeriodo.Text.Trim(), txtMes.Text.Trim(), (int)ModulosSistema.Compras))
            {

                btnGuardar.Visible = false;
                this.Text = "Registro de  Importación [MES CERRADO]";
            }

            else
            {
                btnGuardar.Visible = true;
                this.Text = "Registro de  Importación";
            }
        }

        private void dtpFechaVencimiento_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtTipoCambio.Text = _objImportacionBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
        }

        private void txtTotalIgv_ValueChanged(object sender, EventArgs e)
        {
            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();

            }

            CalcularSubTotal();

        }

        private void txtUtilidad_TextChanged(object sender, EventArgs e)
        {
            CalcularPrecioVentaProductos();
        }
        private void grdDataFob_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdDataFob.ActiveCell == null) return;
            CalcularValoresFilaFob(grdDataFob.Rows[grdDataFob.ActiveRow.Index]);
            switch (grdDataFob.ActiveCell.Column.Key)
            {
                case "v_SerieDocumento":
                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value != null && grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString() != string.Empty)
                    {
                        string Serie;
                        Serie = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString();
                        grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value = (int.Parse(Serie)).ToString("0000");
                        //grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_SerieDocumento"].Value = Serie;
                    }

                    break;
                case "v_CorrelativoDocumento":

                    if (grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value != null && grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString() != string.Empty)
                    {
                        string Correlativo;
                        Correlativo = grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString();
                        if (Correlativo.Length >= 8)
                        {
                            grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value = Correlativo;
                        }
                        else
                        {
                            string Ceros = "0";
                            int i = 1;
                            int NumeroCeros = 8 - Correlativo.Length;
                            while (i < NumeroCeros)
                            {
                                Ceros = Ceros + "0";
                                i++;
                            }
                            grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value = Ceros + Correlativo; //(int.Parse(Correlativo)).ToString("00000000");

                        }
                        // grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value = (int.Parse(Correlativo)).ToString("00000000");
                        // grdDataFob.Rows[grdDataFob.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value = Correlativo;
                    }
                    break;
            }
            CalcularValoresDetalleGrillaProductos();
        }

        private void cboDocumento_TextChanged(object sender, EventArgs e)
        {
            ModificarNotaIngreso();
        }

        private void cboSerieDoc_TextChanged(object sender, EventArgs e)
        {
            ModificarNotaIngreso();
        }

        private void txtNumeroDoc_TextChanged(object sender, EventArgs e)
        {
            ModificarNotaIngreso();
        }


        private void cboDocumento_Validating(object sender, CancelEventArgs e)
        {
            //if (cboSerieDoc.Value.ToString() != "-1" && cboDocumento.Value.ToString() != "-1" && txtNumeroDoc.Text != string.Empty)
            //{

            //    if (_objImportacionBL.ValidarNumeroRegistro(int.Parse(cboSerieDoc.Value.ToString()), int.Parse(cboSerieDoc.Value.ToString()), txtNumeroDoc.Text.Trim(), _objImportacionDto.v_IdImportacion))
            //    {
            //        UltraMessageBox.Show("Este Documento ya ha sido Registrado ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        txtNumeroDoc.Focus();
            //        return;
            //    }

            //}
        }

        private void cboSerieDoc_Validating(object sender, CancelEventArgs e)
        {
            //if (cboSerieDoc.Value.ToString() != "-1" && cboDocumento.Value.ToString() != "-1" && txtNumeroDoc.Text != string.Empty)
            //{

            //    if (_objImportacionBL.ValidarNumeroRegistro(int.Parse(cboSerieDoc.Value.ToString()), int.Parse(cboSerieDoc.Value.ToString()), txtNumeroDoc.Text.Trim(), _objImportacionDto.v_IdImportacion))
            //    {
            //        UltraMessageBox.Show("Este Documento ya ha sido Registrado ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        txtNumeroDoc.Focus();
            //        return;
            //    }

            //}
        }

        private void txtNumeroDoc_Validating(object sender, CancelEventArgs e)
        {
            if (cboSerieDoc.Value.ToString() != "-1" && cboDocumento.Value.ToString() != "-1" && txtNumeroDoc.Text != string.Empty)
            {

                if (_objImportacionBL.ValidarNumeroRegistro(int.Parse(cboDocumento.Value.ToString()), int.Parse(cboSerieDoc.Value.ToString()), txtNumeroDoc.Text.Trim(), _objImportacionDto.v_IdImportacion))
                {
                    UltraMessageBox.Show("Este Documento ya ha sido Registrado ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNumeroDoc.Focus();
                    return;
                }

            }
        }

        #endregion



        private void CalcularPrecioVentaProductos()
        {
            foreach (UltraGridRow Fila in grdDataProductos.Rows)
            {
                if (Fila.Cells["d_PrecioVenta"].Value == null) { Fila.Cells["d_PrecioVenta"].Value = "0"; }
                if (Fila.Cells["d_CostoUnitario"].Value == null) { Fila.Cells["d_CostoUnitario"].Value = "0"; }

                //if (txtUtilidad.Text.Trim() == string.Empty)
                //{

                //    txtUtilidad.Text = "0";
                //}

                decimal Utilidad = txtUtilidad.Text.Trim() == string.Empty ? 0 : decimal.Parse(txtUtilidad.Text.ToString());
                Fila.Cells["d_PrecioVenta"].Value = decimal.Parse(Fila.Cells["d_CostoUnitario"].Text) + (Utilidad * (decimal.Parse(Fila.Cells["d_CostoUnitario"].Text)) / 100);
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }



        private void ModificarNotaIngreso()
        {
            bActualizarNotaIngreso = true;

            foreach (UltraGridRow Fila in grdDataProductos.Rows)
            {
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }

        }

        private void grdDataProductos_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "v_NroFactura")
            {
                Procesos.frmBusquedaFacturasImportacion frmBusquedaFacturas = new Procesos.frmBusquedaFacturasImportacion(grdDataFob);
                frmBusquedaFacturas.ShowDialog();
                if (frmBusquedaFacturas._NroFactura != null)
                {
                    grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_CodCliente"].Value = frmBusquedaFacturas._CodProveedor.ToString().Trim();
                    grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_IdCliente"].Value = frmBusquedaFacturas._IdProveedor.ToString();
                    grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NroFactura"].Value = frmBusquedaFacturas._NroFactura.ToString().Trim();
                    grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["v_NroPedido"].Value = frmBusquedaFacturas._NroPedido == null ? null : frmBusquedaFacturas._NroPedido.ToString();
                    grdDataProductos.Rows[grdDataProductos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                    CalcularValoresFilaProductos(grdDataProductos.Rows[grdDataProductos.ActiveRow.Index]);
                }
            }
        }



        private void CalcularIgv()
        {
            if (txtProm.Text.Trim() == null || txtProm.Text.Trim() == string.Empty)
            {
                txtTotalIgv.Text = txtTax.Text.Trim();

            }
            else if (txtTax.Text == null || txtTax.Text.Trim() == string.Empty)
            {
                txtTotalIgv.Text = txtProm.Text.Trim();
            }
            else
            {

                txtTotalIgv.Text = (decimal.Parse(txtProm.Text.ToString()) + decimal.Parse(txtTax.Text.ToString())).ToString();
            }

        }

        private void txtTax_Validated(object sender, EventArgs e)
        {
            CalcularIgv();
        }

        private void txtProm_Validated(object sender, EventArgs e)
        {
            CalcularIgv();
        }

        private void txtTotalIgv_MaskChanged(object sender, MaskChangedEventArgs e)
        {

        }

        private void groupBox3_Click(object sender, EventArgs e)
        {

        }

        private void txtUtilidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimal2UltraTextBox(txtUtilidad, e);
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {


            foreach (UltraGridRow Fila in grdDataProductos.Rows)
            {
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }

        private void cboMoneda_ValueChanged(object sender, EventArgs e)
        {

            if (cboMoneda.Value == null || cboMoneda.Value.ToString() == "-1") return;

            OperationResult objOperationResult = new OperationResult();
            if ((TotalSoles == 0 && TotalDolares > 0) || (TotalDolares == 0 && TotalSoles > 0))
            {
                var DetallesGastos = _objImportacionBL.ObtenerImportacionDetallesGastos(ref objOperationResult, _objImportacionDto.v_IdImportacion);
                if (DetallesGastos.Any())
                {
                    decimal sol = 0, dol = 0;
                    foreach (var item in DetallesGastos)
                    {
                        switch (item.i_IdMoneda.Value.ToString())
                        {
                            case "1"://Soles
                                sol = item.d_ValorVenta.Value + item.d_NAfectoDetraccion.Value + sol;
                                dol = Utils.Windows.DevuelveValorRedondeado(((item.d_ValorVenta.Value + item.d_NAfectoDetraccion.Value) / item.d_TipoCambio.Value), 2) + dol;

                                break;
                            case "2": //Dolares
                                dol = item.d_ValorVenta.Value + item.d_NAfectoDetraccion.Value + dol;
                                sol = Utils.Windows.DevuelveValorRedondeado(((item.d_ValorVenta.Value + item.d_NAfectoDetraccion.Value) * item.d_TipoCambio.Value), 2) + sol;
                                break;
                        }
                    }

                    TotalSoles = Utils.Windows.DevuelveValorRedondeado(sol, 2);
                    TotalDolares = Utils.Windows.DevuelveValorRedondeado(dol, 2);

                }
            }

            if (cboMoneda.Value.ToString() == "1")
            {
                txtOtrosGastos.Text = TotalSoles.ToString("0.0000");
            }
            else if (cboMoneda.Value.ToString() == "2")
            {
                txtOtrosGastos.Text = TotalDolares.ToString("0.0000");
            }

            if (grdDataProductos.Rows.Count() > 0)
            {
                CalcularValoresDetalleGrillaProductos();
            }

            // CalcularSubTotal();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnImprimirTipoFormato_Click(object sender, EventArgs e)
        {
            Reportes.Compras.frmRegistroImportaciones frm = new Reportes.Compras.frmRegistroImportaciones(_objImportacionDto.v_IdImportacion, rbtnSoles.Checked ? 1 : 2);
            frm.ShowDialog();
        }

        private void btnSalirPanel_Click(object sender, EventArgs e)
        {
            pnlVariosTiposImpresion.Visible = false;
            btnImprimir.Enabled = true;
            btnGuardar.Enabled = true;
        }

        private void linkAsiento_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void btnExtraer_Click(object sender, EventArgs e)
        {

            try
            {
                var f = new frmExtraerDetallesMovimientosCompras();
                BindingList<ordendecompraDto> ListaCabeceraOrdenCompra = new BindingList<ordendecompraDto>();
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {


                    var objOperationResult = new OperationResult();
                    var firstOrDefault = f.ListaRetornoImportacionDetalleProducto.FirstOrDefault();
                    if (firstOrDefault != null && !string.IsNullOrWhiteSpace(firstOrDefault.IdOrdenCompra))
                    {
                        var CabeceraOrdenCompra = new OrdenCompraBL().ListaObtenerOrdenCompraExtraccion(ref objOperationResult, firstOrDefault.IdOrdenCompra);

                        grdDataFob.DataSource = CabeceraOrdenCompra;

                        for (int i = 0; i < grdDataFob.Rows.Count(); i++)
                        {
                            grdDataFob.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                            grdDataFob.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                        }
                        cboDocumentoReferencia.Value = CabeceraOrdenCompra.FirstOrDefault().i_IdTipoDocumentoReferencia.ToString();
                        txtNumeroDocumentoReferencia.Text = CabeceraOrdenCompra.FirstOrDefault().v_SerieDocumentoReferencia + "-" + CabeceraOrdenCompra.FirstOrDefault().v_CorrelativoDocumentoReferencia;
                        _objImportacionDto.v_IdDocumentoReferencia = CabeceraOrdenCompra.FirstOrDefault().v_IdImportacion;
                    }
                }
                if (f.ListaRetornoImportacionDetalleProducto.Any())
                {

                    _TempDetalleProdDto_Eliminar.AddRange(grdDataProductos.Rows.Select(r => (importaciondetalleproductoDto)r.ListObject));
                    grdDataProductos.DataSource = f.ListaRetornoImportacionDetalleProducto;

                }

                for (int i = 0; i < grdDataProductos.Rows.Count(); i++)
                {
                    grdDataProductos.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                    grdDataProductos.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";



                }
                CalcularValoresDetalleGrillaProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void grdDataFob_ClickCellButton(object sender, CellEventArgs e)
        {
            if (grdDataFob.ActiveRow == null) return;
            switch (e.Cell.Column.Key)
            {
                case "_Actualizar":
                    if (UltraMessageBox.Show("¿Desea asignar a todos los productos el Nro. de Factura y Proveedor de esta fila ?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (ValidarNulosVaciosFob())
                        {
                            foreach (var row in grdDataProductos.Rows)
                            {
                                string NroFactura = string.Empty, NroPedido = string.Empty, CodigoProveedor = string.Empty, IdProveedor = string.Empty;
                                NroFactura = grdDataFob.ActiveRow.Cells["i_IdTipoDocumento"].Text + " " + grdDataFob.ActiveRow.Cells["v_SerieDocumento"].Value.ToString() + "-" + grdDataFob.ActiveRow.Cells["v_CorrelativoDocumento"].Value.ToString();
                                NroPedido = grdDataFob.ActiveRow.Cells["v_NroPedido"].Value == null ? null : grdDataFob.ActiveRow.Cells["v_NroPedido"].Value.ToString();
                                CodigoProveedor = grdDataFob.ActiveRow.Cells["v_CodCliente"].Value == null ? null : grdDataFob.ActiveRow.Cells["v_CodCliente"].Value.ToString();
                                IdProveedor = grdDataFob.ActiveRow.Cells["v_IdCliente"].Value == null ? null : grdDataFob.ActiveRow.Cells["v_IdCliente"].Value.ToString();
                                row.Cells["v_NroFactura"].Value = NroFactura;
                                row.Cells["v_NroPedido"].Value = NroPedido;
                                row.Cells["v_CodCliente"].Value = CodigoProveedor;
                                row.Cells["v_IdCliente"].Value = IdProveedor;
                                row.Cells["i_RegistroEstado"].Value = "Modificado";

                            }
                            CalcularValoresDetalleGrillaProductos();
                        }
                    }


                    break;
            }

        }
    }
}
