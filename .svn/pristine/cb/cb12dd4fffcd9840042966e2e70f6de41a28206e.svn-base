using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Letras.BL;
using SAMBHS.Venta.BL;


namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmConsultaHistorialPagosVenta : Form
    {
        private ventaDto _ventaDto;
        private readonly VentaBL _objVentasBL = new VentaBL();
        private TipoBusqueda tipoBusqueda;

        public frmConsultaHistorialPagosVenta(string idVenta, TipoBusqueda _tipoBusqueda, ventaDto _venta = null)
        {
            InitializeComponent();
            _ventaDto = _venta;
            switch (_tipoBusqueda)
            {
                case TipoBusqueda.Venta:
                    ultraLabel9.Visible = false;
                    dtpFechaVencimiento.Visible = false;
                    ObtenerDataVenta(idVenta);
                    break;

                case TipoBusqueda.Letra:
                    ObtenerDataLetra(idVenta, _ventaDto);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("_tipoBusqueda", _tipoBusqueda, null);
            }

            tipoBusqueda = _tipoBusqueda;
        }

        private void frmConsultaHistorialPagosVenta_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var band = this.ultraGrid1.DisplayLayout.Bands[0];

            #region Configura el Sumario de Aportación

            var aportacionSummary = band.Summaries.Add(
                "TotalCobranza", // Give an identifier (key) for this summary
                SummaryType.Custom, // Summary type is custom
                new AportacionSummary(), // Our custom summary calculator
                band.Columns["Pago"], // Column being summarized. Just use Unit Price column.
                SummaryPosition.Left, // Position the summary on the left of summary footer
                null // Since SummaryPosition is Left, pass in null
                );

            aportacionSummary.DisplayFormat = "Cobranza Total = {0:######.00}";
            aportacionSummary.Appearance.TextHAlign = HAlign.Right;
            aportacionSummary.SummaryPosition = SummaryPosition.Right;
            aportacionSummary.Appearance.FontData.Bold = DefaultableBoolean.True;
            aportacionSummary.Appearance.ForeColor = Color.Black;
            aportacionSummary.Appearance.BackColor = Color.LightGray;

            #endregion

            band.SummaryFooterCaption = @"Resultados";
            this.ultraGrid1.DisplayLayout.Override.SummaryFooterAppearance.FontData.Bold = DefaultableBoolean.True;
            this.ultraGrid1.DisplayLayout.Override.SummaryFooterAppearance.BackColor = Color.White;
            this.ultraGrid1.DisplayLayout.Override.SummaryFooterAppearance.ForeColor = Color.Black;

            this.ultraGrid1.DisplayLayout.Override.SummaryFooterCaptionVisible = DefaultableBoolean.True;
            this.ultraGrid1.DisplayLayout.Override.SummaryFooterCaptionAppearance.ForeColor = Color.Black;
        }

        private void ObtenerDataVenta(string idVenta)
        {
            var objOperationResult = new OperationResult();
            _ventaDto = new VentaBL().ObtenerVentaCabecera(ref objOperationResult, idVenta);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Text = string.Format("Historial de Cobranzas de la Venta: {0}",
                _ventaDto.v_Mes.Trim() + "-" + _ventaDto.v_Correlativo);
            txtCliente.Text = _ventaDto.NombreCliente;
            txtCorrelativoDocumento.Text = _ventaDto.v_CorrelativoDocumento;
            txtMoneda.Text = _ventaDto.i_IdMoneda.Value == 1 ? "Soles" : "Dólares";
            txtRucDni.Text = _ventaDto.NroDocCliente;
            txtSaldo.Text = _ventaDto.Saldo.ToString("00.00");
            txtSerieDocumento.Text = _ventaDto.v_SerieDocumento;
            txtTotalVenta.Text = _ventaDto.d_Total.Value.ToString("00.00");
            dtpFechaEmision.Value = _ventaDto.t_FechaRegistro.Value;
            var dsHistorial = new VentaBL().BuscarHistorialPagos(ref objOperationResult, idVenta);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnCobrar.Enabled = _ventaDto.i_IdTipoDocumento != 7 && _ventaDto.Saldo > 0;
            lblSinMovimientos.Visible = !dsHistorial.Any();
            ultraGrid1.DataSource = dsHistorial;
            if (!dsHistorial.Any(p => p.EsLetra))
            {
                ultraGrid1.DisplayLayout.Bands[0].Columns["SaldoLetra"].Hidden = true;
                ultraGrid1.DisplayLayout.Bands[0].Columns["_VerHistorialLetras"].Hidden = true;
            }
            else
            {
                ultraGrid1.Rows.ToList()
                    .Where(f => !Convert.ToBoolean(f.Cells["EsLetra"].Value.ToString()))
                    .ToList()
                    .ForEach(fila =>
                    {
                        fila.Cells["SaldoLetra"].Activation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
                        fila.Cells["_VerHistorialLetras"].Activation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
                    });
            }
            lblNotaCreditosActivas.Text = new VentaBL().ObtieneNotaCreditoActivas(_ventaDto.v_SerieDocumento,
                _ventaDto.v_CorrelativoDocumento, _ventaDto.i_IdTipoDocumento ?? -1);
        }

        private void ObtenerDataLetra(string idLetra, ventaDto venta)
        {
            var objOperationResult = new OperationResult();
            var letradetalle = new LetrasBL().ObtenerLetraDetalleConsulta(ref objOperationResult, idLetra);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Text = string.Format("Historial de Cobranzas de la Letra: {0}",
                _ventaDto.v_Mes.Trim() + "-" + _ventaDto.v_Correlativo);
            txtCliente.Text = venta.NombreCliente;
            txtMoneda.Text = venta.i_IdMoneda.Value == 1 ? "Soles" : "Dólares";
            txtRucDni.Text = venta.NroDocCliente;
            txtSaldo.Text = letradetalle.Saldo.ToString("00.00");
            txtSerieDocumento.Text = letradetalle.v_Serie;
            txtCorrelativoDocumento.Text = letradetalle.v_Correlativo;

            var dsHistorial = new LetrasBL().BuscarHistorialPagos(ref objOperationResult, idLetra);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!dsHistorial.Any()) return;
            ultraGrid1.DataSource = dsHistorial;
            if (!dsHistorial.Any(p => p.EsLetra))
            {
                ultraGrid1.DisplayLayout.Bands[0].Columns["SaldoLetra"].Hidden = true;
                ultraGrid1.DisplayLayout.Bands[0].Columns["_VerHistorialLetras"].Hidden = true;
            }
            else
            {
                ultraGrid1.Rows.ToList()
                    .Where(f => !Convert.ToBoolean(f.Cells["EsLetra"].Value.ToString()))
                    .ToList()
                    .ForEach(fila =>
                    {
                        fila.Cells["SaldoLetra"].Activation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
                        fila.Cells["_VerHistorialLetras"].Activation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
                    });
            }
        }

        private void ultraGrid1_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Column.Key == "_VerHistorialLetras")
            {
                var idLetra = ultraGrid1.ActiveRow.Cells["IdDocumento"].Value.ToString();
                var saldo = decimal.Parse(ultraGrid1.ActiveRow.Cells["SaldoLetra"].Value.ToString());
                var f = new frmConsultaHistorialPagosVenta(idLetra, TipoBusqueda.Letra, _ventaDto);
                f.ShowDialog();
            }

            if (e.Cell.Column.Key == "_VerDocumento")
            {
                if (!Convert.ToBoolean(e.Cell.Row.Cells["EsLetra"].Value.ToString()))
                {
                    var idCobranza = ultraGrid1.ActiveRow.Cells["IdDocumento"].Value.ToString();
                    frmCobranza f = new frmCobranza("Edicion", idCobranza, null, true);
                    f.ShowDialog();
                    ObtenerDataVenta(_ventaDto.v_IdVenta);
                }
            }
        }

        private class AportacionSummary : ICustomSummaryCalculator
        {
            private decimal totals = 0;

            internal AportacionSummary()
            {
            }

            public void BeginCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {

                // Begins the summary for the SummarySettings object passed in. Implementation of 
                // this method should reset any state variables used for calculating the summary.
                this.totals = 0;

            }

            public void AggregateCustomSummary(SummarySettings summarySettings, UltraGridRow row)
            {
                decimal aportacion = 0;

                aportacion = aportacion + decimal.Parse(row.Cells["Pago"].Value.ToString());
                totals = totals + aportacion;

            }

            public object EndCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                // This gets called when the every row has been processed so here is where we
                // would return the calculated summary value.
                return this.totals;
            }
        }

        public enum TipoBusqueda
        {
            Venta,
            Letra
        }

        private void ultraGrid1_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["Estado"].Value.ToString() == "0")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (tipoBusqueda != TipoBusqueda.Venta) return;
            var objOperationResult = new OperationResult();
            if (_objVentasBL.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Today.Date) == "0")
            {
                UltraMessageBox.Show("No se ha registrado ningún tipo de cambio para el día de hoy.", "Sistema",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (_objVentasBL.TieneCobranzaPendiente(_ventaDto.v_IdVenta))
            {
                frmCobranzaRapida frm = new frmCobranzaRapida(_ventaDto.v_IdVenta, _ventaDto.i_IdTipoDocumento ?? -1, -1);
                frm.ShowDialog();
                ObtenerDataVenta(_ventaDto.v_IdVenta);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRecalcularSaldo_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            new CobranzaBL().RecalcularSaldoVenta(ref objOperationResult, _ventaDto.v_IdVenta,
                Globals.ClientSession.GetAsList(), false, new DocumentoBL().DocumentoEsInverso(_ventaDto.i_IdTipoDocumento ?? 0));

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Saldo Recalculado!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
