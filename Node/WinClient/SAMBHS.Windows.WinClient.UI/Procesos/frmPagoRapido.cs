using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE ;
using SAMBHS.Compra.BL;
using SAMBHS.Cobranza.BL;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
     
    public partial class frmPagoRapido : Form
    {
        string _IdCompra;
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        DatahierarchyBL _objDatahierarchyBL= new DatahierarchyBL ();
        UltraCombo ucTipoDocumento = new UltraCombo();
        PagoBL _objPagoBL = new PagoBL();
        List<KeyValueDTO> _ListadoPagos = new List<KeyValueDTO>();
        pagoDto _objPagoDto = new pagoDto();
        pagodetalleDto _objPagoDetalleDto = new pagodetalleDto();
        compraDto _objCompraDto = new compraDto();
        FormaPagoDocumentoBL _objFormaPagoDocumentoBL = new FormaPagoDocumentoBL();
        string _Mode = "New", IdCobranza = String.Empty, strModo = "Nuevo";
        bool FormaPagoRequiereNroDocumento = false;
        decimal MaxAdelanto;
        int _idMonedaCobranza ;
        #region Temporales Detalles de Pagos
        List<pagodetalleDto> _TempDetalle_AgregarDto = new List<pagodetalleDto>();
        #endregion
        public frmPagoRapido(string IdCompra)
        {
            _IdCompra=IdCompra ;
            InitializeComponent();
        }
        private void frmPagoRapido_Load(object sender, EventArgs e)
        {
            UltraStatusbarManager.Inicializar(ultraStatusBar1);
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString("00");
            CargarCombos();
            CargarDetalle("");
            ObtenerPagoPendiente(_IdCompra);
            #region Inicia valor para la forma de pago
            try
            {
                var Doc = ((List<KeyValueDTO>)cboFormaPago.DataSource).Where(p => p.Value1.Contains("EFECTIVO")).FirstOrDefault();

                if (Doc != null)
                {
                    var IdDocumento = Doc.Id;

                    if (IdDocumento != null)
                    {
                        if (_objCompraDto.i_IdMoneda == 1)
                        {
                            var DocMoneda = ((List<KeyValueDTO>)cboFormaPago.DataSource).Where(p => p.Value1.Contains("EFECTIVO") && p.Value1.Contains("SOLES")).FirstOrDefault();

                            if (DocMoneda != null)
                            {
                                var IdDocumentoMoneda = DocMoneda.Id;

                                if (IdDocumentoMoneda != null)
                                {
                                    cboFormaPago.SelectedValue = IdDocumentoMoneda != null ? IdDocumentoMoneda : "-1";
                                }
                                else
                                {
                                    cboFormaPago.SelectedValue = IdDocumento != null ? IdDocumento : "-1";
                                }
                            }
                            else
                            {
                                cboFormaPago.SelectedValue = IdDocumento != null ? IdDocumento : "-1";
                            }
                        }
                        else
                        {
                            var DocMoneda = ((List<KeyValueDTO>)cboFormaPago.DataSource).Where(p => p.Value1.Contains("EFECTIVO") && p.Value1.Contains("DOLARES")).FirstOrDefault();

                            if (DocMoneda != null)
                            {
                                var IdDocumentoMoneda = DocMoneda.Id;

                                if (IdDocumentoMoneda != null)
                                {
                                    cboFormaPago.SelectedValue = IdDocumentoMoneda != null ? IdDocumentoMoneda : "-1";
                                }
                                else
                                {
                                    cboFormaPago.SelectedValue = IdDocumento != null ? IdDocumento : "-1";
                                }
                            }
                            else
                            {
                                cboFormaPago.SelectedValue = IdDocumento != null ? IdDocumento : "-1";
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadDropDownList(cboFormaPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 46, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value1", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            CargarCombosDetalle();
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
            ucTipoDocumento.DropDownWidth = 330;
            #endregion

            Utils.Windows.LoadUltraComboList(ucTipoDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridTesoreria(ref objOperationResult, null), DropDownListAction.Select);
        }
        private void CargarDetalle(string pstringIdPago)
        {
            OperationResult objOperationResult = new OperationResult();
            try
            {
                grdData.DataSource = _objPagoBL.ObtenerPagoDetalle(ref objOperationResult, pstringIdPago);
          
            }
            catch (Exception ex)
            {
                throw ex;
            }
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
            }
        }
        public void ObtenerPagoPendiente(string IdVenta)
        {
            OperationResult objOperationResult = new OperationResult();
            _objCompraDto = _objPagoBL.ObtenerCobranzaPendientePorCompra(ref objOperationResult, IdVenta);
            txtMoneda.Text = _objCompraDto.i_IdMoneda.ToString() == "1" ? "S/." : "US$.";
            txtMonto.Text = _objCompraDto.SaldoPendiente.ToString();
            txtSaldo.Text = _objCompraDto.SaldoPendiente.ToString();
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (uvValidar.Validate(true, false).IsValid)
            {
                if (grdData.Rows.Count() == 0)
                {
                    //UltraMessageBox.Show("Por Favor ingrese almenos una fila al detalle del documento.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Ingrese almenos una fila al detalle del Pago", timer1);
                    return;
                }
                if (_Mode == "New")
                {
                    List<int> ListaDocumentos = grdData.Rows.Select(p => int.Parse(p.Cells["i_IdDocumento"].Value.ToString())).Distinct().ToList();

                    foreach (int Documento in ListaDocumentos)
                    {
                        _ListadoPagos = _objPagoBL.ObtenerListadoPagos(ref objOperationResult, txtPeriodo.Text, txtMes.Text, Documento);
                        
                        if (_ListadoPagos.Count() == 0)
                        {
                            _objPagoDto.v_Correlativo = "00000001";
                        }
                        else
                        {
                            _objPagoDto.v_Correlativo = (int.Parse(_ListadoPagos[_ListadoPagos.Count() - 1].Value1) + 1).ToString("00000000");
                        }

                        List<UltraGridRow> Filas = grdData.Rows.Where(x => x.Cells["i_IdDocumento"].Value.ToString() == Documento.ToString()).ToList();

                        while (_objPagoBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, _objPagoDto.v_Correlativo, Documento) == false)
                        {
                            _objPagoDto.v_Correlativo = (int.Parse(_objPagoDto.v_Correlativo) + 1).ToString("00000000");
                        }

                        _objPagoDto.i_IdTipoDocumento = Documento;
                        _objPagoDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                        _objPagoDto.v_Nombre = null;
                        _objPagoDto.v_Mes = txtMes.Text;
                        _objPagoDto.v_Periodo = txtPeriodo.Text;
                        _objPagoDto.d_TipoCambio = decimal.Parse(_objPagoBL.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Today.Date));
                        _objPagoDto.v_Glosa = null;
                        _objPagoDto.v_Mes = txtMes.Text.Trim();
                        _objPagoDto.v_Periodo = txtPeriodo.Text.Trim();
                        _objPagoDto.d_TotalSoles = Filas.Sum(p => decimal.Parse(p.Cells["d_ImporteSoles"].Value.ToString()));
                        _objPagoDto.i_IdEstado = 1;
                        _objPagoDto.i_IdMoneda = _objCompraDto.i_IdMoneda;
                        _objPagoDto.i_IdMedioPago = _objPagoBL.DevuelveMedioPago(ref objOperationResult, Filas[0].Cells["FormaPago"].Value.ToString());
                        foreach (UltraGridRow Fila in Filas)
                        {
                            LlenarTemporalesPago(Fila.Cells["FormaPago"].Value.ToString());
                        }

                        IdCobranza = _objPagoBL.InsertarPago(ref objOperationResult, _objPagoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);
                        _TempDetalle_AgregarDto = new List<pagodetalleDto>();
                        _objPagoDto = new pagoDto();
                    }
                }

                if (objOperationResult.Success == 1)
                {
                    strModo = "Guardado";
                    //IdCobranza = _objPagoDto.v_IdCobranza;
                    //lblEstado.Text = "**Cobranza Guardada Correctamente**";
                    UltraStatusbarManager.Mensaje(ultraStatusBar1, "Cobranza Guardada Correctamente", timer1);
                    btnGuardar.Enabled = false;
                    btnImprimir.Enabled = true;
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
        private void LlenarTemporalesPago(string FormaPago)
        {
            if (grdData.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["FormaPago"].Text == FormaPago)
                    {
                        switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                        {
                            case "Temporal":
                                if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                                {
                                    
                                    _objPagoDetalleDto = new pagodetalleDto();
                                    _objPagoDetalleDto.d_NetoXCobrar = Fila.Cells["d_NetoXCobrar"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_NetoXCobrar"].Value.ToString());
                                    _objPagoDetalleDto.v_IdCompra = _objCompraDto.v_IdCompra;
                                    _objPagoDetalleDto.i_IdTipoDocumentoRef = Fila.Cells["i_IdTipoDocumentoRef"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumentoRef"].Value.ToString());
                                    _objPagoDetalleDto.d_ImporteDolares = Fila.Cells["d_ImporteDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                    _objPagoDetalleDto.d_ImporteSoles = Fila.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                    _objPagoDetalleDto.v_DocumentoRef = Fila.Cells["v_DocumentoRef"].Value == null ? null : Fila.Cells["v_DocumentoRef"].Value.ToString();
                                    _objPagoDetalleDto.i_IdFormaPago = Fila.Cells["i_IdFormaPago"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdFormaPago"].Value.ToString());
                                    _objPagoDetalleDto.Moneda = txtMoneda.Text.Trim();
                                    _TempDetalle_AgregarDto.Add(_objPagoDetalleDto);
                                }
                                break;
                        }
                    }
                }
            }

        }

        private void btnEliminarDetalle_Click(object sender, EventArgs e)
        {
            grdData.DeleteSelectedRows(false);
            CalcularTotales();
        }

        private void CalcularTotales()
        {
            decimal SumNetoXCobrar = 0;
            if (grdData.Rows.Count() > 0)
            {
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["d_ImporteSoles"].Value == null) { Fila.Cells["d_ImporteSoles"].Value = "0"; }

                    SumNetoXCobrar = SumNetoXCobrar + decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                }
            }
            else
            {
                SumNetoXCobrar = 0;
            }

            txtSaldo.Text = (_objCompraDto.SaldoPendiente - SumNetoXCobrar).ToString("0.00");
            if (decimal.Parse(txtSaldo.Text) <= 0)
            {
                btnCobrar.Enabled = false;
                
                //btnImprimir.Enabled = true;
            }
            else
            {
                btnImprimir.Enabled = false;
            }
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Value == null || cboTipoDocumento.Value.ToString() == "-1")
            {
                UltraMessageBox.Show("Por favor ingrese un Documento primero", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboTipoDocumento.Focus();
                return;
            }

            if (FormaPagoRequiereNroDocumento && string.IsNullOrEmpty(txtDocumento.Text.Trim()))
            {
                txtDocumento.Focus();
                return;
            }

            if (cboFormaPago.Text.Contains("ADELANTO") && MaxAdelanto == 0)
            {
                UltraMessageBox.Show("Por favor ingrese un documento de adelanto válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtDocumento.Focus();
                return;
            }

            OperationResult objOperationResult = new OperationResult();
            if (!string.IsNullOrEmpty(txtMonto.Text.Trim()))
            {
                if (decimal.Parse(txtMonto.Text) <= 0)
                {
                    UltraMessageBox.Show("Por favor ingrese un monto correcto", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtMonto.Focus();
                    return;
                }
                if (decimal.Parse(txtMonto.Text.Trim()) > decimal.Parse(txtSaldo.Text))
                {
                    UltraMessageBox.Show("Por favor ingrese un monto correcto", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtMonto.Focus();
                    return;
                }

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["FormaPago"].Value.ToString() == cboFormaPago.Text)
                    {
                        txtMonto.Clear();
                        CalcularTotales();
                        if (txtSaldo.Text != _objCompraDto.SaldoPendiente.ToString()) txtMonto.Focus();
                         
                        btnCobrar.Enabled = false;
                        CalcularValoresDetalle();
                        return;
                    }
                }

                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                string IdTipoDocumento = "-1";
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["v_IdVenta"].Value = _objCompraDto.v_IdCompra .ToString();

                if (FormaPagoRequiereNroDocumento)
                {
                    string Valor = ((KeyValueDTO)cboFormaPago.SelectedItem).Value3.ToString();
                    IdTipoDocumento = !string.IsNullOrEmpty(Valor) ? Valor : "-1";
                }

                row.Cells["i_IdTipoDocumentoRef"].Value = IdTipoDocumento;
                row.Cells["v_DocumentoRef"].Value = txtDocumento.Text.Trim();
                row.Cells["i_IdMoneda"].Value = _objCompraDto.i_IdMoneda.ToString();
                row.Cells["d_NetoXCobrar"].Value = txtSaldo.Text;
                row.Cells["d_ImporteSoles"].Value = txtMonto.Text.Trim();
                row.Cells["d_ImporteDolares"].Value = "0";
                row.Cells["FormaPago"].Value = cboFormaPago.Text;
                row.Cells["i_IdFormaPago"].Value = cboFormaPago.SelectedValue.ToString();
                row.Cells["i_IdDocumento"].Value = cboTipoDocumento.Value.ToString();
                txtMonto.Clear();
                CalcularTotales();
                if (txtSaldo.Text != _objCompraDto.SaldoPendiente.ToString()) txtMonto.Focus();
                btnCobrar.Enabled = false;
                CalcularValoresDetalle();
                txtMonto.ReadOnly = false;
                txtDocumento.Clear();
            }
            else
            {
                btnCobrar.Enabled = false;
                return;
            }
        }
        private void CalcularValoresDetalle()
        {
            if (grdData.Rows.Count() == 0) return;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
            }
        }
        private void CalcularValoresFila(UltraGridRow Fila)
        {
            CalcularTotales();
        }

        private void txtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtMonto, e);
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdTipoDocumentoRef"].EditorComponent = ucTipoDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumentoRef"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
   
        }

        private void txtDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDocumento.Text) && cboFormaPago.SelectedValue.ToString() != "-1")
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtDocumento.Text.Contains("-"))
                    {
                        string[] SerieCorrelativo = new string[2];
                        SerieCorrelativo = txtDocumento.Text.Split(new Char[] { '-' });
                        try
                        {
                            int serie, correlativo;
                            if (int.TryParse(SerieCorrelativo[0], out serie) && int.TryParse(SerieCorrelativo[1], out correlativo))
                            {
                                txtDocumento.Text = serie.ToString("0000") + "-" + correlativo.ToString("00000000");
                            }
                        }
                        catch (Exception ex)
                        {
                            UltraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MaxAdelanto = 0;
                        return;
                    }

                    var xx = (KeyValueDTO)cboFormaPago.SelectedItem;
                    switch (xx.Value3)
                    {
                        case "7":
                            if (txtDocumento.Text.Contains("-"))
                            {
                               
                                txtMonto.Text = _objPagoBL.DevuelveMontoNotaCredito(txtDocumento.Text.Trim()).ToString("0.00"); 
                                txtMonto.ReadOnly = true;
                            }
                            break;

                        case "433":
                            //if (txtDocumento.Text.Contains("-"))
                            //{
                            //    txtMonto.Text = _objPagoBL.DevuelveSaldoAdelanto(txtDocumento.Text.Trim()).ToString("0.00");
                            //    MaxAdelanto = decimal.Parse(txtMonto.Text);
                            //    txtMonto.ReadOnly = false;
                            //}
                            //else
                            //{
                            //    MaxAdelanto = 0;
                            //}
                            break;
                    }
                }
            }
            else
            {
                //txtMonto.Text = "0";
                MaxAdelanto = 0;
            }
        }

        private void txtDocumento_Validated(object sender, EventArgs e)
        {
            txtDocumento_KeyDown(sender, new KeyEventArgs(Keys.Enter));
        }

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
        }

        private void cboTipoDocumento_ValueChanged(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Value != null && cboTipoDocumento.Value.ToString() != "-1")
            {
                if (!_objDocumentoBL.TieneCuentaValida(int.Parse(cboTipoDocumento.Value.ToString())))
                {
                    UltraMessageBox.Show("El documento con el que intenta pagar no tiene una cuenta contable válida relacionada. \nModifique la información del documento e ingrésele una cuenta válida.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cboTipoDocumento.Value = "-1";
                    return;
                }
                txtSiglas.Text = ((GridKeyValueDTO)cboTipoDocumento.ActiveRow.ListObject).Value2.ToString();
            }
            else
            {
                txtSiglas.Clear();
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

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Cobranza, _objPagoDto.t_FechaRegistro.Value, dtpFechaRegistro.Value, _objPagoDto.v_Correlativo, 0);
         
        }

        private void cboFormaPago_TextChanged(object sender, EventArgs e)
        {
            cboTipoDocumento.Value = _objFormaPagoDocumentoBL.DevuelveComprobantePorFormaPago(int.Parse(cboFormaPago.SelectedValue.ToString()), out FormaPagoRequiereNroDocumento, out _idMonedaCobranza).ToString();
            if (cboFormaPago.SelectedValue.ToString() != "-1" && decimal.Parse(txtSaldo.Text) > 0)
            {
                btnCobrar.Enabled = true;

                txtMonto.ReadOnly = cboTipoDocumento.Value.ToString() == "7" || cboTipoDocumento.Value.ToString() == "8" ? true : false;
                txtDocumento.Enabled = FormaPagoRequiereNroDocumento;
            }
            else
            {
                btnCobrar.Enabled = false;
            }

            if (cboTipoDocumento.Value.ToString() != "-1")
            {
                txtSiglas.Text = ((GridKeyValueDTO)cboTipoDocumento.ActiveRow.ListObject).Value2.ToString();
                cboTipoDocumento.Enabled = false;
            }
            else
            {
                txtSiglas.Clear();
                cboTipoDocumento.Enabled = true;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void ObtenerListadoCobranzas(string pstrPeriodo, string pstrMes)
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    _ListadoCobranzas = _objCobranzaBL.ObtenerListadoCobranzas(ref objOperationResult, pstrPeriodo, pstrMes, TipoDocumento);
        //    switch (strModo)
        //    {
        //        case "Nuevo":
        //            if (_ListadoCobranzas.Count != 0)
        //            {
        //                _MaxV = _ListadoCobranzas.Count() - 1;
        //                _ActV = _MaxV;
        //                txtCorrelativo.Text = (int.Parse(_ListadoCobranzas[_MaxV].Value1) + 1).ToString("00000000");
        //                _Mode = "New";
        //            }
        //            else
        //            {
        //                txtCorrelativo.Text = "00000001";
        //                _Mode = "New";
        //                _MaxV = 1;
        //                _ActV = 1;
        //                _cobranzaDto = new cobranzaDto();
        //            }
        //            break;

        //        case "Guardado":
        //            _MaxV = _ListadoCobranzas.Count() - 1;
        //            _ActV = _MaxV;
        //            if (strIdCobranza == "" | strIdCobranza == null)
        //            {
        //                CargarCabecera(_ListadoCobranzas[_MaxV].Value2);
        //            }
        //            else
        //            {
        //                CargarCabecera(strIdCobranza);
        //            }
        //            break;
        //    }
        //}

    
    
    }
}
