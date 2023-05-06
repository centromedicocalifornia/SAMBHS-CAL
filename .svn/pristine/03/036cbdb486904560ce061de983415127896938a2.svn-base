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
using LoadingClass;
using SAMBHS.Windows.WinClient.UI.Procesos;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.Manguifajas
{
    public partial class frmKardexProducto : Form
    {
        string descripcionProducto, idProductoDetalle, pstrCodigoProducto,SoloCodigo,sUnidadMedida;
        int idAlmacen, FormatoCant;
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        decimal Saldo = 0;
        DateTime FechaAl = new DateTime();

        public frmKardexProducto(string CodigoProducto, string descripcionproducto, string idproductodetalle, int idalmacen, int FormatCant, decimal SaldoActual,string UnidadMedida)
        {
            InitializeComponent();
            descripcionProducto = descripcionproducto;
            idProductoDetalle = idproductodetalle;
            idAlmacen = idalmacen;
            FormatoCant = FormatCant;
            Saldo = SaldoActual;
            pstrCodigoProducto = CodigoProducto.Trim() + " - " + descripcionproducto.Trim();
            SoloCodigo = CodigoProducto;
            sUnidadMedida = UnidadMedida;
        }

        private void frmKardexProducto_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            txtBuscarProducto.Text = descripcionProducto.Trim();
            dtpFechaDesdeKardex.Value = DateTime.Now.Month == 1 ? DateTime.Parse("01" + "/10/" + (Globals.ClientSession.i_Periodo - 1).ToString()) : DateTime.Now.Month == 2 ? DateTime.Parse("01" + "/10/" + (Globals.ClientSession.i_Periodo - 1).ToString()) : DateTime.Now.Month == 3 ? DateTime.Parse("01" + "/11/" + (Globals.ClientSession.i_Periodo - 1).ToString()) : DateTime.Parse("01/" + (DateTime.Now.Month - 3).ToString("00") + "/" + DateTime.Now.Year.ToString());
            dtpFechaHastaKardex.Value = DateTime.Now;
            OperationResult objOperationResult = new OperationResult();
            BindGrid();
            CalcularSaldo();
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales, (int)Globals.ClientSession.i_PrecioDecimales);
            txtSaldoActual.Text = Saldo == 0 ? "0.00" : Saldo.ToString("#.00");
            this.StartPosition = FormStartPosition.CenterParent;
            lblStockAl.Text = "Stock al " + dtpFechaDesdeKardex.Value.Date.AddDays(-1).ToShortDateString ()+" : ";
            FechaAl = DateTime.Parse ( dtpFechaDesdeKardex.Value.Date.AddDays(-1).ToShortDateString()+ " 23:59");

          string  FilterExpresion ="IdAlmacen==" + idAlmacen.ToString ();
          var Establecimiento = new MovimientoBL().BuscarIdEstablecimiento(idAlmacen);
          var SaldoAl = new AlmacenBL().ReporteStock(ref objOperationResult, Establecimiento.i_IdEstablecimiento, DateTime.Parse("01/01/" + FechaAl.Year.ToString()), FechaAl, FilterExpresion, 1, SoloCodigo, "", "-1", 0, 0, 0, 0, 1, FormatoCant, "", DateTime.Now, 0);
          txtStockAl.Text = SaldoAl.Any ()? SaldoAl.FirstOrDefault().Saldo_Cantidad.Value.ToString ():"0.00";
          var SaldoInicial =  new AlmacenBL().SaldosIniciales(Globals.ClientSession.i_Periodo.ToString(), SoloCodigo);
          lblSaldoInicialPeriodo.Text = "Stock  Inicial - Periodo " + Globals.ClientSession.i_Periodo.ToString();
           txtStockInicial.Text = SaldoInicial.Any() ? SaldoInicial.FirstOrDefault().Saldo_Cantidad.Value.ToString () : "0.00";

           lblUnidadMedida.Text = sUnidadMedida;

           #region Configura el Sumario de Ingresos
           UltraGridBand band = this.grdData.DisplayLayout.Bands[0];
           SummarySettings IngresosSummary = band.Summaries.Add(
                  "Ingresos",					// Give an identifier (key) for this summary
                  SummaryType.Custom,				// Summary type is custom
                  new IngresosSummary(),		// Our custom summary calculator
                  band.Columns["Ingresos"],		// Column being summarized. Just use Unit Price column.
                  SummaryPosition.Left,			// Position the summary on the left of summary footer
                  null							// Since SummaryPosition is Left, pass in null
                  );

           IngresosSummary.DisplayFormat = "Total Ingresos = {0:######.00}";
           IngresosSummary.Appearance.TextHAlign = HAlign.Right;
           IngresosSummary.SummaryPosition = SummaryPosition.Right;
           IngresosSummary.Appearance.FontData.Bold = DefaultableBoolean.True;
           IngresosSummary.Appearance.ForeColor = Color.Black;
           IngresosSummary.Appearance.BackColor = Color.LightGray;
           #endregion


           #region Configura el Sumario de Salidas

           SummarySettings SalidasSummary = band.Summaries.Add(
                  "Salidas",					// Give an identifier (key) for this summary
                  SummaryType.Custom,				// Summary type is custom
                  new SalidasSummary(),		// Our custom summary calculator
                  band.Columns["Salidas"],		// Column being summarized. Just use Unit Price column.
                  SummaryPosition.Left,			// Position the summary on the left of summary footer
                  null							// Since SummaryPosition is Left, pass in null
                  );

           SalidasSummary.DisplayFormat = "Total Salidas = {0:######.00}";
           SalidasSummary.Appearance.TextHAlign = HAlign.Right;
           SalidasSummary.SummaryPosition = SummaryPosition.Right;
           SalidasSummary.Appearance.FontData.Bold = DefaultableBoolean.True;
           SalidasSummary.Appearance.ForeColor = Color.Black;
           SalidasSummary.Appearance.BackColor = Color.LightGray;
           #endregion


        }





        #region Clase del tipo Sumario Personalizado para Infragistics
        private class IngresosSummary : ICustomSummaryCalculator
        {
            private decimal totalIngresos = 0;

            internal IngresosSummary()
            {
            }
            public void BeginCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {

                // Begins the summary for the SummarySettings object passed in. Implementation of 
                // this method should reset any state variables used for calculating the summary.
                this.totalIngresos = 0;
            }

            public void AggregateCustomSummary(SummarySettings summarySettings, UltraGridRow row)
            {
               
                    
                        if (row.Cells["Ingresos"].Value != null)
                        {

                            totalIngresos = row.Cells["TipoMotivo"].Value != null && row.Cells["TipoMotivo"].Value.ToString() == "5" ? totalIngresos : totalIngresos + decimal.Parse(row.Cells["Ingresos"].Value.ToString());  
                        }
            }

            public object EndCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                // This gets called when the every row has been processed so here is where we
                // would return the calculated summary value.
                return this.totalIngresos;
            }
        }
        private class SalidasSummary : ICustomSummaryCalculator
        {
            private decimal totalSalidas = 0;

            internal SalidasSummary()
            {
            }

            public void BeginCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {

                // Begins the summary for the SummarySettings object passed in. Implementation of 
                // this method should reset any state variables used for calculating the summary.
                this.totalSalidas = 0;

            }

            public void AggregateCustomSummary(SummarySettings summarySettings, UltraGridRow row)
            {

                if (row.Cells["Salidas"].Value != null)
                {
                    totalSalidas = totalSalidas + decimal.Parse(row.Cells["Salidas"].Value.ToString());  // ingreso + decimal.Parse(row.Cells["Ingresos"].Value.ToString());
                }

            }

            public object EndCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                // This gets called when the every row has been processed so here is where we
                // would return the calculated summary value.
                return this.totalSalidas;
            }
        }
    
        #endregion

        private void FormatoDecimalesGrilla(int DecimalesCantidad, int DecimalesPrecio)
        {
            string FormatoCantidad, FormatoPrecio;
            UltraGridColumn _CantidadIngresos = this.grdData.DisplayLayout.Bands[0].Columns["Ingresos"];
            _CantidadIngresos.MaskDataMode = MaskMode.IncludeLiterals;
            _CantidadIngresos.MaskDisplayMode = MaskMode.IncludeLiterals;



            UltraGridColumn _CantidadSalidas = this.grdData.DisplayLayout.Bands[0].Columns["Salidas"];
            _CantidadSalidas.MaskDataMode = MaskMode.IncludeLiterals;
            _CantidadSalidas.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _CantidadSaldo = this.grdData.DisplayLayout.Bands[0].Columns["Saldo"];
            _CantidadSaldo.MaskDataMode = MaskMode.IncludeLiterals;
            _CantidadSaldo.MaskDisplayMode = MaskMode.IncludeLiterals;




            UltraGridColumn _Precio = this.grdData.DisplayLayout.Bands[0].Columns["d_Precio"];
            _Precio.MaskDataMode = MaskMode.IncludeLiterals;
            _Precio.MaskDisplayMode = MaskMode.IncludeLiterals;

            if (DecimalesCantidad > 0)
            {
                string sharp = "n";
                FormatoCantidad = "nnnnnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "nnnnnn";
            }

            if (DecimalesPrecio > 0)
            {
                string sharp = "n";
                FormatoPrecio = "nnnnnn.";
                for (int i = 0; i < DecimalesPrecio; i++)
                {
                    FormatoPrecio = FormatoPrecio + sharp;
                }
            }
            else
            {
                FormatoPrecio = "nnnnnn";
            }

            _CantidadIngresos.MaskInput = "-" + FormatoCantidad;
            _CantidadSalidas.MaskInput = "-" + FormatoCantidad;
            _CantidadSaldo.MaskInput = "-" + FormatoCantidad;
            _Precio.MaskInput = "-" + FormatoPrecio;
        }
        private void CalcularSaldo()
        {
            decimal saldo = 0;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                var fechaemision = DateTime.Parse(Fila.Cells["Fecha"].Value.ToString()).Year;
                if (dtpFechaDesdeKardex.Value.Year != dtpFechaHastaKardex.Value.Year && dtpFechaHastaKardex.Value.Year == fechaemision && Fila.Cells["TipoMotivo"].Value.ToString() == "5")
                {
                    saldo = decimal.Parse(Fila.Cells["IngresosCantidadEmpaque"].Value.ToString());
                }
                else
                {
                    saldo = Fila.Cells["TipoMovimiento"].Value.ToString() == "1" ? saldo + decimal.Parse(Fila.Cells["IngresosCantidadEmpaque"].Value.ToString()) : saldo - decimal.Parse(Fila.Cells["SalidasCantidadEmpaque"].Text);
                }
                Fila.Cells["Saldo"].Value = saldo;
            }
        }
        private void BindGrid()
        {
            OperationResult objOperationResult = new OperationResult();


            var objData = GetData(ref objOperationResult);

            if (objOperationResult.Success == 1)
            {
                grdData.DataSource = objData;
                grdData.Focus();
                lblContadorFilas.Text = string.Format("Se encontraron {0} Movimientos.", objData.Count);
            }
            else
            {

                lblContadorFilas.Text = string.Format("Se encontraron {0} Movimientos.");
            }
        }

        private List<movimientodetalleDto> GetData(ref OperationResult objOperationResult)
        {


            var _objData = _objMovimientoBL.ObtenerDetalleKardexManguifajasFecha(ref objOperationResult, idAlmacen, idProductoDetalle, 2, dtpFechaDesdeKardex.Value, DateTime.Parse(dtpFechaHastaKardex.Text + " 23:59"));
            txtCodigProducto.Text = _objData.Any() ? _objData.FirstOrDefault().CodigoProducto+" "+_objData.FirstOrDefault ().DescripcionProducto  : pstrCodigoProducto;
            return _objData;

        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["EsDevolucion"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            grdData.DisplayLayout.Bands[0].Columns["Icono"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
            grdData.DisplayLayout.Bands[0].Columns["Icono"].DataType = typeof(Image);
            
           // this.grdData.DisplayLayout.Bands[0].Summaries.Clear();
           // SummarySettings  summary = new SummarySettings ();
           // SummarySettings summary2 = new SummarySettings();
            
       
           // summary = this.grdData.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, this.grdData.DisplayLayout.Bands[0].Columns["Ingresos"]);
           // summary.DisplayFormat = "Total :{0} ";
           //summary2 = this.grdData.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, this.grdData.DisplayLayout.Bands[0].Columns["Salidas"]);
           //summary2.DisplayFormat = "Total :{0} ";
        }

        private void frmKardexProducto_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape: this.Close();
                    break;

            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            if (int.Parse(e.Row.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeIngreso && e.Row.Cells["Origen"].Value.ToString() == "T")
            {
                e.Row.Cells["Icono"].Value = Image.FromFile(@"img\refresh.ico");
            }
            else if (int.Parse(e.Row.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeIngreso)
            {
                e.Row.Cells["Icono"].Value = Image.FromFile(@"img\ARROW22C.ico");
            }

            if (int.Parse(e.Row.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeSalida && e.Row.Cells["Origen"].Value.ToString() == "T")
            {
                e.Row.Cells["Icono"].Value = Image.FromFile(@"img\refresh.ico");
            }
            else if (int.Parse(e.Row.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeSalida)
            {
                e.Row.Cells["Icono"].Value = Image.FromFile(@"img\ARROW01F.ico");
            }

            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("0000");
        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string serie, correlativo;
            string[] SerieCorrelativo;

            if (grdData.ActiveRow == null) return;
            switch (e.Cell.Column.Key)
            {
                case "Ver":
                    string seriecorrelativo = grdData.ActiveRow.Cells["v_NumeroDocumento"].Value != null && grdData.ActiveRow.Cells["v_NumeroDocumento"].Value.ToString() != string.Empty ? grdData.ActiveRow.Cells["v_NumeroDocumento"].Value.ToString() : "";
                    if (seriecorrelativo != string.Empty)
                    {
                        SerieCorrelativo = seriecorrelativo.Split(new Char[] { '-' });

                        serie = SerieCorrelativo[0].Trim();
                        correlativo = SerieCorrelativo[1].Trim();

                        if (grdData.ActiveRow.Cells["Origen"].Value != null && grdData.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenCompra)
                        {
                            var IdCompra = _objMovimientoBL.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.Compra, int.Parse(grdData.ActiveRow.Cells["i_TipoDocumento"].Value.ToString()), serie, correlativo);
                            if (IdCompra != null)
                            {
                                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                                frmRegistroCompra frm = new frmRegistroCompra("Edicion", IdCompra, "KARDEX");
                                frm.ShowDialog();
                            }


                        }
                        else if (grdData.ActiveRow.Cells["Origen"].Value != null && grdData.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenVenta)
                        {
                            var IdVenta = _objMovimientoBL.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.Venta, int.Parse(grdData.ActiveRow.Cells["i_TipoDocumento"].Value.ToString()), serie, correlativo);

                            if (IdVenta != null)
                            {

                                frmRegistroVenta frm = new frmRegistroVenta("Edicion", IdVenta, "KARDEX");
                                frm.ShowDialog();
                            }

                        }
                        else if (grdData.ActiveRow.Cells["Origen"].Value != null && grdData.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenImportacion)
                        {
                            var IdVenta = _objMovimientoBL.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.Importacion, int.Parse(grdData.ActiveRow.Cells["i_TipoDocumento"].Value.ToString()), serie, correlativo);

                            if (IdVenta != null)
                            {

                                //  frmRegistroVenta frm = new frmRegistroVenta("Edicion", IdVenta, "KARDEX");
                                frmRegistroImportacion frm = new frmRegistroImportacion("Edicion", IdVenta, "KARDEX");
                                frm.ShowDialog();
                            }


                        }

                        else if (grdData.ActiveRow.Cells["Origen"].Value != null && grdData.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenGuiaInterna)
                        {
                            var IdGuia = _objMovimientoBL.ObtenerIdDiferentesProceso(ref objOperationResult, ListaProcesos.GuiaRemision, 438, serie, correlativo);
                            if (IdGuia != null)
                            {
                                frmGuiaRemision frm = new frmGuiaRemision("Edicion", IdGuia, "KARDEX");
                                frm.ShowDialog();
                            }

                        }
                        else if (grdData.ActiveRow.Cells["Origen"].Value != null && grdData.ActiveRow.Cells["Origen"].Value.ToString() == Constants.OrigenTransferencia)
                        {

                            string MovimientOrigen =grdData.ActiveRow.Cells["v_IdMovimientoOrigen"].Value ==null ?null : grdData.ActiveRow.Cells["v_IdMovimientoOrigen"].Value.ToString ();
                            if (MovimientOrigen!=null )
                            {
                                frmTransferencia frm = new frmTransferencia("Edicion", MovimientOrigen);
                                frm.ShowDialog ();
                            }
                        
                        }
                        else if (grdData.ActiveRow.Cells["Origen"].Value == string.Empty || grdData.ActiveRow.Cells["Origen"].Value.ToString() == string.Empty)
                        {


                            if (grdData.ActiveRow.Cells["v_IdMovimiento"] != null)
                            {

                                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                                if (int.Parse(grdData.ActiveRow.Cells["TipoMovimiento"].Value.ToString()) == (int)TipoDeMovimiento.NotadeIngreso)
                                {

                                    frmNotaIngreso frm = new frmNotaIngreso("Edicion", grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString(), "KARDEX");
                                    frm.ShowDialog();
                                }
                                else
                                {
                                    frmNotaSalida frm = new frmNotaSalida("Edicion", grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString(), "KARDEX");
                                    frm.ShowDialog();
                                }

                            }


                        }
                    }
                    break;

            }



        }

        private void btnBuscarKardexFecha_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            BindGrid();
            CalcularSaldo();
            FechaAl = DateTime.Parse ( dtpFechaDesdeKardex.Value.Date.AddDays(-1).ToShortDateString()+ " 23:59");
            string FilterExpresion = "IdAlmacen==" + idAlmacen.ToString();
            var Establecimiento = new MovimientoBL().BuscarIdEstablecimiento(idAlmacen);
            var SaldoAl = new AlmacenBL().ReporteStock(ref objOperationResult, Establecimiento.i_IdEstablecimiento, DateTime.Parse("01/01/" + FechaAl.Year.ToString()), FechaAl, FilterExpresion, 1, SoloCodigo, "", "-1", 0, 0, 0, 0, 1, FormatoCant, "", DateTime.Now, 0);
            txtStockAl.Text = SaldoAl.Any ()? SaldoAl.FirstOrDefault().Saldo_Cantidad.Value.ToString ():"0.00";
            lblStockAl.Text = "Stock al " + dtpFechaDesdeKardex.Value.Date.AddDays(-1).ToShortDateString() + " : ";
           

        }


    }
}
