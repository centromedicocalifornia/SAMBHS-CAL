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
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Cobranza.BL;

using SAMBHS.Venta.BL;
using Infragistics.Win;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmCobranzaPendienteConsulta : Form
    {
        CobranzaBL _objCobranzaBL = new CobranzaBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        cobranzapendienteDto cobranzapendienteDto = new cobranzapendienteDto();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        public List<string> _ListaVentas = new List<string>();
        public List<string> _ListaSaldos = new List<string>();
        string _strFilterExpression;
        public string v_IdCliente = string.Empty;
        public bool IncluirLetras, SoloLetras;
        readonly string SiglasDocumento;
        private readonly bool _mostrarDocInversos;

        public frmCobranzaPendienteConsulta(string IdCliente = null, bool _IncluirLetras = true, bool _SoloLetras = false, string _SiglasDocumento = null, bool mostrarDocInversos = false)
        {
            InitializeComponent();
            IncluirLetras = _IncluirLetras;
            if (IdCliente != null)
            {
                OperationResult objOperationResult = new OperationResult();
                var Cliente = new ClienteBL().ObtenerClientePorID(ref objOperationResult, IdCliente);

                txtCliente.Text = Cliente.v_CodCliente;
                txtRazonSocial.Text = (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno + " " + Cliente.v_PrimerNombre + " " + Cliente.v_RazonSocial).Trim();
                btnBuscarCliente.Enabled = false;
                txtCliente.Enabled = false;
                v_IdCliente = Cliente.v_IdCliente;
                SoloLetras = _SoloLetras;
                _mostrarDocInversos = mostrarDocInversos;
            }
            SiglasDocumento = _SiglasDocumento;
        }

        private void frmCobranzaPendienteConsulta_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.All);
            cboTipoDocumento.Value = "-1";
            dtpFechaInicio.Value = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString());
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
            btnSeleccionarTodos.Enabled = !SoloLetras;
            this.Activate();
        }

        private void BindGrid()
        {
            var objData = GetData("FechaRegistro  ASC", _strFilterExpression);

            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);

            if (grdData.Rows.Count > 0)
            {
                for (int i = 0; i < grdData.Rows.Count(); i++)
                {
                    grdData.Rows[i].Cells["Seleccion"].Value = "0";
                    grdData.Rows[i].Cells["Ubicacion"].ToolTipText = grdData.Rows[i].Cells["UbicacionNombreCompleto"].Value != null ? grdData.Rows[i].Cells["UbicacionNombreCompleto"].Value.ToString() : string.Empty;

                    if (!string.IsNullOrEmpty(SiglasDocumento) && grdData.Rows[i].Cells["Ubicacion"].Value != null)
                    {
                        string Ubicacion = grdData.Rows[i].Cells["Ubicacion"].Value.ToString();

                        if (!string.IsNullOrEmpty(Ubicacion))
                            grdData.Rows[i].Cells["Seleccion"].Activation = Ubicacion != SiglasDocumento || Convert.ToBoolean(grdData.Rows[i].Cells["EsLetraDescuentoCobrada"].Value.ToString()) ? Activation.Disabled : Activation.AllowEdit;
                        else
                            grdData.Rows[i].Cells["Seleccion"].Activation = Activation.AllowEdit;
                    }

                    //Se necesita ocultar las notas de credito si la consulta no es para canjear a letras.
                    if (!_mostrarDocInversos)
                        grdData.Rows[i].Hidden = grdData.Rows[i].Cells["EsDocInverso"].Value != null && grdData.Rows[i].Cells["EsDocInverso"].Value.ToString() == "1";
                }
            }
        }

        private List<cobranzapendienteDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
                var idCliente = v_IdCliente;
                var TipoDocumento = int.Parse(cboTipoDocumento.Value.ToString());
                var Serie = txtSerieDoc.Text.Trim();
                var correlativo = txtCorrelativoDoc.Text.Trim();

                var objOperationResult = new OperationResult();
                var _objData = _objCobranzaBL.ListarCobranzasPendientes(ref objOperationResult, pstrSortExpression, idCliente, dtpFechaInicio.Value.Date, dtpFechaFin.Value, IncluirLetras, SoloLetras, TipoDocumento, Serie, correlativo);

                if (objOperationResult.Success != 1)
                {
                    UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return _objData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                BindGrid();
                grdData.Focus();
                if (!grdData.Rows.Any()) return;
                var ds = (List<cobranzapendienteDto>)grdData.DataSource;
                var visible = ds.All(p => string.IsNullOrWhiteSpace(p.Estado));
                grdData.DisplayLayout.Bands[0].Columns["Estado"].Hidden = visible;
                grdData.DisplayLayout.Bands[0].Columns["Ubicacion"].Hidden = visible;
                grdData.DisplayLayout.Bands[0].Columns["FechaVencimiento"].Hidden = visible;

            }
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["Seleccion"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        }

        private void btnTerminar_Click(object sender, EventArgs e)
        {
            List<UltraGridRow> Filas = new List<UltraGridRow>();

            grdData.Rows.Where(p => p.Cells["Seleccion"].Value != null && p.Cells["Seleccion"].Value.ToString() == "1").ToList().ForEach(o => Filas.Add(o));

            if (Application.OpenForms["frmCobranza"] != null)
            {
                (Application.OpenForms["frmCobranza"] as Procesos.frmCobranza).RecibirItems(Filas);
            }

  

            this.Close();
        }

        private void btnSeleccionarTodos_Click(object sender, EventArgs e)
        {
            grdData.Rows.Where(p => p.Cells["Seleccion"].Activation == Activation.AllowEdit).ToList().ForEach(o => o.Cells["Seleccion"].Value = "1");
            txtTotal.Text =
                    grdData.Rows.Where(c => c.Cells["Seleccion"].Value.ToString() == "1")
                        .Sum(p => decimal.Parse(p.Cells["d_Saldo"].Value.ToString())).ToString("0.00");
        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
                txtSerieDoc.Clear();
                txtCorrelativoDoc.Clear();
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value | p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                    txtSerieDoc.Clear();
                    txtCorrelativoDoc.Clear();
                }
            }
            txtSerieDoc.Enabled = cboTipoDocumento.Value.ToString() == "-1" ? false : true;
            txtCorrelativoDoc.Enabled = cboTipoDocumento.Value.ToString() == "-1" ? false : true;
        }

        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
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

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
        }

        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            int doc;
            if (int.TryParse(txtCorrelativoDoc.Text, out doc))
                Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc, "{0:00000000}");
        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                txtCliente.Text = frm._NroDocumento;
                txtRazonSocial.Text = frm._RazonSocial;
                v_IdCliente = frm._IdCliente;

            }
        }

        private void txtCliente_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCliente.Text.Trim()))
            {
                v_IdCliente = null;
                txtRazonSocial.Clear();
            }
        }

        private void txtRazonSocial_TextChanged(object sender, EventArgs e)
        {

        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key != "Seleccion") return;
            var editor = e.Cell.EditorResolved;
            e.Cell.Value = editor.Value;
            var seleccionado = e.Cell.Value.ToString() == "1";

            #region Validacion para letras en descuentos

            if (seleccionado)
            {
                var filasSeleccionadas = grdData.Rows.Where(p => p.Cells["Seleccion"].Value.ToString() == "1").ToList();

                if (filasSeleccionadas.Any())
                {
                    var actualFilaEsLetraDescuento = Convert.ToBoolean(e.Cell.Row.Cells["EsLetra"].Value.ToString()) &&
                                                     CobranzaBL.EsLetraDescuento(
                                                         e.Cell.Row.Cells["v_IdVenta"].Value.ToString(), true);

                    var existenSeleccionadasLetrasNoDescuento =
                        filasSeleccionadas.Any(p => !Convert.ToBoolean(p.Cells["EsLetra"].Value.ToString())) ||
                        filasSeleccionadas.Any(p => !CobranzaBL.EsLetraDescuento(p.Cells["v_IdVenta"].Value.ToString(), true));

                    var existenSeleccionadasLetrasDescuento =
                        filasSeleccionadas.Any(p => Convert.ToBoolean(p.Cells["EsLetra"].Value.ToString())) &&
                        filasSeleccionadas.Any(p => CobranzaBL.EsLetraDescuento(p.Cells["v_IdVenta"].Value.ToString(), true));

                    if (actualFilaEsLetraDescuento)
                    {
                        if (existenSeleccionadasLetrasNoDescuento)
                        {
                            MessageBox.Show(
                                @"No se puede seleccionar una Letra en Descuento junto con otras que no lo son.",
                                @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cell.Value = "0";
                        }
                    }
                    else
                    {
                        if (existenSeleccionadasLetrasDescuento)
                        {
                            MessageBox.Show(
                                @"No se puede seleccionar el registro junto con letras en descuento.",
                                @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.Cell.Value = "0";
                        }
                    }
                }
            }

            #endregion

            if (SoloLetras)
            {
                if (seleccionado)
                {
                    grdData.Rows.Where(o => o.Cells["Seleccion"].Value.ToString() == "0").ToList()
                        .ForEach(p =>
                        {
                            p.Cells["Seleccion"].Activation = Activation.Disabled;
                        });
                }
                else
                {
                    grdData.Rows.Where(o => o.Cells["Seleccion"].Value.ToString() == "0").ToList()
                        .ForEach(p =>
                        {
                            p.Cells["Seleccion"].Activation = Activation.AllowEdit;
                        });
                }
            }

            txtTotal.Text =
                grdData.Rows.Where(c => c.Cells["Seleccion"].Value.ToString() == "1")
                    .Sum(p => decimal.Parse(p.Cells["d_Saldo"].Value.ToString())).ToString("0.00");
        }

        private void cboTipoDocumento_ValueChanged(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Value != null && cboTipoDocumento.Value.ToString() != "-1")
            {
                txtSerieDoc.Enabled = true;
                txtCorrelativoDoc.Enabled = true;
            }
            else
            {
                txtSerieDoc.Clear();
                txtCorrelativoDoc.Clear();
                txtSerieDoc.Enabled = false;
                txtCorrelativoDoc.Enabled = false;
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (Convert.ToBoolean(e.Row.Cells["EsLetraDescuentoCobrada"].Value.ToString()))
            {
                e.Row.Appearance.ForeColor = Color.Orange;
                e.Row.ToolTipText = "Ésta letra en descuento tiene un abono por parte del banco.";
            }
        }
    }
}
