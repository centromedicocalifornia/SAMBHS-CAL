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
using Infragistics.Documents.Excel;
using System.IO;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinMaskedEdit;

namespace SAMBHS.Windows.WinClient.UI.Migraciones
{
    public partial class frmCargaInicial : Form
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        movimientoDto _movimientoDto = new movimientoDto();
        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
        List<KeyValueDTO> _ListadoMovimientos = new List<KeyValueDTO>();
        UltraCombo ucUnidadMedida = new UltraCombo();
        UltraCombo ucTipoDocumento = new UltraCombo();
        int _MaxV, _ActV;
        string _Mode;
        public string _pstrIdMovimiento_Nuevo;
        string strModo = "Nuevo", strIdmovimiento;
        string FormatoCantidad;

        #region Temporales DetalleVenta
        List<movimientodetalleDto> _TempDetalle_AgregarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempDetalle_ModificarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempDetalle_EliminarDto = new List<movimientodetalleDto>();
        #endregion

        public frmCargaInicial(string Modo)
        {
            InitializeComponent();
        }

        private void frmNotaIngreso_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = DateTime.Now.Year.ToString();
            txtMes.Text = DateTime.Now.Month.ToString();
            
            #region Cargar Combos
            Utils.Windows.LoadDropDownList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboMotivo, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 19, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            CargarCombosDetalle();
            #endregion

            FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales, (int)Globals.ClientSession.i_PrecioDecimales);
            ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);

            if (_movimientoDto.v_OrigenTipo == "C" | _movimientoDto.v_OrigenTipo == "I" | _movimientoDto.v_OrigenTipo == "G")
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
            int DecimalesCantidad = (int)Globals.ClientSession.i_CantidadDecimales;

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

            cboColumnaPrecio.SelectedIndex = 0;
            cboColumnaNombre.SelectedIndex = 0;
            cboColumnaCodigo.SelectedIndex = 0;
            cboColumnaCantidad.SelectedIndex = 0;
            cboColumnaPedido.SelectedIndex = 0;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidad"].Value = "-1";
                    row.Cells["i_IdTipoDocumento"].Value = "-1";
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["d_Cantidad"].Value = "0";
                    row.Cells["d_Total"].Value = "0";
                }
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
                row.Cells["d_Precio"].Value = "0";
                row.Cells["d_Cantidad"].Value = "0";
                row.Cells["d_Total"].Value = "0";
            }
            UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodigoInterno"];
            this.grdData.ActiveCell = aCell;
            grdData.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (uvDatos.Validate(true, false).IsValid)
            {
                if (txtTipoCambio.Value == null || txtTipoCambio.Value.ToString() == "")
                {
                    UltraMessageBox.Show("Por Favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtTipoCambio.Focus();
                    return;
                }

                if (grdData.Rows.Count() == 0)
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        if (Fila.Cells["v_IdProductoDetalle"].Value == null)
                        {
                            UltraMessageBox.Show("Por favor ingrese correctamente todos los productos al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                CalcularTotal();
                if (_Mode == "New")
                {
                    while (_objMovimientoBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text, (int)TipoDeMovimiento.NotadeIngreso) == false)
                    {
                        txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                    }
                    decimal cantidad, precio;
                    _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                    _movimientoDto.i_IdAlmacenOrigen = int.Parse(cboAlmacen.SelectedValue.ToString());
                    _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.SelectedValue.ToString());
                    _movimientoDto.i_IdTipoMotivo = int.Parse(cboMotivo.SelectedValue.ToString());
                    _movimientoDto.t_Fecha = dtpFecha.Value;
                    _movimientoDto.v_Glosa = txtGlosa.Text.Trim();
                    _movimientoDto.v_Mes = txtMes.Text.Trim();
                    _movimientoDto.v_Periodo = txtPeriodo.Text.Trim();
                    _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                    _movimientoDto.v_Correlativo = txtCorrelativo.Text;
                    _movimientoDto.d_TotalCantidad = txtCantidad.Text == "" ? 0 : decimal.TryParse(txtCantidad.Text, out cantidad) ? cantidad : 0;
                    _movimientoDto.d_TotalPrecio = txtTotal.Text == "" ? 0 : decimal.TryParse(txtTotal.Text, out precio) ? precio : 0;
                    _movimientoDto.i_EsDevolucion = chkDevolucion.Checked == true ? 1 : 0;
                    _movimientoDto.i_IdEstablecimiento = int.Parse(Globals.ClientSession.i_IdEstablecimiento.Value.ToString());
                    LlenarTemporales();
                    _objMovimientoBL.InsertarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);
                }
                else if (_Mode == "Edit")
                {
                    _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                    _movimientoDto.i_IdAlmacenOrigen = int.Parse(cboAlmacen.SelectedValue.ToString());
                    _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.SelectedValue.ToString());
                    _movimientoDto.i_IdTipoMotivo = int.Parse(cboMotivo.SelectedValue.ToString());
                    _movimientoDto.t_Fecha = dtpFecha.Value;
                    _movimientoDto.v_Glosa = txtGlosa.Text.Trim();
                    _movimientoDto.v_Mes = txtMes.Text.Trim();
                    _movimientoDto.v_Periodo = txtPeriodo.Text.Trim();
                    _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                    _movimientoDto.v_Correlativo = txtCorrelativo.Text;
                    _movimientoDto.d_TotalCantidad = txtCantidad.Text == "" ? 0 : decimal.Parse(txtCantidad.Text);
                    _movimientoDto.d_TotalPrecio = txtTotal.Text == "" ? 0 : decimal.Parse(txtTotal.Text);
                    _movimientoDto.i_EsDevolucion = chkDevolucion.Checked == true ? 1 : 0;
                    _movimientoDto.i_IdEstablecimiento = int.Parse(Globals.ClientSession.i_IdEstablecimiento.Value.ToString());
                    LlenarTemporales();
                    _objMovimientoBL.ActualizarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto);
                }
                if (objOperationResult.Success == 1)
                {
                    strModo = "Guardado";
                    EdicionBarraNavegacion(true);
                    _pstrIdMovimiento_Nuevo = _movimientoDto.v_IdMovimiento;
                    strIdmovimiento = _movimientoDto.v_IdMovimiento;
                    ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _movimientodetalleDto = new movimientodetalleDto();
                    _movimientodetalleDto.v_IdMovimientoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_movimientodetalleDto);
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                }
            }
            else
            {
                grdData.Rows[grdData.ActiveRow.Index].Delete(false);
            }
        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtProveedor.Text = frm._RazonSocial;
                _movimientoDto.v_IdCliente = frm._IdProveedor;
            }
        }

        private void txtProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtProveedor.Text == "") return;
                Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtProveedor.Text, "Nombre");
                frm.ShowDialog();
                if (frm._IdProveedor != null)
                {
                    txtProveedor.Text = frm._RazonSocial;
                    _movimientoDto.v_IdCliente = frm._IdProveedor;
                }
            }
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string TipoCambio = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
            txtTipoCambio.Text = TipoCambio;
            txtPeriodo.Text = dtpFecha.Value.Year.ToString();
            txtMes.Text = dtpFecha.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.NotaIngreso, _movimientoDto.t_Fecha, dtpFecha.Value, _movimientoDto.v_Correlativo, 0);

        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }

        private void frmNotaIngreso_FormClosing(object sender, FormClosingEventArgs e)
        {

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
            if (grdData.ActiveCell.Column.Key == "v_NroGuiaRemision")
            {
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroGuiaRemision"].Value != null)
                {
                    string NroGuia;
                    NroGuia = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroGuiaRemision"].Value.ToString();
                    if (NroGuia.Contains("-"))
                    {
                        string[] SerieCorrelativo = new string[2];
                        SerieCorrelativo = NroGuia.Split(new Char[] { '-' });
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroGuiaRemision"].Value = int.Parse(SerieCorrelativo[0]).ToString("0000") + "-" + int.Parse(SerieCorrelativo[1]).ToString("00000000");
                    }
                }
            }
            else if (grdData.ActiveCell.Column.Key == "v_NumeroDocumento")
            {
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_NumeroDocumento"].Value != null)
                {
                    string NroGuia;
                    NroGuia = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NumeroDocumento"].Value.ToString();
                    if (NroGuia.Contains("-"))
                    {
                        string[] SerieCorrelativo = new string[2];
                        SerieCorrelativo = NroGuia.Split(new Char[] { '-' });
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NumeroDocumento"].Value = int.Parse(SerieCorrelativo[0]).ToString("0000") + "-" + int.Parse(SerieCorrelativo[1]).ToString("00000000");
                    }
                }
            }
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
                    break;

                case "Nuevo":
                    if (_ListadoMovimientos.Count != 0)
                    {
                        _MaxV = _ListadoMovimientos.Count() - 1;
                        _ActV = _MaxV;
                        LimpiarCabecera();
                        CargarDetalle("");
                        txtCorrelativo.Text = (int.Parse(_ListadoMovimientos[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        _movimientoDto = new movimientoDto();
                        EdicionBarraNavegacion(false);
                        cboAlmacen.Enabled = true;
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
                        EdicionBarraNavegacion(false);
                    }
                    //txtMes.Enabled = true;
                    txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
                    cboAlmacen.SelectedValue = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    break;

                case "Guardado":
                    _MaxV = _ListadoMovimientos.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdmovimiento == "" | strIdmovimiento == null)
                    {
                        CargarCabecera(_ListadoMovimientos[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdmovimiento);
                    }
                    btnNuevoMovimiento.Enabled = true;
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
            OperationResult objOperationResult = new OperationResult();
            LimpiarCabecera();
            CargarDetalle("");
            txtCorrelativo.Text = (int.Parse(_ListadoMovimientos[_MaxV].Value1) + 1).ToString("00000000");
            _Mode = "New";
            _movimientoDto = new movimientoDto();
            EdicionBarraNavegacion(false);
            txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
            cboAlmacen.Enabled = true;
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
            cboAlmacen.SelectedValue = "-1";
            cboMoneda.SelectedValue = "-1";
            cboMotivo.SelectedValue = "-1";
            dtpFecha.Value = DateTime.Today;
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
            if (_movimientoDto != null)
            {
                _Mode = "Edit";
                cboAlmacen.SelectedValue = _movimientoDto.i_IdAlmacenOrigen.ToString();
                cboMoneda.SelectedValue = _movimientoDto.i_IdMoneda.ToString();
                cboMotivo.SelectedValue = _movimientoDto.i_IdTipoMotivo.ToString();
                txtGlosa.Text = _movimientoDto.v_Glosa;
                dtpFecha.Value = _movimientoDto.t_Fecha.Value;
                txtTipoCambio.Text = _movimientoDto.d_TipoCambio.ToString();
                txtProveedor.Text = _movimientoDto.v_NombreCliente;
                txtCorrelativo.Text = _movimientoDto.v_Correlativo;
                txtPeriodo.Text = _movimientoDto.v_Periodo;
                txtMes.Text = _movimientoDto.v_Mes;
                txtTotal.Text = _movimientoDto.d_TotalPrecio.Value.ToString("0.00");
                txtCantidad.Text = _movimientoDto.d_TotalCantidad.Value.ToString(FormatoCantidad);
                chkDevolucion.Checked = _movimientoDto.i_EsDevolucion == 1 ? true : false;
                CargarDetalle(_movimientoDto.v_IdMovimiento);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar el movimiento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void CargarDetalle(string pstringIdMovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objMovimientoBL.ObtenerMovimientoDetalles(ref objOperationResult, pstringIdMovimiento);
            if (grdData.Rows.Count > 0)
            {
                BuscarNombresArticulos();
                for (int i = 0; i < grdData.Rows.Count(); i++)
                {
                    grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
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

            Utils.Windows.LoadUltraComboList(ucTipoDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 1, 0), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucUnidadMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select);
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
                                _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidad"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidad"].Value.ToString());
                                _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _movimientodetalleDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Total"].Value.ToString());
                                _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                                _movimientodetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
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
                                _movimientodetalleDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Total"].Value.ToString());
                                _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                                _movimientodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _movimientodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _movimientodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _movimientodetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _TempDetalle_ModificarDto.Add(_movimientodetalleDto);
                            }
                            break;
                    }
                }
            }

        }

        private void BuscarNombresArticulos()
        {
            List<string[]> ListaCadena = new List<string[]>();
            ListaCadena = _objMovimientoBL.DevolverNombres(grdData.Rows[0].Cells["v_IdMovimiento"].Value.ToString(), int.Parse(cboAlmacen.SelectedValue.ToString()));

            for (int i = 0; i < ListaCadena.Count; i++)
            {
                grdData.Rows[i].Cells["v_CodigoInterno"].Value = ((string[])ListaCadena[i])[0];
                grdData.Rows[i].Cells["v_NombreProducto"].Value = ((string[])ListaCadena[i])[1];
            }
        }

        private void CalcularTotal()
        {
            decimal TotalPrecio = 0, TotalCantidad = 0;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if (Fila.Cells["d_Precio"].Value == null | Fila.Cells["d_Cantidad"].Value == null) return;
                Fila.Cells["d_Total"].Value = (decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString())).ToString();
                TotalPrecio = TotalPrecio + decimal.Parse(Fila.Cells["d_Total"].Value.ToString());
                TotalCantidad = TotalCantidad + decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());

                if (Fila.Cells["i_IdUnidad"].Value != null)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_IdUnidad"].Value.ToString() != "-1" && decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0)
                    {
                        decimal TotalEmpaque = 0;
                        decimal Empaque = decimal.Parse(Fila.Cells["Empaque"].Value.ToString());
                        string Producto = Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                        decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                        int UM = int.Parse(Fila.Cells["i_IdUnidad"].Value.ToString());
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

            txtCantidad.Text = TotalCantidad.ToString(FormatoCantidad);
            txtTotal.Text = TotalPrecio.ToString("0.00");
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
                FormatoCantidad = "nnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "nnn";
            }

            if (DecimalesPrecio > 0)
            {
                string sharp = "n";
                FormatoPrecio = "nnnn.";
                for (int i = 0; i < DecimalesPrecio; i++)
                {
                    FormatoPrecio = FormatoPrecio + sharp;
                }
            }
            else
            {
                FormatoPrecio = "nnnn";
            }

            _Cantidad.MaskInput = FormatoCantidad;
            _Precio.MaskInput = "-" + FormatoPrecio;
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
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidad"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = "1.0000";
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
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidad"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = "1.0000";
                    }
                }
            }
            CalcularTotal();
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
            if (int.Parse(cboAlmacen.SelectedValue.ToString()) == -1)
            {
                UltraMessageBox.Show("Porfavor seleccione un almacén antes de buscar un producto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (e.Cell.Column.Key == "v_CodigoInterno")
            {
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
                {
                    Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(int.Parse(cboAlmacen.SelectedValue.ToString()), null, null, grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Text == null ? string.Empty : grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Text.ToString());
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
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                    }
                }
            }
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void grdData_AfterCellActivate(object sender, EventArgs e)
        {
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value == null | grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value == null) return;
            if (decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString()) == 0 | decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value.ToString()) == 0) return;
            decimal cantidad;
            decimal precio, total;
            cantidad = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
            precio = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value.ToString());
            total = cantidad * precio;
            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Total"].Value = total;
            CalcularTotal();
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (_movimientoDto.v_OrigenTipo == null)
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
        }

        private void grdData_BeforeExitEditMode(object sender, BeforeExitEditModeEventArgs e)
        {
            if (e.CancellingEditOperation)
                return;
            if (this.grdData.ActiveCell.Column.Key == "d_Cantidad" | this.grdData.ActiveCell.Column.Key == "d_Precio")
            {
                //decimal Cantidad = grdData.ActiveCell.Text == "" ? 0 : decimal.Parse(grdData.ActiveCell.Text);

                if (grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value == null | grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value == null) return;
                if (decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString()) == 0 | decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value.ToString()) == 0) return;
                decimal cantidad;
                decimal precio, total;
                cantidad = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
                precio = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value.ToString());
                total = cantidad * precio;
                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Total"].Value = total;
                CalcularTotal();
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

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }
        #endregion

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count() > 0)
            {
                grdData.Selected.Rows.AddRange((UltraGridRow[])grdData.Rows.All);
                grdData.DeleteSelectedRows(false);
            }

            try
            {
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = "Archivos Excel (*.*)| *.*";
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = false;
                string sFileName;
                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    sFileName = choofdlog.FileName;
                }
                else
                {
                    return;
                }

                Workbook workbook = Workbook.Load(sFileName);
                Worksheet worksheet = workbook.Worksheets[0];

                this.ultraDataSource1.Reset();

                bool isHeaderRow = true;
                foreach (WorksheetRow row in worksheet.Rows)
                {
                    // Assume that the first row is the column headers. 
                    if (isHeaderRow)
                    {
                        foreach (WorksheetCell cell in row.Cells)
                        {
                            // Get the text of the cell. 
                            string columnKey = cell.GetText();

                            // Adda column
                            UltraDataColumn column = this.ultraDataSource1.Band.Columns.Add(columnKey);

                            // Set the DataType. 
                            switch (columnKey)
                            {
                                case "Codigo":
                                    column.DataType = typeof(string);
                                    break;
                                case "Descripcion":
                                    column.DataType = typeof(string);
                                    break;
                                default:
                                    column.DataType = typeof(string);
                                    break;
                            }
                        }

                        // All following rows will not be headers. 
                        isHeaderRow = false;
                    }
                    else
                    {
                        // Get the data fom the excel row cells. 
                        List<object> rowData = new List<object>();
                        foreach (WorksheetCell cell in row.Cells)
                        {
                            rowData.Add(cell.Value);
                        }
                        try
                        {
                            this.ultraDataSource1.Rows.Add(rowData.ToArray());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            continue;
                        }
                        // Add a row to the UltraDataSource

                    }

                    button2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            // Bind the UltraDataSource to the grid. 
            this.ultraGrid1.SetDataBinding(this.ultraDataSource1, "Band 0");

            foreach (UltraGridCell cell in ultraGrid1.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboColumnaCantidad.Items.Add(ColumnKey);
                    cboColumnaCodigo.Items.Add(ColumnKey);
                    cboColumnaNombre.Items.Add(ColumnKey);
                    cboColumnaPrecio.Items.Add(ColumnKey);
                    cboColumnaPedido.Items.Add(ColumnKey);
                }
            }


        }

        private void grdData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "d_Cantidad" | e.Cell.Column.Key == "d_Precio")
            {
                if (grdData.ActiveRow != null && e.Cell.Value != null)
                {
                    UltraGridRow Fila = grdData.ActiveRow;
                    if (Fila.Cells["d_Precio"].Value == null | Fila.Cells["d_Cantidad"].Value == null) return;
                    Fila.Cells["d_Total"].Value = (decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString())).ToString();
                }
            }

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            #region Migracion
            string ColumaCodigo = cboColumnaCodigo.Text;
            string txtColumnaCantidad = cboColumnaCantidad.Text;
            string txtColumnaNombre = cboColumnaNombre.Text;
            string txtColumnaPrecio = cboColumnaPrecio.Text;
            string txtColumnaPedido = cboColumnaPedido.Text;

            var Filas = ultraGrid1.Rows.Where(p => p.Cells[ColumaCodigo].Value.ToString() != string.Empty && p.Cells[txtColumnaNombre].Value.ToString() != string.Empty).ToList();

            foreach (UltraGridRow Fila in Filas)
            {
                string Codigo = Fila.Cells[ColumaCodigo].Value.ToString().Trim();
                var CodigoYUM = _objMovimientoBL._DevolverIdProductoDetalle(Codigo);
                string IdProducto = CodigoYUM[0];
                string IdUM = CodigoYUM[1];
                decimal Empaque = decimal.TryParse(CodigoYUM[2], out Empaque) ? Empaque : 0;
                if (IdProducto != null)
                {
                    decimal cantidad, precio;
                    decimal StockTotal = txtColumnaCantidad != "--Seleccionar--" ? string.IsNullOrEmpty(Fila.Cells[txtColumnaCantidad].Value.ToString()) ? 0 : decimal.TryParse(Fila.Cells[txtColumnaCantidad].Value.ToString().Trim(), out cantidad) ? cantidad : 0 : 1;
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidad"].Value = IdUM;
                    row.Cells["i_IdTipoDocumento"].Value = "-1";
                    row.Cells["v_IdProductoDetalle"].Value = IdProducto;
                    row.Cells["d_Cantidad"].Value = StockTotal.ToString();
                    row.Cells["d_Precio"].Value = txtColumnaPrecio != "--Seleccionar--" ? decimal.TryParse(Fila.Cells[txtColumnaPrecio].Value.ToString(), out precio) ? precio.ToString() : "0"  : "1";
                    row.Cells["d_Total"].Value = (StockTotal * 1);
                    row.Cells["v_CodigoInterno"].Value = Fila.Cells[ColumaCodigo].Value.ToString().Trim();
                    row.Cells["v_NombreProducto"].Value = Fila.Cells[txtColumnaNombre].Value.ToString().Trim();
                    row.Cells["v_NroPedido"].Value = txtColumnaPedido != "--Seleccionar--" ? Fila.Cells[txtColumnaPedido].Value.ToString().Trim() : string.Empty;
                    row.Cells["Empaque"].Value = Empaque.ToString();
                    row.Cells["i_IdUnidadMedidaProducto"].Value = IdUM;
                }
            }
            CalcularTotal();
            #endregion
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ultraPopupControlContainer1.Show();
        }
    }
}
