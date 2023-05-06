using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Requerimientos.NBS;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using Infragistics.Win.UltraWinMaskedEdit;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmFormatoUnicoFacturacion : Form
    {
        private TipoAccion _tipoAccion;
        readonly DocumentoBL objDocumentoBl = new DocumentoBL();
        private string _strIdFormatoUnico;
        public string IdFormatoUnico = string.Empty;
        private const int DocumentoFuf = 437;

        #region Temporales
        private List<nbs_formatounicofacturaciondetalleDto> tempInsertar = new List<nbs_formatounicofacturaciondetalleDto>();
        private List<nbs_formatounicofacturaciondetalleDto> tempEditar = new List<nbs_formatounicofacturaciondetalleDto>();
        private List<nbs_formatounicofacturaciondetalleDto> tempEliminar = new List<nbs_formatounicofacturaciondetalleDto>(); 
        #endregion

        nbs_formatounicofacturacionDto _nbsFormatounicofacturacionDto = new nbs_formatounicofacturacionDto();

        public frmFormatoUnicoFacturacion(TipoAccion ta, string pstrIdFuf = null)
        {
            InitializeComponent();
            _tipoAccion = ta;
            _strIdFormatoUnico = pstrIdFuf;
        }

        private void frmFormatoUnicoFacturacion_Load(object sender, EventArgs e)
        {
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            BackColor = new GlobalFormColors().FormColor;
            ultraPanel1.Appearance.BackColor = new GlobalFormColors().BannerColor;
            UltraStatusbarManager.Inicializar(ultraStatusBar1);
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_PrecioDecimales);
            dtpFechaRegistro.Value = DateTime.Today;
            Utils.Windows.SetLimitesPeriodo(dtpFechaRegistro);
            ObtenerDatos(_strIdFormatoUnico);
        }

        private void FormatoDecimalesGrilla(int DecimalesPrecio)
        {
            string FormatoPrecio;

            UltraGridColumn _dImporteNotarial = this.grdData.DisplayLayout.Bands[0].Columns["d_Importe"];
            UltraGridColumn _dImporteRegistral = this.grdData.DisplayLayout.Bands[0].Columns["d_ImporteRegistral"];

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

        private void CargarCabecera(string id)
        {
            try
            {
                var objOperationResult = new OperationResult();
                _nbsFormatounicofacturacionDto = FormatoUnicoFacturacionBl.ObtenerCabecera(ref objOperationResult, id);

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la transacción.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtPeriodo.Text = _nbsFormatounicofacturacionDto.v_Periodo;
                txtMes.Text = _nbsFormatounicofacturacionDto.v_Mes;
                txtCliente.Text = _nbsFormatounicofacturacionDto.NombreCliente;
                txtCorrelativo.Text = _nbsFormatounicofacturacionDto.v_Correlativo;
                txtNroFormato.Text = _nbsFormatounicofacturacionDto.v_NroFormato;
                txtFacturarA.Text = _nbsFormatounicofacturacionDto.NombreClienteFacturacion;
                txtRucCliente.Text = _nbsFormatounicofacturacionDto.NroDocumentoCliente;
                txtRucCliente.Tag = _nbsFormatounicofacturacionDto.v_IdCliente;
                txtRucFacturarA.Text = _nbsFormatounicofacturacionDto.NroDocumentoClienteFacturacion;
                txtRucFacturarA.Tag = _nbsFormatounicofacturacionDto.v_IdClienteFacturar;
                dtpFechaRegistro.Value = _nbsFormatounicofacturacionDto.t_FechaRegistro ?? DateTime.Now;
                if (_nbsFormatounicofacturacionDto.d_Total != null)
                    txtTotal.Text = _nbsFormatounicofacturacionDto.d_Total.Value.ToString("##.00");
                txtResponsable.Text = _nbsFormatounicofacturacionDto.UsuarioResponsable;
                CargarDetalle(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private void CargarDetalle(string id)
        {
            try
            {
                var objOperationResult = new OperationResult();
                var ds = FormatoUnicoFacturacionBl.ObtenerDetalle(ref objOperationResult, id);
                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la transacción.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                grdData.DataSource = ds;
                if (grdData.Rows.Any()) grdData.Rows.ToList().ForEach(p => p.Cells["i_RegistroTipo"].Value = "NoTemporal");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private void ObtenerDatos(string id)
        {
            var objOperationResult = new OperationResult();
            switch (_tipoAccion)
            {
                case TipoAccion.Nuevo:
                    txtNroFormato.Text = objDocumentoBl.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, DocumentoFuf).ToString("00000000").Trim();
                    txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.FormatoUnicoFacturacion, null, dtpFechaRegistro.Value, _nbsFormatounicofacturacionDto.v_Correlativo, 0);
                    CargarDetalle("");
                    txtResponsable.Text = Globals.ClientSession.v_UserName;
                    break;

                case TipoAccion.Editar:
                    CargarCabecera(id);
                    btnBuscarOrdenes.Enabled = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            var objOperationResult = new OperationResult();
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.FormatoUnicoFacturacion, _nbsFormatounicofacturacionDto.t_FechaRegistro, dtpFechaRegistro.Value, _nbsFormatounicofacturacionDto.v_Correlativo, 0);
        }

        private void txtRucCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmBuscarCliente f = new frmBuscarCliente("V", "");
            f.ShowDialog();

            if (f._IdCliente != null)
            {
                txtRucCliente.Text = f._NroDocumento;
                txtRucCliente.Tag = f._IdCliente;
                txtCliente.Text = f._RazonSocial;
                txtRucFacturarA.Text = f._NroDocumento;
                txtRucFacturarA.Tag = f._IdCliente;
                txtFacturarA.Text = f._RazonSocial;

            }
        }

        private void ultraGroupBox3_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscarOrdenes_Click(object sender, EventArgs e)
        {
            var idCliente = txtRucCliente.Tag != null ? txtRucCliente.Tag.ToString() : string.Empty;
            if (!string.IsNullOrEmpty(idCliente))
            {
                var f = new frmOrdenesTrabajoPendientes(idCliente);
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    grdData.DataSource = f.OrdenTrabajoDetalle;
                    if (grdData.Rows.Any())
                    {
                        grdData.Rows.ToList().ForEach(p => { 
                            p.Cells["i_RegistroTipo"].Value = "Temporal"; 
                            p.Cells["i_RegistroEstado"].Value = "Modificado";
                          //  p.Cells["i_FacturadoContabilidad"].Value = "0";
                        });
                    }
                    txtTotal.Text = CalcularTotal().ToString("#.00");
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                var objOperationResult = new OperationResult();
                switch (_tipoAccion)
                {
                    case TipoAccion.Nuevo:
                        _nbsFormatounicofacturacionDto = new nbs_formatounicofacturacionDto
                        {
                            d_Total = !string.IsNullOrEmpty(txtTotal.Text.Trim())
                                ? decimal.Parse(txtTotal.Text.Trim())
                                : 0,
                            v_Correlativo = txtCorrelativo.Text,
                            v_IdCliente = txtRucCliente.Tag != null
                                ? txtRucCliente.Tag.ToString()
                                : string.Empty,
                            v_Mes = txtMes.Text,
                            v_Periodo = txtPeriodo.Text,
                            v_NroFormato = txtNroFormato.Text,
                            t_FechaRegistro = dtpFechaRegistro.Value,
                            i_Facturado = 0,
                            v_IdClienteFacturar = txtRucFacturarA.Tag != null ? txtRucFacturarA.Tag.ToString() : null
                        };
                        LlenarTemporales();
                        _strIdFormatoUnico = FormatoUnicoFacturacionBl.InsertarFormatoUnicoFacturacion(ref objOperationResult,
                            _nbsFormatounicofacturacionDto, tempInsertar, Globals.ClientSession.GetAsList());
                        break;

                    case TipoAccion.Editar:
                        _nbsFormatounicofacturacionDto.d_Total = !string.IsNullOrEmpty(txtTotal.Text.Trim())
                            ? decimal.Parse(txtTotal.Text.Trim())
                            : 0;
                        _nbsFormatounicofacturacionDto.v_Correlativo = txtCorrelativo.Text;
                        _nbsFormatounicofacturacionDto.v_IdCliente = txtRucCliente.Tag != null
                            ? txtRucCliente.Tag.ToString()
                            : string.Empty;
                        _nbsFormatounicofacturacionDto.v_Mes = txtMes.Text;
                        _nbsFormatounicofacturacionDto.v_Periodo = txtPeriodo.Text;
                        _nbsFormatounicofacturacionDto.v_NroFormato = txtNroFormato.Text;
                        _nbsFormatounicofacturacionDto.t_FechaRegistro = dtpFechaRegistro.Value;
                        _nbsFormatounicofacturacionDto.i_Facturado = 0;
                        _nbsFormatounicofacturacionDto.v_IdClienteFacturar = txtRucFacturarA.Tag != null ? txtRucFacturarA.Tag.ToString() : null;
                        LlenarTemporales();
                        FormatoUnicoFacturacionBl.ActualizarFormatoUnicoFacturacion(ref objOperationResult,
                            _nbsFormatounicofacturacionDto, tempInsertar, tempEditar, tempEliminar,
                            Globals.ClientSession.GetAsList());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (objOperationResult.Success == 0)
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                        @"Error en la transacción.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    MessageBox.Show("Registro Guardado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    _tipoAccion = TipoAccion.Editar;
                    CargarCabecera(_strIdFormatoUnico);
                    IdFormatoUnico = _strIdFormatoUnico;
                }

                tempEliminar = new List<nbs_formatounicofacturaciondetalleDto>();
                tempInsertar = new List<nbs_formatounicofacturaciondetalleDto>();
                tempEditar = new List<nbs_formatounicofacturaciondetalleDto>();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdData_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            e.Cell.Row.Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void LlenarTemporales()
        {
            try
            {
                if (!grdData.Rows.Any()) return;
                foreach (var fila in grdData.Rows)
                {
                    switch (fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                var _planillaafechrsexttardDto = (nbs_formatounicofacturaciondetalleDto) fila.ListObject;
                                tempInsertar.Add(_planillaafechrsexttardDto);
                            }
                            break;

                        case "NoTemporal":
                            if (fila.Cells["i_RegistroEstado"].Value != null && fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                var _planillaafechrsexttardDto = (nbs_formatounicofacturaciondetalleDto) fila.ListObject;
                                tempEditar.Add(_planillaafechrsexttardDto);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private decimal CalcularTotal()
        {
            try
            {
                if (!grdData.Rows.Any()) return 0;
                var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
                filas = filas.Where(f => f.Cells["d_Total"].Value != null).ToList();
                return filas.Sum(f => decimal.Parse(f.Cells["d_Total"].Value.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                return 0;
            }
        }

        private void txtRucFacturarA_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmBuscarCliente f = new frmBuscarCliente("V", "");
            f.ShowDialog();

            if (f._IdCliente != null)
            {
                txtRucFacturarA.Text = f._NroDocumento;
                txtRucFacturarA.Tag = f._IdCliente;
                txtFacturarA.Text = f._RazonSocial;
            }
        }

       

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_DescripcionTemporal":
                    var des = new FrmEditarDescripcion(e.Cell.Value != null ? e.Cell.Value.ToString() : string.Empty);
                    des.FormClosing += delegate
                    {
                        e.Cell.Value = des.Descripcion;
                    };

                    des.ShowDialog();
                    break;
            }
        }
    }
}
