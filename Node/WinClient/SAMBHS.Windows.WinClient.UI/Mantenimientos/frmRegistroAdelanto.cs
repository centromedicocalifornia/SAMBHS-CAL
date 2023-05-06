using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Security.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmRegistroAdelanto : Form
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        Cobranza.BL.AdelantoBL _objAdelantoBL = new Cobranza.BL.AdelantoBL();
        SecurityBL _objSecurityBL = new SecurityBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<KeyValueDTO> _ListadoAdelantos = new List<KeyValueDTO>();
        adelantoDto _adelantoDto = new adelantoDto();
        public string _pstrIdAdelanto_Nuevo;
        string strModo, _Mode, strIdAdelanto;
        int _MaxV = 0, _ActV = 0;
        public frmRegistroAdelanto(string Modo, string Id)
        {
            strModo = Modo;
            strIdAdelanto = Id;
            InitializeComponent();
        }

        #region Permisos Botones
        bool _btnGuardar = false;
        bool _btnImprimir = false;
        #endregion

        private void frmRegistroAdelanto_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            #region ControlAcciones
            var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmRegistroAdelanto", Globals.ClientSession.i_RoleId);

            _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroAdelanto_SAVE", _formActions);
            _btnImprimir = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroAdelanto_PRINT", _formActions);
            btnImprimir.Enabled = _btnImprimir;
            #endregion

            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;

            txtPeriodo.Text = DateTime.Now.Year.ToString();
            txtMes.Text = DateTime.Now.Month.ToString("00");
            var listadoComboDocumentos = new DocumentoBL().ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", listadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            cboTipoDocumento.Value = "-1";
            cboMoneda.SelectedValue = Globals.ClientSession.i_IdMoneda.ToString();
            ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);

            if (_objAdelantoBL.DevuelveEstadoBotonGuardar(_adelantoDto.v_SerieDocumento, _adelantoDto.v_CorrelativoDocumento))
            {
                btnGuardar.Enabled = _btnGuardar;
            }
            else
            {
                btnGuardar.Enabled = false;
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            #region Validaciones Generales
            txtImporte.Text = txtImporte.Text == string.Empty ? "0" : txtImporte.Text;
            txtTipoCambio.Text = txtTipoCambio.Text == string.Empty ? "0" : txtTipoCambio.Text;
            decimal d;
            var tc = decimal.TryParse(txtTipoCambio.Text, out d) ? d : 0m;
            if (tc <= 0) {
                txtTipoCambio.Focus();
                return;
            }

            //if (cboTipoDocumento.Value == null || cboTipoDocumento.Value.ToString().Equals("-1"))
            //{
            //    MessageBox.Show(@"Por favor ingrese correctamente el documento de caja", @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    cboTipoDocumento.Focus();
            //    return;
            //}

            #endregion

            if (uvDatos.Validate(true, false).IsValid)
            {
                if (decimal.Parse(txtImporte.Text.Trim()) <= 0)
                {
                    txtImporte.Focus();
                    txtImporte.SelectAll();
                    return;
                }

                if (_Mode == "New")
                {
                    while (_objAdelantoBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                    {
                        txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                    }

                    while (_objAdelantoBL.ExisteDocumento(txtSerieDoc.Text, txtCorrelativoDocIni.Text) == false)
                    {
                        txtCorrelativoDocIni.Text = (int.Parse(txtCorrelativoDocIni.Text) + 1).ToString("00000000");
                    }

                    #region Guarda Entidad Adelanto
                    _adelantoDto.d_Importe = txtImporte.Text == string.Empty ? 0 : decimal.Parse(txtImporte.Text.Trim());
                    _adelantoDto.d_TipoCambio = decimal.Parse(txtTipoCambio.Text.Trim());
                    _adelantoDto.i_IdMoneda = int.Parse(cboMoneda.SelectedValue.ToString());
                    _adelantoDto.i_IdTipoDocumento = 433;
                    _adelantoDto.v_Correlativo = txtCorrelativo.Text;
                    _adelantoDto.v_Mes = txtMes.Text;
                    _adelantoDto.v_SerieDocumento = txtSerieDoc.Text;
                    _adelantoDto.v_CorrelativoDocumento = txtCorrelativoDocIni.Text;
                    _adelantoDto.v_Glosa = txtGlosa.Text;
                    _adelantoDto.v_Periodo = txtPeriodo.Text;
                    _adelantoDto.t_FechaAdelanto = dptFecha.Value;
                    _pstrIdAdelanto_Nuevo = _objAdelantoBL.InsertarAdelanto(ref objOperationResult, _adelantoDto, Globals.ClientSession.GetAsList());
                    #endregion
                }
                else if (_Mode == "Edit")
                {
                    if (_adelantoDto.d_Consumo != null && _adelantoDto.d_Consumo.Value > 0)
                    {
                        UltraMessageBox.Show("Este Adelanto no puede ser modificado porque ya ha sido usado en cobranza", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    #region Actualiza Entidad Venta
                    _adelantoDto.d_Importe = txtImporte.Text == string.Empty ? 0 : decimal.Parse(txtImporte.Text.Trim());
                    _adelantoDto.d_TipoCambio = decimal.Parse(txtTipoCambio.Text.Trim());
                    _adelantoDto.i_IdMoneda = int.Parse(cboMoneda.SelectedValue.ToString());
                    _adelantoDto.i_IdTipoDocumento = 433;
                    _adelantoDto.v_Correlativo = txtCorrelativo.Text;
                    _adelantoDto.v_Mes = txtMes.Text;
                    _adelantoDto.v_SerieDocumento = txtSerieDoc.Text;
                    _adelantoDto.v_CorrelativoDocumento = txtCorrelativoDocIni.Text;
                    _adelantoDto.v_Glosa = txtGlosa.Text;
                    _adelantoDto.v_Periodo = txtPeriodo.Text;
                    _adelantoDto.t_FechaAdelanto = dptFecha.Value;
                    _objAdelantoBL.ActualizarAdelanto(ref objOperationResult, _adelantoDto, Globals.ClientSession.GetAsList());
                    #endregion
                }

                if (objOperationResult.Success == 1)
                {
                    strModo = "Guardado";
                    btnImprimir.Enabled = _btnImprimir == true ? true : false;
                    ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                    _Mode = "Edit";
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ObtenerListadoVentas(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoAdelantos = _objAdelantoBL.ObtenerListadoAdelantos(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdAdelanto);
                    break;

                case "Nuevo":
                    if (_ListadoAdelantos.Count != 0)
                    {
                        _MaxV = _ListadoAdelantos.Count() - 1;
                        _ActV = _MaxV;
                        LimpiarCabecera();
                        txtCorrelativo.Text = (int.Parse(_ListadoAdelantos[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        _adelantoDto = new adelantoDto();
                        EdicionBarraNavegacion(false);
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        _MaxV = 1;
                        _ActV = 1;
                        _adelantoDto = new adelantoDto();
                        EdicionBarraNavegacion(false);
                    }

                    txtTipoCambio.Text = _objAdelantoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dptFecha.Value.Date);
                    txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, 433).Trim();
                    txtCorrelativoDocIni.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, 433).ToString("00000000");
                    break;

                case "Guardado":
                    _MaxV = _ListadoAdelantos.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdAdelanto == "" | strIdAdelanto == null)
                    {
                        CargarCabecera(_ListadoAdelantos[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdAdelanto);
                    }
                    break;

                case "Consulta":
                    if (_ListadoAdelantos.Count != 0)
                    {
                        _MaxV = _ListadoAdelantos.Count() - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoAdelantos[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoAdelantos[_MaxV].Value2);
                        _Mode = "Edit";
                        EdicionBarraNavegacion(true);
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        _MaxV = 1;
                        _ActV = 1;
                        _adelantoDto = new adelantoDto();
                        txtTipoCambio.Text = _objAdelantoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dptFecha.Value.Date);
                        EdicionBarraNavegacion(false);
                        txtMes.Enabled = true;
                    }
                    break;
            }
        }

        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            txtCorrelativo.Enabled = ON_OFF;
            txtMes.Enabled = ON_OFF;
        }

        private void CargarCabecera(string idAdelanto)
        {
            _adelantoDto = new adelantoDto();
            _adelantoDto = _objAdelantoBL.DevuelveAdelanto(idAdelanto);

            if (_adelantoDto != null)
            {
                cboMoneda.SelectedValue = _adelantoDto.i_IdMoneda.ToString();
                txtPeriodo.Text = _adelantoDto.v_Periodo;
                txtTipoCambio.Text = _adelantoDto.d_TipoCambio.ToString();
                dptFecha.Value = _adelantoDto.t_FechaAdelanto?? DateTime.Now;
                txtMes.Text = int.Parse(_adelantoDto.v_Mes).ToString("00");
                txtImporte.Text = _adelantoDto.d_Importe.Value.ToString("0.00");
                txtCliente.Text = _adelantoDto.NombreCliente;
                txtPeriodo.Text = _adelantoDto.v_Periodo;
                txtGlosa.Text = _adelantoDto.v_Glosa;
                txtSerieDoc.Text = _adelantoDto.v_SerieDocumento;
                txtCorrelativo.Text = _adelantoDto.v_Correlativo;
                txtCorrelativoDocIni.Text = _adelantoDto.v_CorrelativoDocumento;
                txtSaldo.Text = _adelantoDto.d_Saldo.Value.ToString("0.00");
                cboTipoDocumento.Value = _adelantoDto.i_IdDocumentoCaja.Value;
                _Mode = "Edit";
                btnImprimir.Enabled = _btnImprimir;
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la compra", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void LimpiarCabecera()
        {

        }

        private void btnBuscarDetraccion_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", "");
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                _adelantoDto.v_IdCliente = frm._IdCliente;
                txtCliente.Text = frm._RazonSocial;
            }
        }

        private void txtImporte_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtImporte, e);
        }

        private void txtTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtImporte, e);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            Reportes.Cobranza.frmDocumentoVoucherAdelantos frm = new Reportes.Cobranza.frmDocumentoVoucherAdelantos(_adelantoDto.v_IdAdelanto);
            frm.ShowDialog();
        }

        private void dptFecha_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = dptFecha.Value.Year.ToString();
            txtMes.Text = dptFecha.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Adelanto, _adelantoDto.t_FechaAdelanto, dptFecha.Value, _adelantoDto.v_Correlativo, 0);
        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = ((List<GridKeyValueDTO>)cboTipoDocumento.DataSource).Find(p => p.Id == (string)cboTipoDocumento.Value || p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
        }
    }
}
