using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Contabilidad.BL;
using SAMBHS.CommonWIN.BL;
using Infragistics.Win.UltraWinGrid;
using LoadingClass;
using SAMBHS.Compra.BL;
using SAMBHS.Common.BE;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmPago : Form
    {
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        UltraCombo ucTipoDocumento = new UltraCombo();
        UltraCombo ucFormasPago = new UltraCombo();
        PagoBL _objPagoBL = new PagoBL();
        List<KeyValueDTO> _ListadoPagos = new List<KeyValueDTO>();
        List<string[]> _ListaCompras = new List<string[]>();
        pagoDto _pagoDto = new pagoDto();
        pagodetalleDto _pagodetalleDto = new pagodetalleDto();
        public string strIdPago = "";
        string _Mode, strModo = "Nuevo";
        int _MaxV, _ActV;
        readonly bool _sePuedeEliminar;
        #region Temporales Detalles de Pago
        List<pagodetalleDto> _TempDetalle_AgregarDto = new List<pagodetalleDto>();
        List<pagodetalleDto> _TempDetalle_ModificarDto = new List<pagodetalleDto>();
        List<pagodetalleDto> _TempDetalle_EliminarDto = new List<pagodetalleDto>();
        #endregion
        public frmPago(string Modo, string IdPago, List<string[]> ListaVentas, bool sePuedeEliminar = false)
        {
            strModo = Modo;
            strIdPago = IdPago;
            _ListaCompras = ListaVentas;
            _sePuedeEliminar = sePuedeEliminar;
            InitializeComponent();

        }
        private void frmPago_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            UltraStatusbarManager.Inicializar(ultraStatusBar1);

            #region Controla Cierre de Mes
            if (new CierreMensualBL().VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString(), DateTime.Now.Month.ToString(), (int)ModulosSistema.CtasPagar))
            {
                btnGuardar.Visible = false;
                this.Text = "Pago [MES CERRADO]";
            }
            else
            {

                btnGuardar.Visible = true;
                this.Text = "Pago";
            }
            #endregion

            #region ControlAcciones
            //var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmCobranza", Globals.ClientSession.i_RoleId);
            //_btnBuscarPendientes = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmCobranza_PENDIENTES", _formActions);
            //_btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmCobranza_SAVE", _formActions);
            //_btnImprimir = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmCobranza_PRINT", _formActions);
            //btnGuardar.Enabled = _btnGuardar;
            //btnBuscarPendientes.Enabled = _btnBuscarPendientes;
            //BtnImprimir.Enabled = _btnImprimir;
            #endregion

            CargarCombos();
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString("00");
            ObtenerListadoPagos(txtPeriodo.Text, txtMes.Text.Trim());
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();
            btnEliminarPago.Visible = btnGuardar.Visible && _sePuedeEliminar;
        }
        private void ObtenerListadoPagos(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoPagos = _objPagoBL.ObtenerListadoPagos(ref objOperationResult, pstrPeriodo, pstrMes, int.Parse(cboTipoDocumento.Value.ToString()));
            switch (strModo)
            {
                case "Edicion":
                    CargarCabecera(strIdPago);
                    if (_pagoDto.i_IdEstado == 0) btnGuardar.Enabled = false;
                    cboTipoDocumento.Enabled = false;
                    cboMoneda.Enabled = false;
                    txtTipoCambio.Enabled = false;
                    BtnImprimir.Enabled = true;
                    break;
                case "Nuevo":
                    if (_ListadoPagos.Count != 0)
                    {
                        _MaxV = _ListadoPagos.Count() - 1;
                        _ActV = _MaxV;
                        // LimpiarCabecera();
                        txtCorrelativo.Text = (int.Parse(_ListadoPagos[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        _pagoDto = new pagoDto();
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        // LimpiarCabecera();
                        _MaxV = 1;
                        _ActV = 1;
                        _pagoDto = new pagoDto();
                    }
                    txtTipoCambio.Text = _objPagoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
                    cboMedioPago.Value = "1";
                    cboEstado.Value = "1";
                    cboEstado.Enabled = false;
                    if (_ListaCompras == null)
                    {
                        CargarDetalle("");
                    }
                    else
                    {
                        CargarDetalle("");
                        RecibirComprasFacturadas(_ListaCompras);

                    }
                    BtnImprimir.Enabled = false;
                    break;

                case "Guardado":
                    _MaxV = _ListadoPagos.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdPago == "" | strIdPago == null)
                    {
                        CargarCabecera(_ListadoPagos[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdPago);
                    }
                    BtnImprimir.Enabled = true;
                    break;
                case "Consulta":
                    if (_ListadoPagos.Count != 0)
                    {
                        _MaxV = _ListadoPagos.Count() - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoPagos[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoPagos[_MaxV].Value2);
                        _Mode = "Edit";
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        // LimpiarCabecera();
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        _pagoDto = new pagoDto();
                        txtTipoCambio.Text = _objPagoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
                        txtMes.Enabled = true;
                    }
                    break;


                case "PagoRapido":
                    if (_ListadoPagos.Count != 0)
                    {
                        _MaxV = _ListadoPagos.Count() - 1;
                        _ActV = _MaxV;
                        // LimpiarCabecera();
                        txtCorrelativo.Text = (int.Parse(_ListadoPagos[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        _pagoDto = new pagoDto();
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        // LimpiarCabecera();
                        _MaxV = 1;
                        _ActV = 1;
                        _pagoDto = new pagoDto();
                    }
                    txtTipoCambio.Text = _objPagoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
                    cboMedioPago.Value = "1";
                    cboEstado.Value = "1";
                    cboEstado.Enabled = false;

                    if (_ListaCompras == null)
                    {
                        // CargarDetalle("");
                        CargarDetalleDesdeCompras(strIdPago);

                    }
                    else
                    {
                        //CargarDetalle("");
                        CargarDetalleDesdeCompras(strIdPago);
                        RecibirComprasFacturadas(_ListaCompras);

                    }
                    BtnImprimir.Enabled = false;
                    cboMoneda.Enabled = false;
                    btnBuscarPendientes.Enabled = false;
                    break;
            }
        }
        private void CargarCabecera(string IdPago)
        {
            OperationResult objOperationResult = new OperationResult();
            _pagoDto = new pagoDto();
            _pagoDto = _objPagoBL.ObtenerPagoCabecera(ref objOperationResult, IdPago);
            if (_pagoDto != null)
            {
                txtPeriodo.Text = _pagoDto.v_Periodo;


                cboTipoDocumento.Value = _pagoDto.i_IdTipoDocumento.ToString();
                cboEstado.Value = _pagoDto.i_IdEstado.ToString();
                cboMedioPago.Value = _pagoDto.i_IdMedioPago.ToString();
                txtGlosa.Text = _pagoDto.v_Glosa;
                txtNombre.Text = _pagoDto.v_Nombre;
                dtpFechaRegistro.Value = _pagoDto.t_FechaRegistro.Value;
                txtCorrelativo.Text = _pagoDto.v_Correlativo;
                txtMes.Text = int.Parse(_pagoDto.v_Mes.ToString().Trim()).ToString("00");
                txtTipoCambio.Text = _pagoDto.d_TipoCambio.ToString();
                txtTipoCambio.Text = _pagoDto.d_TipoCambio.ToString();
                txtTotalDolares.Text = _pagoDto.d_TotalDolares.ToString();
                txtTotalSoles.Text = _pagoDto.d_TotalSoles.ToString();
                cboMoneda.Value = _pagoDto.i_IdMoneda.ToString();
                _Mode = "Edit";
                CargarDetalle(_pagoDto.v_IdPago);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar el pago", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void CargarDetalle(string IdPago)
        {
            OperationResult objOperationResult = new OperationResult();
            try
            {
                grdData.DataSource = _objPagoBL.ObtenerPagoDetalle(ref objOperationResult, IdPago);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //DevolverNombreRelaciones();
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["Moneda"].Value = cboMoneda.Value.ToString() == "1" ? "S/." : "US$.";
            }
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
            }
            CalcularTotales();
        }
        private void ObtenerCorrelativoPago(int IdDocumento)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoPagos = _objPagoBL.ObtenerListadoPagos(ref objOperationResult, txtPeriodo.Text.Trim(), txtMes.Text.Trim(), IdDocumento);
            if (_ListadoPagos.Count != 0)
            {
                _MaxV = _ListadoPagos.Count() - 1;
                _ActV = _MaxV;
                // LimpiarCabecera();
                txtCorrelativo.Text = (int.Parse(_ListadoPagos[_MaxV].Value1) + 1).ToString("00000000");
            }
            else
            {
                txtCorrelativo.Text = "00000001";
                _MaxV = 1;
                _ActV = 1;
            }
        }
        private void CargarDetalleDesdeCompras(string IdCompras)
        {
            OperationResult objOperationResult = new OperationResult();
            BindingList<GrillaPagoDetalleDto> Compras = new BindingList<GrillaPagoDetalleDto>();
            try
            {
                Compras = _objPagoBL.ObtenerDetalleCompras(ref objOperationResult, IdCompras);
                grdData.DataSource = Compras;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //DevolverNombreRelaciones();
            if (Compras.Count() > 0)
            {

                cboMoneda.Value = Compras.FirstOrDefault().i_IdMoneda.Value.ToString();

            }
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["Moneda"].Value = cboMoneda.Value.ToString() == "1" ? "S/." : "US$.";
            }
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
            }
            CalcularTotales();
        }
        #region Comportamiento Controles

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value != null && cboTipoDocumento.Value.ToString() == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
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

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            if (strModo == "Edicion") return;

            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value | p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
            ObtenerCorrelativoPago(int.Parse(cboTipoDocumento.Value.ToString()));
            bool TieneCuentaContable = false;

            if (cboTipoDocumento.Value != null && cboTipoDocumento.Value != "-1")
            {
                OperationResult objOperationResult = new OperationResult();
                int idMoneda = _objPagoBL.DevuelveMonedaPorDocumento(ref objOperationResult, int.Parse(cboTipoDocumento.Value.ToString()), out TieneCuentaContable);
                //cboMoneda.Value = 
                if (strModo == "PagoRapido")
                {
                    if (idMoneda != int.Parse(cboMoneda.Value.ToString()))
                    {
                        UltraStatusbarManager.MarcarError(ultraStatusBar1, "Este documento  tiene una moneda diferente a la Moneda del Pago", timer1);
                        btnGuardar.Enabled = false;
                        return;
                    }

                }
                else
                {

                    if (objOperationResult.Success == 1)
                    {
                        cboMoneda.Value = idMoneda.ToString();
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (!TieneCuentaContable)
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Este documento no tiene una cuenta contable relacionada", timer1);
                    btnGuardar.Enabled = false;
                    //  cboMoneda.Enabled = false;
                }
                else
                {
                    UltraStatusbarManager.Inicializar(ultraStatusBar1);
                    btnGuardar.Enabled = true;
                    cboMoneda.Enabled = false;
                }
            }

        }

        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
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
        #endregion

        #region ConsultaPagosPendientes

        private void btnBuscarPendientes_Click(object sender, EventArgs e)
        {
            //if (cboTipoDocumento.Value != null && cboTipoDocumento.Value.ToString() != "-1" && cboMoneda.SelectedValue.ToString() != "-1")
            //{
            if (uvValidarPendientes.Validate(true, false).IsValid)
            {
                if (cboTipoDocumento.Value == null || cboTipoDocumento.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Por favor elija un Tipo Documento ", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboTipoDocumento.Focus();
                    return;
                }
                txtTipoCambio.Text = string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? "0" : txtTipoCambio.Text;

                if (decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por favor elija un tipo de cambio primero", "Sistema", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    txtTipoCambio.Focus();
                }
                else
                {
                    // frmCobranzaPendienteConsulta frm = new frmCobranzaPendienteConsulta(null, true, false, cboTipoDocumento.Text);
                    frmPagoPendienteConsulta frm = new frmPagoPendienteConsulta(null, true, false, cboTipoDocumento.Text);
                    frm.ShowDialog();

                    if (grdData.Rows.Count() > 0)
                    {
                        cboTipoDocumento.Enabled = false;
                        cboMoneda.Enabled = false;
                    }
                }
            }
            //}
        }


        public void RecibirItems(List<UltraGridRow> Filas)
        {
            bool Repetido = false;
            for (int i = 0; i < Filas.Count; i++)
            {
                if (grdData.Rows.Where(p => p.Cells["v_IdCompra"].Value != null && p.Cells["v_IdCompra"].Value.ToString() == Filas[i].Cells["v_IdCompra"].Value.ToString()).Count() != 0)
                {
                    Repetido = false;// modificado por nuevo requerimiento...(se pueden pagar dos veces en la misma cobranza el mismo documento)
                }
                else
                {
                    Repetido = false;
                }

                if (Repetido == false)
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    decimal TCambio = decimal.Parse(txtTipoCambio.Text.Trim());
                    if (TCambio != 0)
                    {
                        switch (cboMoneda.Value.ToString())
                        {
                            case "1":

                                switch (Filas[i].Cells["Moneda"].Value.ToString())
                                {
                                    case "S/.":
                                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                        this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                                        row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                                        row.Cells["i_IdFormaPago"].Value = "-1";
                                        row.Activate();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCompra"].Value = Filas[i].Cells["v_IdCompra"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["TipoDocumento"].Value = Filas[i].Cells["TipoDocumento"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["NroDocumento"].Value = Filas[i].Cells["NroDocumento"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["NombreProveedor"].Value = Filas[i].Cells["NombreProveedor"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_NetoXCobrar"].Value = Filas[i].Cells["d_Saldo"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["Moneda"].Value = "S/.";
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdMoneda"].Value = grdData.Rows[grdData.ActiveRow.Index].Cells["Moneda"].Value.ToString() == "S/." ? 1 : 2;
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsLetra"].Value = (bool)Filas[i].Cells["EsLetra"].Value ? 1 : 0;
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["MonedaOriginal"].Value = Filas[i].Cells["Moneda"].Value.ToString();
                                        break;

                                    case "US$.":
                                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                        this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                                        row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                                        row.Cells["i_IdFormaPago"].Value = "-1";
                                        row.Activate();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCompra"].Value = Filas[i].Cells["v_IdCompra"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["TipoDocumento"].Value = Filas[i].Cells["TipoDocumento"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["NroDocumento"].Value = Filas[i].Cells["NroDocumento"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["NombreProveedor"].Value = Filas[i].Cells["NombreProveedor"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_NetoXCobrar"].Value = Math.Round((decimal.Parse(Filas[i].Cells["d_Saldo"].Value.ToString()) * TCambio), 2, MidpointRounding.AwayFromZero).ToString("0.00");
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["Moneda"].Value = "S/.";
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdMoneda"].Value = grdData.Rows[grdData.ActiveRow.Index].Cells["Moneda"].Value.ToString() == "S/." ? 1 : 2;
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsLetra"].Value = (bool)Filas[i].Cells["EsLetra"].Value ? 1 : 0;
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["MonedaOriginal"].Value = Filas[i].Cells["Moneda"].Value.ToString();
                                        break;
                                }

                                break;

                            case "2":

                                switch (Filas[i].Cells["Moneda"].Value.ToString())
                                {
                                    case "S/.":
                                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                        this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                                        row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                                        row.Cells["i_IdFormaPago"].Value = "-1";
                                        row.Activate();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCompra"].Value = Filas[i].Cells["v_IdCompra"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["TipoDocumento"].Value = Filas[i].Cells["TipoDocumento"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["NroDocumento"].Value = Filas[i].Cells["NroDocumento"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["NombreProveedor"].Value = Filas[i].Cells["NombreProveedor"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_NetoXCobrar"].Value = Math.Round((decimal.Parse(Filas[i].Cells["d_Saldo"].Value.ToString()) / TCambio), 2, MidpointRounding.AwayFromZero).ToString("0.00");
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["Moneda"].Value = "US$.";
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdMoneda"].Value = grdData.Rows[grdData.ActiveRow.Index].Cells["Moneda"].Value.ToString() == "S/." ? 1 : 2;
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsLetra"].Value = (bool)Filas[i].Cells["EsLetra"].Value ? 1 : 0;
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["MonedaOriginal"].Value = Filas[i].Cells["Moneda"].Value.ToString();
                                        break;

                                    case "US$.":
                                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                        this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                                        row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                                        row.Cells["i_IdFormaPago"].Value = "-1";
                                        row.Activate();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCompra"].Value = Filas[i].Cells["v_IdCompra"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["TipoDocumento"].Value = Filas[i].Cells["TipoDocumento"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["NroDocumento"].Value = Filas[i].Cells["NroDocumento"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["NombreProveedor"].Value = Filas[i].Cells["NombreProveedor"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_NetoXCobrar"].Value = Filas[i].Cells["d_Saldo"].Value.ToString();
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["Moneda"].Value = "US$.";
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdMoneda"].Value = grdData.Rows[grdData.ActiveRow.Index].Cells["Moneda"].Value.ToString() == "S/." ? 1 : 2;
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsLetra"].Value = (bool)Filas[i].Cells["EsLetra"].Value ? 1 : 0;
                                        grdData.Rows[grdData.ActiveRow.Index].Cells["MonedaOriginal"].Value = Filas[i].Cells["Moneda"].Value.ToString();
                                        break;
                                }

                                break;

                        }
                    }
                    else
                    {
                        UltraMessageBox.Show("Por favor, ingrese un tipo de cambio válido", "Error de validación");
                    }
                }
            }

            CalcularValoresDetalle();
        }

        private void CalcularValoresDetalle()
        {
            if (grdData.Rows.Count() == 0) return;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
            }
        }

        private int CalcularTotales()
        {
            if (grdData.Rows.Count() > 0)
            {
                decimal SumSoles = 0, SumDolares = 0;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["d_ImporteSoles"].Value == null) { Fila.Cells["d_ImporteSoles"].Value = "0"; }
                    if (Fila.Cells["d_ImporteDolares"].Value == null) { Fila.Cells["d_ImporteDolares"].Value = "0"; }

                    if (Fila.Cells["Moneda"].Value.ToString() == Fila.Cells["MonedaOriginal"].Value.ToString())
                    {
                        //if (decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString()) > decimal.Parse(Fila.Cells["d_NetoXCobrar"].Value.ToString()))
                        //{
                        //    UltraStatusbarManager.MarcarError(ultraStatusBar1, "El Importe no debe ser mayor que el Monto por Pagar", timer1);

                        //    UltraGridCell aCell = grdData.ActiveRow.Cells["d_ImporteSoles"];
                        //    grdData.ActiveRow = grdData.ActiveRow;
                        //    grdData.ActiveCell = aCell;
                        //    grdData.PerformAction(UltraGridAction.EnterEditMode, true, false);

                        //    return -1;
                       // }
                    }

                    SumSoles = SumSoles + decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                    SumDolares = SumDolares + decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                }
                txtTotalSoles.Text = SumSoles.ToString("0.00");
                txtTotalDolares.Text = SumDolares.ToString("0.00");
            }
            return 0;
        }
        private void CalcularValoresFila(UltraGridRow Fila)
        {
            if (Fila.Cells["i_IdMoneda"].Value == null) return;
            if (Fila.Cells["d_NetoXCobrar"].Value == null) { Fila.Cells["d_NetoXCobrar"].Value = "0"; }
            decimal TipoCambio = decimal.Parse(txtTipoCambio.Text.Trim());
            CalcularTotales();
        }

        public void RecibirComprasFacturadas(List<string[]> ListaCompras)
        {
            foreach (string[] Compra in ListaCompras)
            {
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["v_IdCompra"].Value = Compra[0];
                row.Cells["i_IdFormaPago"].Value = "-1";
                row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                string[] CompraDetalle = new string[5];
                CompraDetalle = _objPagoBL.DevolverNombres(Compra[0]);

                if (CompraDetalle != null)
                {
                    row.Cells["TipoDocumento"].Value = CompraDetalle[0];
                    row.Cells["NroDocumento"].Value = CompraDetalle[1];
                    row.Cells["Cliente"].Value = CompraDetalle[2];
                    row.Cells["i_IdMoneda"].Value = CompraDetalle[3];
                    row.Cells["Moneda"].Value = CompraDetalle[4];
                    row.Cells["d_NetoXCobrar"].Value = decimal.Parse(Math.Round(decimal.Parse(Compra[1].ToString()), 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                }
            }
        }
        #endregion

        #region CRUD

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvGuardar.Validate(true, false).IsValid)
            {

                if (txtTipoCambio.Text.Trim() == string.Empty)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;
                }
                else if (decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }

                if (CalcularTotales() == -1)
                {
                    grdData.Focus();
                    return;
                }

                if (cboTipoDocumento.Value.ToString() == "-1")
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por Favor seleccione un tipo de documento", timer1);
                    cboTipoDocumento.PerformAction(UltraComboAction.Dropdown);
                    return;
                }

                if (grdData.Rows.Count() == 0)
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por Favor ingrese almenos una fila al detalle del pago", timer1);
                    return;
                }
                else
                {
                    if (ValidaCamposNulosVacios() == true)
                    {
                        foreach (UltraGridRow Fila in grdData.Rows)
                        {
                            if (Fila.Cells["d_ImporteSoles"].Value == null)
                            {
                                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Todos los Totales no están calculados.", timer1);
                                return;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                if (_Mode == "New")
                {
                    while (_objPagoBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text, int.Parse(cboTipoDocumento.Value.ToString())) == false)
                    {
                        txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                    }
                    _pagoDto.i_IdTipoDocumento = int.Parse(cboTipoDocumento.Value.ToString());
                    _pagoDto.i_IdTipoDocumento = int.Parse(cboTipoDocumento.Value.ToString());
                    _pagoDto.v_Correlativo = txtCorrelativo.Text;
                    _pagoDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                    _pagoDto.v_Nombre = txtNombre.Text;
                    _pagoDto.v_Mes = txtMes.Text;
                    _pagoDto.v_Periodo = txtPeriodo.Text;
                    _pagoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                    _pagoDto.v_Glosa = txtGlosa.Text.Trim();
                    _pagoDto.v_Mes = txtMes.Text.Trim();
                    _pagoDto.v_Periodo = txtPeriodo.Text.Trim();
                    _pagoDto.v_Correlativo = txtCorrelativo.Text;
                    _pagoDto.d_TotalDolares = decimal.Parse(txtTotalDolares.Text.Trim());
                    _pagoDto.d_TotalSoles = decimal.Parse(txtTotalSoles.Text.Trim());
                    _pagoDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                    _pagoDto.i_IdMedioPago = int.Parse(cboMedioPago.Value.ToString());
                    _pagoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                    LlenarTemporalesPago();
                    strIdPago = _objPagoBL.InsertarPago(ref objOperationResult, _pagoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);
                }
                else if (_Mode == "Edit")
                {
                    _pagoDto.i_IdTipoDocumento = int.Parse(cboTipoDocumento.Value.ToString());
                    _pagoDto.v_Correlativo = txtCorrelativo.Text;
                    _pagoDto.v_Nombre = txtNombre.Text;
                    _pagoDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                    _pagoDto.v_Mes = txtMes.Text;
                    _pagoDto.v_Periodo = txtPeriodo.Text;
                    _pagoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                    _pagoDto.v_Glosa = txtGlosa.Text.Trim();
                    _pagoDto.v_Mes = txtMes.Text.Trim();
                    _pagoDto.v_Periodo = txtPeriodo.Text.Trim();
                    _pagoDto.v_Correlativo = txtCorrelativo.Text;
                    _pagoDto.d_TotalDolares = decimal.Parse(txtTotalDolares.Text.Trim());
                    _pagoDto.d_TotalSoles = decimal.Parse(txtTotalSoles.Text.Trim());
                    _pagoDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                    _pagoDto.i_IdMedioPago = int.Parse(cboMedioPago.Value.ToString());
                    _pagoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                    LlenarTemporalesPago();
                    strIdPago = _objPagoBL.ActualizarPago(ref objOperationResult, _pagoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto);
                }

                if (objOperationResult.Success == 1)
                {
                    strModo = "Guardado";
                    //_pstrIdPago = _pagoDto.v_IdPago;
                    // strIdCobranza = _pagoDto.v_IdCobranza;
                    ObtenerListadoPagos(txtPeriodo.Text, txtMes.Text);
                    cboTipoDocumento.Enabled = false;
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UltraStatusbarManager.Mensaje(ultraStatusBar1, "El registro se ha guardado correctamente", timer1);
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _TempDetalle_AgregarDto = new List<pagodetalleDto>();
                _TempDetalle_ModificarDto = new List<pagodetalleDto>();
                _TempDetalle_EliminarDto = new List<pagodetalleDto>();

            }
            else
            {
                UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }

        }

        private void btnEliminarDetalle_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    _pagodetalleDto = new pagodetalleDto();
                    _pagodetalleDto.v_IdPagoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPagoDetalle"].Value.ToString();
                    // _cobranzadetalleDto.v_IdCobranzaDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPagoDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_pagodetalleDto);
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                }
            }
            else
            {
                grdData.Rows[grdData.ActiveRow.Index].Delete(false);
            }
            CalcularValoresDetalle();
        }


        private void LlenarTemporalesPago()
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
                                _pagodetalleDto = new pagodetalleDto();
                                _pagodetalleDto.v_IdPago = _pagoDto.v_IdPago;
                                _pagodetalleDto.d_NetoXCobrar = Fila.Cells["d_NetoXCobrar"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_NetoXCobrar"].Value.ToString());
                                _pagodetalleDto.v_IdCompra = Fila.Cells["v_IdCompra"].Value == null ? null : Fila.Cells["v_IdCompra"].Value.ToString();
                                _pagodetalleDto.d_ImporteDolares = Fila.Cells["d_ImporteDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                _pagodetalleDto.d_ImporteSoles = Fila.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                _pagodetalleDto.i_IdTipoDocumentoRef = Fila.Cells["i_IdTipoDocumentoRef"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumentoRef"].Value.ToString());
                                _pagodetalleDto.i_IdFormaPago = Fila.Cells["i_IdFormaPago"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdFormaPago"].Value.ToString());
                                _pagodetalleDto.v_DocumentoRef = Fila.Cells["v_DocumentoRef"].Value == null ? null : Fila.Cells["v_DocumentoRef"].Value.ToString();
                                _pagodetalleDto.v_Observacion = Fila.Cells["v_Observacion"].Value == null ? null : Fila.Cells["v_Observacion"].Value.ToString();
                                _pagodetalleDto.Moneda = Fila.Cells["Moneda"].Value.ToString();
                                _pagodetalleDto.i_EsLetra = int.Parse(Fila.Cells["i_EsLetra"].Value.ToString());
                                _TempDetalle_AgregarDto.Add(_pagodetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _pagodetalleDto = new pagodetalleDto();
                                _pagodetalleDto.v_IdPago = Fila.Cells["v_IdPago"].Value == null ? null : Fila.Cells["v_IdPago"].Value.ToString();
                                _pagodetalleDto.v_IdPagoDetalle = Fila.Cells["v_IdPagoDetalle"].Value == null ? null : Fila.Cells["v_IdPagoDetalle"].Value.ToString();
                                _pagodetalleDto.v_IdCompra = Fila.Cells["v_IdCompra"].Value == null ? null : Fila.Cells["v_IdCompra"].Value.ToString();
                                _pagodetalleDto.d_NetoXCobrar = Fila.Cells["d_NetoXCobrar"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_NetoXCobrar"].Value.ToString());
                                _pagodetalleDto.d_ImporteDolares = Fila.Cells["d_ImporteDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                _pagodetalleDto.d_ImporteSoles = Fila.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                _pagodetalleDto.i_IdTipoDocumentoRef = Fila.Cells["i_IdTipoDocumentoRef"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumentoRef"].Value.ToString());
                                _pagodetalleDto.v_DocumentoRef = Fila.Cells["v_DocumentoRef"].Value == null ? null : Fila.Cells["v_DocumentoRef"].Value.ToString();
                                _pagodetalleDto.i_IdFormaPago = Fila.Cells["i_IdFormaPago"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdFormaPago"].Value.ToString());
                                _pagodetalleDto.v_Observacion = Fila.Cells["v_Observacion"].Value == null ? null : Fila.Cells["v_Observacion"].Value.ToString();
                                _pagodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _pagodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _pagodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _pagodetalleDto.i_EsLetra = int.Parse(Fila.Cells["i_EsLetra"].Value.ToString());
                                _pagodetalleDto.Moneda = Fila.Cells["Moneda"].Value.ToString();
                                _TempDetalle_ModificarDto.Add(_pagodetalleDto);
                            }
                            break;
                    }
                }
            }

        }



        #endregion

        #region Combos
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMedioPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboWithValue2(ref objOperationResult, 44, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 26, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            cboTipoDocumento.Value = "-1";
            CargarCombosDetalle();
            cboMoneda.SelectedIndex = int.Parse(Globals.ClientSession.i_IdMoneda.Value.ToString());
        }

        private void CargarCombosDetalle()
        {
            OperationResult objOperationResult = new OperationResult();
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
            ucTipoDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucTipoDocumento.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            ucTipoDocumento.DropDownWidth = 330;
            #endregion

            #region Configura Combo Formas de Pago
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            ultraGridColumnaID.Header.Caption = "Cod.";
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaID.Width = 30;
            ultraGridColumnaDescripcion.Width = 200;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaID, ultraGridColumnaDescripcion });
            ucFormasPago.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            ucFormasPago.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            ucFormasPago.DropDownWidth = 330;
            #endregion
            ucFormasPago.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucTipoDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridTesoreria(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucFormasPago, "Value1", "Id", _objPagoBL.ListadoFormasPago(), DropDownListAction.Select);
        }
        #endregion
        #region Grilla
        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdTipoDocumentoRef"].EditorComponent = ucTipoDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumentoRef"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdFormaPago"].EditorComponent = ucFormasPago;
            e.Layout.Bands[0].Columns["i_IdFormaPago"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }
        #endregion

        #region Clases/Validaciones
        private bool ValidaCamposNulosVacios()
        {
            if (grdData.Rows.Where(p => p.Cells["d_ImporteSoles"].Value == null || decimal.Parse(p.Cells["d_ImporteSoles"].Value.ToString().Trim()) <= 0).Count() != 0)
            {
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente el Importe.", timer1);
                UltraGridRow Row = grdData.Rows.Where(p => p.Cells["d_ImporteSoles"].Value == null || decimal.Parse(p.Cells["d_ImporteSoles"].Value.ToString().Trim()) <= 0).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["d_ImporteSoles"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_ImporteSoles"];
                this.grdData.ActiveCell = aCell;
                grdData.PerformAction(UltraGridAction.EnterEditMode);
                return false;
            }

            if (grdData.Rows.Where(p => p.Cells["i_IdFormaPago"].Value != null && p.Cells["i_IdFormaPago"].Value.ToString() == "-1").Count() > 0)
            {
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente las formas de pago.", timer1);
                UltraGridRow Row = grdData.Rows.Where(p => p.Cells["i_IdFormaPago"].Value.ToString() == "-1").FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["i_IdFormaPago"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdFormaPago"];
                this.grdData.ActiveCell = aCell;
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown);
                return false;
            }

            if (grdData.Rows.Where(p => !p.Cells["i_IdFormaPago"].Text.Contains("EFECTIVO") && p.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "-1").Count() > 0)
            {
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese los documentos de referencia.", timer1);
                UltraGridRow Row = grdData.Rows.Where(p => p.Cells["i_IdFormaPago"].Text != "EFECTIVO" && p.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "-1").FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["i_IdTipoDocumentoRef"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"];
                this.grdData.ActiveCell = aCell;
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown);
                return false;
            }

            foreach (string IdCompra in grdData.Rows.AsParallel().ToList().Select(p => p.Cells["v_IdCompra"].Value.ToString()).Distinct().ToList())
            {
                List<UltraGridRow> Filas = new List<UltraGridRow>();
                Filas = grdData.Rows.Where(p => p.Cells["v_IdCompra"].Value.ToString() == IdCompra).ToList();

                if (Filas.Count() > 1)
                {
                    foreach (var item in Filas)
                    {
                        if (Filas.Where(p => p.Cells["i_IdTipoDocumentoRef"].Value.ToString() == item.Cells["i_IdTipoDocumentoRef"].Value.ToString()).Count() > 1)
                        {
                            UltraStatusbarManager.MarcarError(ultraStatusBar1, "No se pueden registrar el mismo documento varias veces con la misma modalidad de pago.", timer1);
                            grdData.Selected.Rows.AddRange(Filas.Where(p => p.Cells["i_IdTipoDocumentoRef"].Value.ToString() == item.Cells["i_IdTipoDocumentoRef"].Value.ToString()).ToArray());
                            grdData.Focus();
                            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                            return false;
                        }
                    }
                }
            }

            //foreach (string IdCompra in grdData.Rows.AsParallel().ToList().Select(p => p.Cells["v_IdCompra"].Value.ToString()).Distinct().ToList())
            //{
            //    if (Globals.ClientSession.i_RedondearVentas == 1 && grdData.Rows.Where(p => p.Cells["v_IdCompra"].Value.ToString() == IdCompra && p.Cells["Moneda"].Value.ToString() == p.Cells["MonedaOriginal"].Value.ToString()).Sum(o => decimal.Parse(o.Cells["d_ImporteSoles"].Value.ToString())) > decimal.Parse(grdData.Rows.Where(q => q.Cells["v_IdCompra"].Value.ToString() == IdCompra).Select(p => p.Cells["d_NetoXCobrar"].Value.ToString()).Distinct().First()))
            //    {
            //        UltraStatusbarManager.MarcarError(ultraStatusBar1, "La suma de los importes superan el monto por cobrar de este documento.", timer1);
            //        List<UltraGridCell> cells = new List<UltraGridCell>();
            //        grdData.Rows.Where(p => p.Cells["v_IdCompra"].Value.ToString() == IdCompra).ToList().ForEach(o => cells.Add(o.Cells["d_ImporteSoles"]));
            //        grdData.Selected.Cells.AddRange(cells.ToArray());
            //        grdData.Focus();
            //        grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);

            //        return false;
            //    }
            //}
            return true;
        }

        /*   private void  CalcularTotalesGrilla()
           {
               if (grdData.Rows.Count() > 0)
               {
                   decimal SumSoles = 0, SumDolares = 0;

                   foreach (UltraGridRow Fila in grdData.Rows)
                   {
                       if (Fila.Cells["d_ImporteSoles"].Value == null) { Fila.Cells["d_ImporteSoles"].Value = "0"; }
                       if (Fila.Cells["d_ImporteDolares"].Value == null) { Fila.Cells["d_ImporteDolares"].Value = "0"; }
                       //if (Globals.ClientSession.i_RedondearVentas == 1 && Fila.Cells["Moneda"].Value.ToString() == Fila.Cells["MonedaOriginal"].Value.ToString())
                       //{
                       //    if (decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString()) > decimal.Parse(Fila.Cells["d_NetoXCobrar"].Value.ToString()))
                       //    {
                       //        UltraStatusbarManager.MarcarError(ultraStatusBar1, "El Importe no debe ser mayor que el Monto por Pagar", timer1);

                       //        UltraGridCell aCell = grdData.ActiveRow.Cells["d_ImporteSoles"];
                       //        grdData.ActiveRow = grdData.ActiveRow;
                       //        grdData.ActiveCell = aCell;
                       //        grdData.PerformAction(UltraGridAction.EnterEditMode, true, false);

                       //        return -1;
                       //    }
                       //}
                       SumSoles = SumSoles + decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                       SumDolares = SumDolares + decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                   }
                   txtTotalSoles.Text = SumSoles.ToString("0.00");
                   txtTotalDolares.Text = SumDolares.ToString("0.00");
               }
              // return 0;
           }
         * */

        #endregion

        #region Grilla
        private void grdData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "i_IdFormaPago")
            {
                var xx = (List<GridKeyValueDTO>)ucFormasPago.DataSource;

                if (xx != null)
                {
                    var doc = xx.Where(p => p.Id == e.Cell.Value.ToString()).FirstOrDefault();

                    if (doc == null) return;
                    grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value = doc.Value3 != null && !string.IsNullOrEmpty(doc.Value3.ToString()) ? int.Parse(doc.Value3.ToString()) : -1;
                }
            }
        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            string IdTipoDocRef = grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value.ToString();

            if (IdTipoDocRef != "-1")
            {
                if (grdData.ActiveCell.Column.Key == "v_DocumentoRef")
                {
                    if (grdData.ActiveCell.Text.Contains("-"))
                    {
                        decimal Importe = 0;

                        switch (IdTipoDocRef.Trim())
                        {
                            case "7":
                               int i;
                                var SerieCorrelativo = grdData.ActiveCell.Text.Split('-');
                                var serie = int.TryParse(SerieCorrelativo[0], out i) ? i.ToString("0000") : SerieCorrelativo[0];
                                var correlativo = int.TryParse(SerieCorrelativo[1], out i) ? i.ToString("00000000"): "No formated";
                                grdData.ActiveCell.Value = string.Format("{0}-{1}", serie.ToUpper(), correlativo.ToUpper());
                                Importe = _objPagoBL.DevuelveMontoNotaCredito(grdData.ActiveCell.Text.Trim());
                                if (Importe == 0)
                                {
                                    UltraMessageBox.Show("El documento no existe o ya fue usado en un pago", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                                    grdData.ActiveRow.Cells["v_DocumentoRef"].Value = "";
                                    return;
                                }
                                grdData.ActiveRow.Cells["d_ImporteSoles"].Value = Importe.ToString();
                                break;
                        }
                    }
                    return;
                }
            }
            CalcularValoresDetalle();
        }
        private void grdData_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (e.Cell.Column.Key == "d_ImporteSoles")
            {
                if (grdData.ActiveRow != null && grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "7")
                {
                  //  e.Cell.CancelUpdate();
                }
            }
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {

            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEliminarDetalle.Enabled = false;
            }
            else
            {
                btnEliminarDetalle.Enabled = true;
            }
        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdData.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grdData.ActiveCell.Column.Key)
                {
                    case "d_NetoXCobrar":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;
                }
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            if (grdData.ActiveCell.Column.Key == "d_ImporteSoles")
            {
                if (grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "7")
                {
                 //   e.SuppressKeyPress = true;
                }
            }

            if (this.grdData.ActiveCell.Column.Key != "i_IdTipoDocumentoRef")
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


        #endregion

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Pagos, _pagoDto.t_FechaRegistro, dtpFechaRegistro.Value, _pagoDto.v_Correlativo, int.Parse(cboTipoDocumento.Value.ToString()));
            #region Controla Cierre de Mes
            if (new CierreMensualBL().VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.CtasPagar))
            {
                btnGuardar.Visible = false;
                this.Text = "Pago [MES CERRADO]";
            }
            else
            {

                btnGuardar.Visible = true;
                this.Text = "Pago";
            }
            #endregion
        }

        private void dtpFechaRegistro_Leave(object sender, EventArgs e)
        {
            if (_Mode == "New")
            {
                OperationResult objOperationResult = new OperationResult();
                txtTipoCambio.Text = _objPagoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dtpFechaRegistro_Validated(object sender, EventArgs e)
        {
            dtpFechaRegistro_ValueChanged(sender, e);
        }

        private void btnEliminarPago_Click(object sender, EventArgs e)
        {
            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar este pago  de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {

                    string pstrIdPago = _pagoDto.v_IdPago;
                    _objPagoBL.EliminarPago(ref _objOperationResult, pstrIdPago, Globals.ClientSession.GetAsList());
                    if (_objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show(_objOperationResult.ErrorMessage + "\n\n" + _objOperationResult.ExceptionMessage + "\n\nTARGET: " + _objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Close();
                }
            }
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {

        }

        private void txtGlosa_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.CaracteresValidos((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }

        private void grdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            if (e.Cell.Value == null || grdData.ActiveCell == null) return;
            if (grdData.ActiveCell.Column.Key != "d_ImporteSoles") return;

            if (decimal.Parse(e.NewValue.ToString()) > decimal.Parse(grdData.ActiveRow.Cells["d_NetoXCobrar"].Value.ToString()))
            {
                //UltraMessageBox.Show("No se puede actualizar el importe a un valor superior.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //e.Cancel = true;
            }
        }


    }
}
