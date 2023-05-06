using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmDocumentos : Form
    {
        DocumentoBL _objBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        public documentoDto _objDocumentosDto = new documentoDto();
        documentoDto _documentosDto = new documentoDto();
        documentorolDto _documentoRolDto = new documentorolDto();
        string _Mode;
        string strFilterExpression;
        public frmDocumentos(string Parametro)
        {
            InitializeComponent();
            //panel1.BackColor = frmMaster.ColorSystemForms;
            //Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            //appearance7.BackColor = frmMaster.ColorSystemForms;
            //appearance7.BackColor2 = frmMaster.ColorSystemForms;
            //this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance7;
        }

        private void frmDocumentos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            _Mode = "New";
            BindGrid();
            grdData.DisplayLayout.Bands[0].Columns["i_UsadoCompras"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            grdData.DisplayLayout.Bands[0].Columns["i_UsadoContabilidad"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            grdData.DisplayLayout.Bands[0].Columns["i_UsadoLibroDiario"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            grdData.DisplayLayout.Bands[0].Columns["i_UsadoTesoreria"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            grdData.DisplayLayout.Bands[0].Columns["i_UsadoVentas"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            grdData.DisplayLayout.Bands[0].Columns["i_RequiereSerieNumero"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            grdData.DisplayLayout.Bands[0].Columns["i_UsadoPedidoCotizacion"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            CLR();
            ActivarBotones(false, false, false, false, true);
        }

        #region Botones
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Nuevo();
            _Mode = "New";
            ActivarBotones(false, false, true, true, false);
            txtCodDocumento.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            _Mode = "Edit";
            EdicionActivado();
            ActivarBotones(false, false, true, true, false);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int intDocCode = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells[0].Value.ToString());
            if (intDocCode != null && intDocCode != 0)
            {

                OperationResult objOperationResult = new OperationResult();
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    _objBL.DocumentoBorrar(ref objOperationResult, intDocCode, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(
                            objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                            objOperationResult.AdditionalInformation, "Error en el Proceso", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    BindGrid();
                    Nuevo();
                    EdicionDesactivado();
                    ActivarBotones(false, false, false, false, true);
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            strFilterExpression = null;
            
            if (uvDocumentos.Validate(true, false).IsValid)
            {
                if (txtCodDocumento.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Código apropiado para el Documento.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodDocumento.Focus();
                    return;
                }


                if (_Mode == "New")
                {
                    if (new DocumentoBL().SiglasNoDisponibles(txtSigla.Text.Trim()))
                    {
                        MessageBox.Show("Las siglas del documento ya están siendo usadas en otro documento.!", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (RevisaCodigoDocumento() == 0) return;
                    if (RevisaSiglaDocumento() == 0) return;

                    _documentosDto = new documentoDto
                    {
                        i_CodigoDocumento = int.Parse(txtCodDocumento.Text.Trim()),
                        i_UsadoCompras = chkUsadoenCompras.Checked ? 1 : 0,
                        i_UsadoContabilidad = chkUsadoenContabilidad.Checked ? 1 : 0,
                        i_UsadoLibroDiario = chkUsadoenLibroDiario.Checked ? 1 : 0,
                        i_UsadoTesoreria = chkUsadoenTesoreria.Checked ? 1 : 0,
                        i_UsadoVentas = chkUsadoenVentas.Checked ? 1 : 0,
                        i_UsadoRendicionCuentas = chkUsadoenRendicionCuentas.Checked ? 1 : 0,
                        i_RequiereSerieNumero = chkUsaSerieNumero.Checked ? 1 : 0,
                        i_UsadoPedidoCotizacion = chkUsadoenPedidoCoti.Checked ? 1 : 0,
                        v_Nombre = txtNombres.Text.Trim(),
                        v_Siglas = txtSigla.Text.Trim(),
                        v_NroCuenta = txtCuenta.Text,
                        i_BancoDetraccion =  chkBancoDetraccion.Checked ? 1 : 0
                    };

                    if (rb_Abono.Checked) { _documentosDto.i_Naturaleza = 1; }
                    if (rb_Cargo.Checked) { _documentosDto.i_Naturaleza = 2; }
                    if (rb_Ambos.Checked) { _documentosDto.i_Naturaleza = 3; }
                    _documentosDto.i_UsadoDocumentoInterno = uckUsadoDocumentoInterno.Checked ? 1 : 0;
                    _documentosDto.i_OperacionTransitoria = chkDocumentoTransitorio.Checked ? 1 : 0;
                    _documentosDto.v_provimp_3i = txtProveedor.Text.Trim();
                    _documentosDto.i_UsadoDocumentoContable = uckDocumentoContable.Checked ? 1 : 0;
                    _documentosDto.i_DescontarStock = uckDisminuiraStock.Checked ? 1 : 0;
                    _documentoRolDto = new documentorolDto();
                                                                                                  
                _objBL.DocumentoNuevo(ref objOperationResult, _documentosDto, Globals.ClientSession.GetAsList(), _documentoRolDto);
                }
                else if (_Mode == "Edit")
                {
                    _documentosDto.i_CodigoDocumento = int.Parse(txtCodDocumento.Text.Trim());
                    if (new DocumentoBL().SiglasNoDisponibles(txtSigla.Text.Trim(), _documentosDto.i_CodigoDocumento))
                    {
                        MessageBox.Show("Las siglas del documento ya están siendo usadas en otro documento.!", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _documentosDto.i_UsadoCompras = chkUsadoenCompras.Checked ? 1 : 0;
                    _documentosDto.i_BancoDetraccion = chkBancoDetraccion.Checked ? 1 : 0;
                    _documentosDto.i_UsadoContabilidad = chkUsadoenContabilidad.Checked ? 1 : 0;

                    _documentosDto.i_UsadoLibroDiario = chkUsadoenLibroDiario.Checked ? 1 : 0;

                    _documentosDto.i_UsadoTesoreria = chkUsadoenTesoreria.Checked ? 1 : 0;

                    _documentosDto.i_UsadoVentas = chkUsadoenVentas.Checked ? 1 : 0;

                    _documentosDto.i_UsadoRendicionCuentas = chkUsadoenRendicionCuentas.Checked ? 1 : 0;

                    _documentosDto.i_RequiereSerieNumero = chkUsaSerieNumero.Checked ? 1 : 0;

                    _documentosDto.i_UsadoPedidoCotizacion = chkUsadoenPedidoCoti.Checked ? 1 : 0;

                    if (rb_Abono.Checked)
                    {
                        _documentosDto.i_Naturaleza = 1;
                    }
                    if (rb_Cargo.Checked)
                    {
                        _documentosDto.i_Naturaleza = 2;
                    }
                    if (rb_Ambos.Checked)
                    {
                        _documentosDto.i_Naturaleza = 3;
                    }
                    _documentosDto.i_UsadoDocumentoInterno = uckUsadoDocumentoInterno.Checked ? 1 : 0;
                    _documentosDto.i_UsadoDocumentoContable = uckDocumentoContable.Checked ? 1 : 0;
                    _documentosDto.i_UsadoDocumentoInverso = uckUsadoDocumentoInverso.Checked ? 1 : 0;
                    _documentosDto.v_Nombre = txtNombres.Text.Trim();
                    _documentosDto.v_NroCuenta = txtCuenta.Text;
                    _documentosDto.v_provimp_3i = txtProveedor.Text.Trim();
                    _documentosDto.v_Siglas = txtSigla.Text.Trim();
                    _documentosDto.i_OperacionTransitoria = chkDocumentoTransitorio.Checked ? 1 : 0;
                    _documentosDto.i_DescontarStock = uckDisminuiraStock.Checked ? 1 : 0;

                    // Save the data
                    _objBL.DocumentoActualiza(ref objOperationResult, _documentosDto, Globals.ClientSession.GetAsList(), _documentoRolDto);
                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    EdicionDesactivado();
                    ActivarBotones(true, true, false, false, true);
                    grdData.Enabled = true;
                    Globals.ListaDocumentosContable = _objBL.DocumentoEsContable();
                    Globals.ListaDocumentosInversos = _objBL.DocumentoEsInverso();
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(objOperationResult.ExceptionMessage, "ERROR!!!-", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }
            }
            else
            {
                UltraMessageBox.Show("Por favor corrija la información ingresada. Vea los indicadores de error.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            BindGrid();
            MantenerSeleccion(_documentosDto.i_CodigoDocumento.ToString("000"));
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CLR();
            ActivarBotones(false, false, false, false, true);
        }
        #endregion

        #region Eventos

        private void BindGrid()
        {
            var objData = GetData(0, null, "i_CodigoDocumento ASC", strFilterExpression);

            grdData.DataSource = objData;
        }

        private List<documentoDto> GetData(int pintPageIndex, int? pintPageSize, string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objBL.ObtenDocumentosPaginadoFiltrado(ref objOperationResult, pintPageIndex, pintPageSize, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void txtCodDocumento_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            Entrada_Numerica(e);

            if (!string.IsNullOrEmpty(txtCodDocumento.Text))
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    RevisaCodigoDocumento();
                }
            }
        }

        private void txtCodDocumento_Validated_1(object sender, EventArgs e)
        {
            int numero;
            if (!string.IsNullOrEmpty(txtCodDocumento.Text))
            {
                if (IsNumeric(txtCodDocumento.Text) == true)
                {
                    numero = Convert.ToInt32(txtCodDocumento.Text);
                    txtCodDocumento.Text = string.Format("{0:000}", numero);
                }
                else
                {
                    txtCodDocumento.Text = "";
                }
            }
        }

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].SortedColumns.Add("i_CodigoDocumento", false, true);
            grdData.DisplayLayout.Bands[0].Columns["i_CodigoDocumento"].Format = "000";
        }

        private void txtProveedor_DoubleClick(object sender, EventArgs e)
        {
            //Common.frmEscogerAnexo frm = new Common.frmEscogerAnexo("PROVEEDORES", 1);
            //frm.ShowDialog();
            //_AnexoId = frm._AnexoSeleccionadoId;
            //if (_AnexoId == null) return;
            //txtProveedor.Text = _AnexoId.ToString();
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow.IsActiveRow == true)
            {
                EdicionDesactivado();
                int intDocCode = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells[0].Value.ToString());
                OperationResult objOperationResult = new OperationResult();
                _documentosDto = _objBL.ObtenDocumentosPorID(ref objOperationResult, intDocCode);
                _documentoRolDto = _objBL.ObtenerListadoDocumentosRol(ref objOperationResult, null, null, intDocCode).FirstOrDefault ();

               // cboEnum.Value = _documentoRolDto ==null ?"-1" :_documentoRolDto.i_CodigoEnum.ToString();
               
                txtCodDocumento.Text = string.Format("{0:000}", _documentosDto.i_CodigoDocumento);
                txtNombres.Text = _documentosDto.v_Nombre;
                txtSigla.Text = _documentosDto.v_Siglas;

                uckDocumentoContable.Checked = _documentosDto.i_UsadoDocumentoContable != null && _documentosDto.i_UsadoDocumentoContable != 0;

                if (_documentosDto.i_UsadoCompras == 1) chkUsadoenCompras.Checked = true;
                else chkUsadoenCompras.Checked = false;

                if (_documentosDto.i_UsadoContabilidad == 1) chkUsadoenContabilidad.Checked = true;
                else chkUsadoenContabilidad.Checked = false;

                if (_documentosDto.i_UsadoLibroDiario == 1) chkUsadoenLibroDiario.Checked = true;
                else chkUsadoenLibroDiario.Checked = false;

                chkBancoDetraccion.Checked = (_documentosDto.i_BancoDetraccion ?? 0) == 1;

                if (_documentosDto.i_UsadoRendicionCuentas == 1) chkUsadoenRendicionCuentas.Checked = true;
                else chkUsadoenRendicionCuentas.Checked = false;

                if (_documentosDto.i_UsadoTesoreria == 1) chkUsadoenTesoreria.Checked = true;
                else chkUsadoenTesoreria.Checked = false;

                if (_documentosDto.i_UsadoVentas == 1) chkUsadoenVentas.Checked = true;
                else chkUsadoenVentas.Checked = false;

                if (_documentosDto.i_RequiereSerieNumero == 1) chkUsaSerieNumero.Checked = true;
                else chkUsaSerieNumero.Checked = false;

                if (_documentosDto.i_UsadoPedidoCotizacion == 1) chkUsadoenPedidoCoti.Checked = true;
                else chkUsadoenPedidoCoti.Checked = false;

                if (_documentosDto.i_Naturaleza == 1)
                {
                    rb_Abono.Checked = true;
                }
                if (_documentosDto.i_Naturaleza == 2)
                {
                    rb_Cargo.Checked = true;
                }
                if (_documentosDto.i_Naturaleza == 3)
                {
                    rb_Ambos.Checked = true;
                }
                if (_documentosDto.i_Naturaleza == null)
                {
                    rb_Abono.Checked = false;
                    rb_Cargo.Checked = false;
                    rb_Ambos.Checked = false;
                }
                uckUsadoDocumentoInterno.Checked = _documentosDto.i_UsadoDocumentoInterno != null && _documentosDto.i_UsadoDocumentoInterno != 0;
                uckUsadoDocumentoInverso.Checked = _documentosDto.i_UsadoDocumentoInverso != null && _documentosDto.i_UsadoDocumentoInverso != 0;
                uckDisminuiraStock.Checked = _documentosDto.i_DescontarStock != null && _documentosDto.i_DescontarStock != 0;
                chkDocumentoTransitorio.Checked = _documentosDto.i_OperacionTransitoria == 1;
                txtCuenta.Text = _documentosDto.v_NroCuenta;
                txtDestino.Text = _documentosDto.i_Destino != null ? _documentosDto.i_Destino.ToString() : string.Empty;
                txtProveedor.Text = _documentosDto.v_provimp_3i != null ? _documentosDto.v_provimp_3i.Trim() : string.Empty;
                ActivarBotones(true, true, false, false, true);
            }
        }

        private void txtCuenta_DoubleClick(object sender, EventArgs e)
        {
            frmPlanCuentasConsulta frm = new frmPlanCuentasConsulta("");
            frm.ShowDialog();
            string _NroSubCuenta = frm._NroSubCuenta;
            if (_NroSubCuenta != null) txtCuenta.Text = _NroSubCuenta;
        }
        #endregion

        #region Clases/Validaciones

        private void Entrada_Numerica(KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }
            bool IsDec = false;
            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else if (e.KeyChar == 46)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private int RevisaCodigoDocumento()
        {
            OperationResult objOperationResult = new OperationResult();
            var _objCheck = _objBL.CheckByID(ref objOperationResult, int.Parse(txtCodDocumento.Text));
            if (_objCheck.Count != 0)
            {
                UltraMessageBox.Show("El Código ingresado ya existe en los registros", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtCodDocumento.Focus();
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private int RevisaSiglaDocumento()
        {
            OperationResult objOperationResult = new OperationResult();
            var _objCheck = _objBL.CheckBySigla(ref objOperationResult, txtSigla.Text);
            if (_objCheck.Count != 0)
            {
                UltraMessageBox.Show("La Sigla ingresada ya existe en los registros", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtSigla.Focus();
                return 0;
            }
            return 1;
        }

        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public void EdicionDesactivado()
        {
            txtCodDocumento.Enabled = false;
            txtNombres.Enabled = false;
            txtSigla.Enabled = false;
            chkUsadoenVentas.Enabled = false;
            chkUsadoenCompras.Enabled = false;
            chkUsadoenContabilidad.Enabled = false;
            chkUsadoenLibroDiario.Enabled = false;
            chkUsadoenRendicionCuentas.Enabled = false;
            chkUsadoenTesoreria.Enabled = false;
            chkUsadoenVentas.Enabled = false;
            chkUsaSerieNumero.Enabled = false;
            chkUsadoenPedidoCoti.Enabled = false;
            uckUsadoDocumentoInterno.Enabled = false;
            uckUsadoDocumentoInverso.Enabled = false;
            txtCuenta.Enabled = false;
            txtDestino.Enabled = false;
            txtProveedor.Enabled = false;
            rb_Abono.Enabled = false;
            rb_Ambos.Enabled = false;
            rb_Cargo.Enabled = false;
            grdData.Enabled = true;
            gb01.Enabled = false;
            gb02.Enabled = false;
            gb03.Enabled = false;
            gb04.Enabled = false;
            chkBancoDetraccion.Enabled = false;
        }

        public void EdicionActivado()
        {
            txtCodDocumento.Enabled = false;
            txtNombres.Enabled = true;
            txtSigla.Enabled = int.Parse(txtCodDocumento.Text) >= 100;
            chkUsadoenVentas.Enabled = true;
            chkUsadoenCompras.Enabled = true;
            chkUsadoenContabilidad.Enabled = true;
            chkUsadoenLibroDiario.Enabled = true;
            chkUsadoenRendicionCuentas.Enabled = true;
            chkUsadoenTesoreria.Enabled = true;
            chkUsadoenVentas.Enabled = true;
            chkUsaSerieNumero.Enabled = true;
            chkUsadoenPedidoCoti.Enabled = true;
            txtCuenta.Enabled = true;
            txtDestino.Enabled = true;
            txtProveedor.Enabled = true;
            rb_Abono.Enabled = true;
            rb_Ambos.Enabled = true;
            rb_Cargo.Enabled = true;
            uckUsadoDocumentoInterno.Enabled = true;
            uckUsadoDocumentoInverso.Enabled = true;
            grdData.Enabled = false;
            gb01.Enabled = true;
            gb02.Enabled = true;
            gb03.Enabled = true;
            gb04.Enabled = true;
            chkBancoDetraccion.Enabled = true;
        }

        public void Nuevo()
        {
            txtCodDocumento.Enabled = true;
            txtCodDocumento.Text = "";
            txtNombres.Enabled = true;
            txtNombres.Text = "";
            txtSigla.Enabled = true;
            txtSigla.Text = "";
            chkUsadoenVentas.Enabled = true;
            chkUsadoenVentas.Checked = false;
            chkUsadoenCompras.Enabled = true;
            chkUsadoenCompras.Checked = false;
            chkUsadoenContabilidad.Enabled = true;
            chkUsadoenContabilidad.Checked = false;
            chkUsadoenLibroDiario.Enabled = true;
            chkUsadoenLibroDiario.Checked = false;
            chkUsadoenRendicionCuentas.Enabled = true;
            chkUsadoenRendicionCuentas.Checked = false;
            chkUsadoenTesoreria.Enabled = true;
            chkUsadoenTesoreria.Checked = false;
            chkUsadoenVentas.Enabled = true;
            chkUsadoenVentas.Checked = false;
            chkUsaSerieNumero.Enabled = true;
            chkUsaSerieNumero.Checked = false;
            chkUsadoenPedidoCoti.Enabled = true;
            chkUsadoenPedidoCoti.Checked = false;
            uckDocumentoContable.Checked = true;
            uckUsadoDocumentoInterno.Enabled = true;
            uckUsadoDocumentoInterno.Checked = false;
            chkBancoDetraccion.Checked = false;
            uckUsadoDocumentoInverso.Enabled = true;
            uckUsadoDocumentoInverso.Checked = false;

            txtCuenta.Enabled = true;
            txtCuenta.Text = "";
            txtDestino.Enabled = true;
            txtDestino.Text = "";
            txtProveedor.Enabled = true;
            txtProveedor.Text = "";
            rb_Cargo.Checked = true;
            rb_Abono.Enabled = true;
            rb_Ambos.Enabled = true;
            rb_Cargo.Enabled = true;
            grdData.Enabled = false;
            gb01.Enabled = true;
            gb02.Enabled = true;
            gb03.Enabled = true;
            gb04.Enabled = true;
        }

        public void CLR()
        {
            txtCodDocumento.Text = "";
            txtNombres.Text = "";
            txtSigla.Text = "";
            chkUsadoenVentas.Checked = false;
            chkUsadoenCompras.Checked = false;
            chkUsadoenContabilidad.Checked = false;
            chkUsadoenLibroDiario.Checked = false;
            chkUsadoenRendicionCuentas.Checked = false;
            chkUsadoenTesoreria.Checked = false;
            chkUsadoenVentas.Checked = false;
            chkUsaSerieNumero.Checked = false;
            chkUsadoenPedidoCoti.Checked = false;
            uckUsadoDocumentoInterno.Checked = false;
            uckUsadoDocumentoInverso.Checked = false;
            txtCuenta.Text = "";
            txtDestino.Text = "";
            txtProveedor.Text = "";
            chkBancoDetraccion.Checked = false;
            rb_Cargo.Checked = true;
            rb_Abono.Enabled = true;
            rb_Ambos.Enabled = true;
            rb_Cargo.Enabled = true;
            grdData.Enabled = true;
            EdicionDesactivado();
        }

        public void ActivarBotones(bool editar, bool eliminar, bool guardar, bool cancelar, bool nuevo)
        {
            btnEditar.Enabled = editar;
            btnEliminar.Enabled = eliminar;
            btnGuardar.Enabled = guardar;
            btnCancelar.Enabled = cancelar;
            btnNuevo.Enabled = nuevo;
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["i_CodigoDocumento"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        #endregion

        private void txtCuenta_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCuenta.Text))
            {
                if (!Utils.Windows.EsCuentaImputable(txtCuenta.Text.Trim()))
                {
                    UltraMessageBox.Show("La cuenta ingresada no es una cuenta Imputable válida o no existe.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            txtProveedor.Text = frm._IdProveedor != null ? frm._CodigoProveedor : string.Empty;
        }

        private void chkUsadoenCompras_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsadoenCompras.Checked || chkUsadoenVentas.Checked)
            {
                uckDisminuiraStock.Visible = true;

            }
            else
            {
                uckDisminuiraStock.Checked = false;
                uckDisminuiraStock.Visible = false;

            }
        }

        private void txtCuenta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmPlanCuentasConsulta f = new frmPlanCuentasConsulta("10");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtCuenta.Text = f._NroSubCuenta;
            }
        }

        private void uckUsadoDocumentoInterno_CheckedChanged(object sender, EventArgs e)
        {
            chkDocumentoTransitorio.Visible = uckUsadoDocumentoInterno.Checked;
            chkDocumentoTransitorio.Checked = false;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var f = new frmMigrarDocumentos();
            f.ShowDialog();
            BindGrid();
        }

        private void btnRelacionRol_Click(object sender, EventArgs e)
        {
            var f = new FrmDocumentoRol();
            f.ShowDialog();
        }
        
    }

    
}

