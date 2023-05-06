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
using SAMBHS.Compra.BL;


namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmConsultaHistorialPagosCompra : Form
    {
        private compraDto _compraDto;

        private readonly ComprasBL _objComprasBL = new ComprasBL();
        private TipoBusqueda tipoBusqueda;

        public frmConsultaHistorialPagosCompra(string idVenta, TipoBusqueda _tipoBusqueda, compraDto _venta = null)
        {
            InitializeComponent();
            _compraDto = _venta;
            switch (_tipoBusqueda)
            {
                case TipoBusqueda.Venta:
                    ultraLabel9.Visible = false;
                    dtpFechaVencimiento.Visible = false;
                    ObtenerDataCompra(idVenta);
                    break;

                case TipoBusqueda.Letra:
                    ObtenerDataLetra(idVenta, _compraDto);
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

            aportacionSummary.DisplayFormat = "Pago Total = {0:######.00}";
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
            //this.grdData.DisplayLayout.Override.SummaryFooterCaptionAppearance.BackColor = Color.DarkBlue;
            this.ultraGrid1.DisplayLayout.Override.SummaryFooterCaptionAppearance.ForeColor = Color.Black;
            btnRecalcularSaldosTodosPagos.Visible =  Globals.ClientSession.i_SystemUserId.Equals(1);

                
        }

        private void ObtenerDataCompra(string idCompra)
        {
            var objOperationResult = new OperationResult();
           
            _compraDto = new ComprasBL().ObtenerCompraCabecera(ref  objOperationResult, idCompra);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Text = string.Format("Historial de Pagos de la Compra: {0}",
                _compraDto.v_Mes.Trim() + "-" + _compraDto.v_Correlativo);
            txtCliente.Text = _compraDto.NombreProveedor ;
            txtCorrelativoDocumento.Text = _compraDto.v_CorrelativoDocumento;
            txtMoneda.Text = _compraDto.i_IdMoneda.Value == 1 ? "Soles" : "Dólares";
            txtRucDni.Text = _compraDto.RUCProveedor;
            txtSaldo.Text = _compraDto.Saldo.ToString("00.00");
            txtSerieDocumento.Text = _compraDto.v_SerieDocumento;
            txtTotalVenta.Text = _compraDto.d_Total.Value.ToString("00.00");
            dtpFechaEmision.Value = _compraDto.t_FechaRegistro.Value;
         
            var dsHistorial = new ComprasBL().BuscarHistorialPagos(ref objOperationResult, idCompra);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                dtpFechaVencimiento.Value = dsHistorial.Any() ? dsHistorial.FirstOrDefault().FechaVencimiento : DateTime.Now;
            }
            catch (Exception ex)
            {
                dtpFechaVencimiento.Value = DateTime.Now;
            }
            btnCobrar.Enabled = _compraDto.Saldo > 0;
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
        }




        private void ObtenerDataLetra(string idLetra, compraDto compra)
        {
            var objOperationResult = new OperationResult();

            var letradetalle = new LetrasBL().ObtenerLetraPagarDetalleConsulta(ref objOperationResult, idLetra);
         
            
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Text = string.Format("Historial de Pagos de la Letra: {0}",
                letradetalle.v_Serie.Trim () +"-"+ letradetalle.v_Correlativo.Trim ());
               // compra.v_Mes.Trim() + "-" + compra.v_Correlativo);
           // _compraDto.v_Mes.Trim() + "-" + _compraDto.v_Correlativo.Trim ;
            txtCliente.Text = compra.NombreProveedor;
            txtMoneda.Text = compra.i_IdMoneda.Value == 1 ? "Soles" : "Dólares";
            txtRucDni.Text = compra.RUCProveedor;
            txtSaldo.Text = letradetalle.Saldo.ToString("00.00");
            txtSerieDocumento.Text = letradetalle.v_Serie;
            txtCorrelativoDocumento.Text = letradetalle.v_Correlativo;

            var dsHistorial = new LetrasBL().BuscarHistorialPagosCompra(ref objOperationResult, idLetra);
            dtpFechaVencimiento.Value = dsHistorial.Any() ? dsHistorial.FirstOrDefault().FechaVencimiento : DateTime.Now;
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

            lblSinMovimientos.Visible = !dsHistorial.Any();
        }


        //private void ObtenerDataLetra(string idLetra, compraDto compra)
        //{
        //    var objOperationResult = new OperationResult();
        //    var letradetalle = new LetrasBL().ObtenerLetraDetalleConsulta(ref objOperationResult, idLetra);
        //    if (objOperationResult.Success == 0)
        //    {
        //        MessageBox.Show(
        //            objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
        //            objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //    Text = string.Format("Historial de Pagos de la Letra: {0}",
        //        _compraDto.v_Mes.Trim() + "-" + _compraDto.v_Correlativo);
        //    txtCliente.Text = compra.NombreProveedor;
        //    txtMoneda.Text = compra.i_IdMoneda.Value == 1 ? "Soles" : "Dólares";
        //    txtRucDni.Text = compra.RUCProveedor;
        //    txtSaldo.Text = letradetalle.Saldo.ToString("00.00");
        //    txtSerieDocumento.Text = letradetalle.v_Serie;
        //    txtCorrelativoDocumento.Text = letradetalle.v_Correlativo;

        //    var dsHistorial = new LetrasBL().BuscarHistorialPagosCompra(ref objOperationResult, idLetra);
        //    if (objOperationResult.Success == 0)
        //    {
        //        MessageBox.Show(
        //            objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
        //            objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    if (!dsHistorial.Any()) return;
        //    ultraGrid1.DataSource = dsHistorial;
        //    if (!dsHistorial.Any(p => p.EsLetra))
        //    {
        //        ultraGrid1.DisplayLayout.Bands[0].Columns["SaldoLetra"].Hidden = true;
        //        ultraGrid1.DisplayLayout.Bands[0].Columns["_VerHistorialLetras"].Hidden = true;
        //    }
        //    else
        //    {
        //        ultraGrid1.Rows.ToList()
        //            .Where(f => !Convert.ToBoolean(f.Cells["EsLetra"].Value.ToString()))
        //            .ToList()
        //            .ForEach(fila =>
        //            {
        //                fila.Cells["SaldoLetra"].Activation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
        //                fila.Cells["_VerHistorialLetras"].Activation = Infragistics.Win.UltraWinGrid.Activation.Disabled;
        //            });
        //    }
        //}

        private void ultraGrid1_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Column.Key == "_VerHistorialLetras")
            {
                var idLetra = ultraGrid1.ActiveRow.Cells["IdDocumento"].Value.ToString();
                var saldo = decimal.Parse(ultraGrid1.ActiveRow.Cells["SaldoLetra"].Value.ToString());
                var f = new frmConsultaHistorialPagosCompra(idLetra, TipoBusqueda.Letra, _compraDto);
                f.ShowDialog();
            }

            if (e.Cell.Column.Key == "_VerDocumento")
            {
                if (!Convert.ToBoolean(e.Cell.Row.Cells["EsLetra"].Value.ToString()))
                {
                    var idPago = ultraGrid1.ActiveRow.Cells["IdDocumento"].Value.ToString();
                   // frmPago f = new frmPago("Edicion", idCobranza, null, true);
                    frmPago f = new frmPago("Edicion", idPago, null,true);
                    f.ShowDialog();
                    ObtenerDataCompra(_compraDto.v_IdCompra);
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

      

            if (_objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Today.Date) == "0")
            {
                UltraMessageBox.Show("No se ha registrado ningún tipo de cambio para el día de hoy.", "Sistema",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (_objComprasBL.TienePagoPendiente(_compraDto.v_IdCompra))
            {
                frmPagoRapido frm = new frmPagoRapido(_compraDto.v_IdCompra ); //, _compraDto.i_IdTipoDocumento ?? -1, -1);
                frm.ShowDialog();
                ObtenerDataCompra(_compraDto.v_IdCompra);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRecalcularSaldo_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            //new CobranzaBL().RecalcularSaldoVenta
            new PagoBL ().RecalcularSaldoCompra (ref objOperationResult, _compraDto.v_IdCompra,
                Globals.ClientSession.GetAsList(), false, new DocumentoBL().DocumentoEsInverso(_compraDto.i_IdTipoDocumento ?? 0));

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Saldo Recalculado!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void btnRecalcularSaldosTodosPagos_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult =new OperationResult ();

            new PagoBL().ProcesoRecalcularSaldoTodosPagos(ref  objOperationResult);
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
