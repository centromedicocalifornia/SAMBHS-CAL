using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using CrystalDecisions.Shared;
using SAMBHS.Common.Resource;
using System.Globalization;
using System.IO;
using SAMBHS.Common.BE;
using SAMBHS.Almacen.BL;
using SAMBHS.CommonWIN.BL;
using ZXing;
using ZXing.PDF417;
using ZXing.PDF417.Internal;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoFactura : Form
    {
        #region Declaraciones / Referencias

        string TotalesenLetras, TotalEnNumero;
        List<string> IdVentas;
        bool ImpresionVistaPrevia = true;
        CrystalDecisions.CrystalReports.Engine.ReportDocument rp;
        TiposReportes tipoReporte = new TiposReportes();
        Idioma _Idioma = new Idioma();
        bool InlcuirAlias = false;
        List<ReporteDocumentoFactura> ListaDetalles = new List<ReporteDocumentoFactura>();
        #endregion

        #region Carga de inicializacion

        public frmDocumentoFactura(List<string> _IdVentas, bool IVistaPrevia, TiposReportes _tr = TiposReportes.Factura, Idioma Idi = Idioma.Espaniol,List<ReporteDocumentoFactura> ListaDocFac=null,bool IncAlias=false)
        {

            tipoReporte = _tr;
            InitializeComponent();
            IdVentas = _IdVentas;
            _Idioma = Idi;
            ImpresionVistaPrevia = IVistaPrevia;
            ListaDetalles = ListaDocFac;
            InlcuirAlias = IncAlias;
            ImpresionDirecto();
            
            

        }
        #endregion

        #region Prodecimientos/Funciones
        byte[] GetQrCodeArray(ventaDto obj)
        {
            var objResult = new OperationResult();
            var cliente = new ClienteBL().ObtenerCliente(ref objResult, obj.v_IdCliente);
            if (objResult.Success == 0) return null;
            var listParams = new List<string>
            {
                Globals.ClientSession.v_RucEmpresa, // RUC Emisor
                (obj.i_IdTipoDocumento ?? 1).ToString("00"), // Tipo Comp
                obj.v_SerieDocumento,
                obj.v_CorrelativoDocumento,
                (obj.d_IGV ?? 0).ToString("0.00"),
                (obj.d_Total ?? 0).ToString("0.00"),
                (obj.t_FechaRegistro ?? DateTime.Now).ToShortDateString(),
                (cliente.i_IdTipoIdentificacion ?? 0).ToString("00"),
                cliente.v_NroDocIdentificacion
            };
            var pdf = new PDF417Writer();
            var res = pdf.encode(string.Join("|", listParams) + "|", BarcodeFormat.PDF_417, 300, 50);
            var bmp = new BarcodeWriter
            {
                Options = new PDF417EncodingOptions
                {
                    ErrorCorrection = PDF417ErrorCorrectionLevel.L5
                }
            }.Write(res);
            using (var mem = new MemoryStream())
            {
                bmp.Save(mem, ImageFormat.Jpeg);
                return mem.ToArray();
            }

        }
        private void ImpresionDirecto()
        {
            var objOperationResult = new OperationResult();
            try
            {
                if (IdVentas != null)
                {
                    foreach (var idVenta in IdVentas)
                    {
                        var ruc = Globals.ClientSession.v_RucEmpresa;
                        // rp = ReportesUtils.DevolverReporte(ruc, TiposReportes.Factura);
                        rp = ReportesUtils.DevolverReporte(ruc, tipoReporte);
                        if (rp == null)
                        {
                            UltraMessageBox.Show(
                                "Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ",
                                "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        List<ReporteDocumentoFactura> aptitudeCertificate = new List<ReporteDocumentoFactura>();
                        if (ListaDetalles == null)
                        {
                             aptitudeCertificate = new VentaBL().ReporteDocumentoVenta(ref objOperationResult, idVenta);
                        }
                        else
                        {
                            aptitudeCertificate = ListaDetalles;
                        }
                        var first = aptitudeCertificate.FirstOrDefault();
                        TotalEnNumero = first.Total.ToString("0.00");
                        var moneda = first.Moneda == "S/" ? Currency.Soles : Currency.Dolares;
                        if (ruc == Constants.RucManguifajas || ruc == Constants.RucJc || ruc == Constants.RucJorplast)
                        {
                            if (moneda == Currency.Soles)
                            {

                                TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0
                                    ? Utils.ConvertirenLetras(TotalEnNumero) + " SOLES "
                                    : Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " +
                                      "S.E.U.O.";
                            }
                            if (moneda == Currency.Dolares)
                            {
                                TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0
                                    ? Utils.ConvertirenLetras(TotalEnNumero) +
                                      " DOLARES AMERICANOS "
                                    : Utils.ConvertirenLetras(TotalEnNumero) +
                                      " DOLARES AMERICANOS " + "S.E.U.O.";
                            }
                        }
                        else
                        {

                            if (moneda == Currency.Soles)
                            {

                                if (_Idioma == Idioma.Espaniol)
                                {
                                    TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0
                                        ? "SON :" + Utils.ConvertirenLetras(TotalEnNumero) +
                                          " SOLES "
                                        : "SON :" + Utils.ConvertirenLetras(TotalEnNumero) +
                                          " SOLES " + "S.E.U.O.";
                                }
                                else
                                {
                                    TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0
                                            ? "AMOUNT :" + Utils.DecimalToWords(decimal.Parse(TotalEnNumero)) +
                                              " SOLES "
                                            : "AMOUNT:" + Utils.DecimalToWords(decimal.Parse(TotalEnNumero)) +
                                              " SOLES " + "S.E.U.O.";


                                }
                            }
                            if (moneda == Currency.Dolares)
                            {
                                if (_Idioma == Idioma.Espaniol)
                                {

                                    TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0
                                        ? "SON :" + Utils.ConvertirenLetras(TotalEnNumero) +
                                          " DOLARES AMERICANOS "
                                        : "SON :" + Utils.ConvertirenLetras(TotalEnNumero) +
                                          " DOLARES AMERICANOS " + "S.E.U.O.";
                                }
                                else
                                {
                                    TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0
                                           ? "AMOUNT:" + Utils.DecimalToWords(decimal.Parse(TotalEnNumero)) +
                                             " DOLARES AMERICANOS "
                                           : "AMOUNT:" + Utils.DecimalToWords(decimal.Parse(TotalEnNumero)) +
                                             " DOLARES AMERICANOS " + "S.E.U.O.";
                                }
                            }

                        }

                        var DatosAlmacen = new AlmacenBL().ObtenerDatosEmpresa(Globals.ClientSession.i_IdAlmacenPredeterminado.Value, Globals.ClientSession.i_IdEstablecimiento.Value);
                        string DatosEmpresa = DatosAlmacen.Count() > 0 ?  DatosAlmacen[0] : "";
                        string TelefonoEmpresa = DatosAlmacen.Count() > 0 ? DatosAlmacen[1] : "";
                       
                        var vta = new VentaBL().ObtenerVentaCabecera(ref objOperationResult, idVenta);
                        var qr = GetQrCodeArray(vta);
                        var config = new ConfiguracionFacturacionBL().GetConfiguracion(out objOperationResult);
                        aptitudeCertificate.ForEach(v => { v.CodigoQR = qr;
                            v.Descuento = vta.d_Descuento ?? 0;
                        });
                        using (var ds1 = new DataSet())
                        {
                            List<ReporteDocumentoFactura> ServicioFlete = new List<ReporteDocumentoFactura>();
                            List<ReporteDocumentoFactura> ServicioSeguro = new List<ReporteDocumentoFactura>();
                            string AnexoDetalle = "";
                            List<ReporteDocumentoFactura> Reporte = new List<ReporteDocumentoFactura>();
                            if (Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic || Globals.ClientSession.v_RucEmpresa == Constants.RucDemo)
                            {

                                Reporte = aptitudeCertificate;
                                aptitudeCertificate = Reporte.Where(o => o.FormaParteOtrosTributos == 0).ToList();
                                ServicioFlete = Reporte.Where(o => o.FormaParteOtrosTributos == 1 && o.Descripcion.ToUpper().Contains("FLETE")).ToList();
                                ServicioSeguro = Reporte.Where(o => o.FormaParteOtrosTributos == 1 && o.Descripcion.ToUpper().Contains("SEGURO")).ToList();
                                var anexdetalle = Reporte.Where(o => !string.IsNullOrEmpty(o.ObservacionDetalle)).FirstOrDefault();
                                AnexoDetalle = anexdetalle != null ? anexdetalle.ObservacionDetalle : "";

                            }
                            using (var dt = Utils.ConvertToDatatable(aptitudeCertificate))
                            {


                                dt.TableName = "dsDocumentoFactura";
                                ds1.Tables.Add(dt);
                                rp.SetDataSource(ds1);
                                rp.SetParameterValue("TotalLetras", TotalesenLetras);
                                rp.SetParameterValue("CantidadDecimal", Globals.ClientSession.i_CantidadDecimales ?? 2);
                                rp.SetParameterValue("CantidadDeciamlPrecio",
                                    Globals.ClientSession.i_PrecioDecimales ?? 2);

                                var fieldsName = new List<string>();
                                foreach (ParameterField item in rp.ParameterFields)
                                    fieldsName.Add(item.Name);
                                if (fieldsName.Contains("Mes"))
                                    rp.SetParameterValue("Mes",
                                        new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(
                                            int.Parse(first.FechaRegistro.Month.ToString("00"))).ToUpper());

                                if (fieldsName.Contains("TelefonoEmpresa"))
                                    rp.SetParameterValue("TelefonoEmpresa",
                                        TelefonoEmpresa);
                                if (fieldsName.Contains("DireccionEmpresa"))
                                    rp.SetParameterValue("DireccionEmpresa",
                                       DatosEmpresa);
                               

                                    if (fieldsName.Contains("ServicioFlete"))

                                        if (Reporte.Any() && (Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cfr || Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif))
                                        {
                                            rp.SetParameterValue("ServicioFlete", ServicioFlete == null ? "" : "FLETE : ");
                                        }
                                        else
                                        {
                                            rp.SetParameterValue("ServicioFlete", "");
                                        }
                                    if (fieldsName.Contains("TotalFlete"))

                                        if (Reporte.Any() && (Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cfr || Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif))
                                        {
                                            rp.SetParameterValue("TotalFlete", !ServicioFlete.Any() ? 0 : ServicioFlete.Sum(o => o.PrecioVenta));
                                        }
                                        else
                                        {
                                            rp.SetParameterValue("TotalFlete", 0);
                                        }
                               
                                
                                    if (fieldsName.Contains("ServicioSeguro"))
                                        if (Reporte.Any() && Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif)
                                        {
                                            rp.SetParameterValue("ServicioSeguro", ServicioSeguro == null ? "" : "SEGURO :");
                                        }
                                        else
                                        {
                                            rp.SetParameterValue("ServicioSeguro", "");
                                        }

                                    if (fieldsName.Contains("TotalSeguro"))
                                        if (Reporte.Any() && Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif)
                                        {
                                            rp.SetParameterValue("TotalSeguro", !ServicioSeguro.Any() ? 0 : ServicioSeguro.Sum(o => o.PrecioVenta));
                                        }
                                        else
                                        {
                                            rp.SetParameterValue("TotalSeguro", 0 );
                                        }

                                if (fieldsName.Contains("Idioma"))
                                    rp.SetParameterValue("Idioma", _Idioma);

                                if (fieldsName.Contains("Anexo"))
                                    rp.SetParameterValue("Anexo", AnexoDetalle);

                                if (fieldsName.Contains("IncluirAlias"))
                                    rp.SetParameterValue("IncluirAlias", InlcuirAlias);
                                rp.SetParameterValue("_ResolucionIntendencia", config.v_Resolucion != null ? config.v_Resolucion : "-");
                                try
                                {
                                    if (!ImpresionVistaPrevia)
                                    {
                                        rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                                        var impresora =
                                            Globals.ListaEstablecimientoDetalle.Where(
                                                x =>
                                                    x.i_IdTipoDocumento == (int)TiposDocumentos.Factura &&
                                                    !string.IsNullOrEmpty(x.v_NombreImpresora) &&
                                                    x.v_Serie.Trim() == first.SerieDocumento).ToArray();

                                        if (impresora.Length > 0)
                                        {
                                            //  rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                                            try
                                            {
                                                var nombreImpresora =
                                                    impresora[0].v_NombreImpresora.Trim();
                                                rp.PrintOptions.PrinterName = nombreImpresora;
                                                rp.PrintToPrinter(1, false, 1, 1);
                                            }
                                            catch (Exception)
                                            {
                                                crystalReportViewer1.ReportSource = rp;
                                                crystalReportViewer1.Show();
                                            }
                                        }
                                        else
                                        {
                                            rp.PrintToPrinter(1, false, 1, 1);
                                        }

                                    }
                                }
                                catch
                                {
                                    UltraMessageBox.Show("El nombre de impresora no existe ", "Sistema",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                    return;
                                }
                                finally
                                {
                                    crystalReportViewer1.ReportSource = rp;
                                    crystalReportViewer1.Show();
                                }
                            }
                        }

                    }

                    //this.Close();
                }
                else
                {
                    UltraMessageBox.Show("El documento no se puede imprimir ", "Error de Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Se produjo un error  al imprimir documento", "Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "frmDocumentoFactura.\nLinea:" +
                                                           ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
            }
        }

        #endregion

        private void frmDocumentoFactura_Load(object sender, EventArgs e)
        {

        }

    }

    public class DocFactura
    {
        protected string Family = "Arial";
        public bool Success { get; set; }

        #region Ticket

        private Graphics _graph;
        private Font _font;
        private readonly Brush _brush = Brushes.Black;
        private float x;
        private float x2;
        private readonly IEnumerable<string> _idVentas;
        private List<ReporteDocumentoFactura> _rep;
        #endregion

        #region Construct

        public DocFactura(IEnumerable<string> idVentas)
        {
            _idVentas = idVentas;
        }

        #endregion

        #region Methods

        public void Print()
        {
            var objOperationResult = new OperationResult();
            foreach (var idVenta in _idVentas)
            {
                _rep = new VentaBL().ReporteDocumentoVenta(ref objOperationResult, idVenta);
                using (var pdoc = new PrintDocument())
                {
                    pdoc.PrintPage += pdoc_PrintPage;

                    var f = _rep.First();
                    var serie = f.Documento.Split('-')[0].Trim();
                    var config = (from r in Globals.ListaEstablecimientoDetalle
                                  where r.i_IdTipoDocumento == (int)TiposDocumentos.Factura && !string.IsNullOrEmpty(r.v_NombreImpresora) && r.v_Serie.Trim() == serie
                                  select new Tuple<string, int>(r.v_NombreImpresora, r.i_ImpresionVistaPrevia ?? 0)
                    ).FirstOrDefault();
                    try
                    {
                        if (config != null)
                        {
                            pdoc.PrinterSettings.PrinterName = config.Item1;
                            pdoc.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("FACTURA v1", 850, 550);

                            if (config.Item2 == 1)
                                using (var pp = new PrintPreviewDialog { Document = pdoc })
                                    if (pp.ShowDialog() != DialogResult.OK) return;
                        }
                        else
                            if (UltraMessageBox.Show("No se encontro configuración de impresión.\n¿Desea usar Impresora Predeterminada?",
                                    Botones: MessageBoxButtons.YesNo, Icono: MessageBoxIcon.Question) == DialogResult.No)
                                return;
                        pdoc.Print();
                        Success = true;
                    }
                    catch (Exception er)
                    {
                        UltraMessageBox.Show(er.Message, "Error en Impresion", Icono: MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            _graph = e.Graphics;
            float width = e.PageBounds.Width;
            x = 0;
            x2 = width - x;
            _font = new Font(Family, 8, FontStyle.Regular);
            var header = _rep.First();
            Text(header.FechaRegistro.ToString("d"), new Point(50, 115));
            Text(DateTime.Now.ToShortTimeString(), new Point(110, 115));
            Text(header.NombreCliente, new Point(50, 130));
            Text(header.Direccion, new Point(60, 150));
            Text(header.NroDocCliente, new Point(50, 165));
            Text(header.CondicionPago, new Point(350, 115));
            Text(header.Vendedor, new Point(370, 165));
            Text(header.Documento, new Point(650, 135));
            _font.Dispose();
            _font = new Font(Family, 8, FontStyle.Regular);
            var init = 205;
            var formatCant = "0." + new string('0', Globals.ClientSession.i_CantidadDecimales ?? 2);
            var formatPrecio = "0." + new string('0', Globals.ClientSession.i_PrecioDecimales ?? 2);
            foreach (var item in _rep)
            {
                if (item.CodigoArticulo.All(r => r == '0')) continue;
                Text(item.CodigoArticulo, new Point(5, init));
                Text(item.Cantidad.ToString(formatCant), new Point(90, init), StringAlignment.Far, width: 45);
                Text(item.Unidad, new Point(150, init));
                Text(item.Descripcion, new Point(215, init), width: 380);
                Text(item.d_PrecioImpresion.ToString(formatPrecio), new Point(600, init), StringAlignment.Far, width: 100);
                var preV = item.d_PrecioImpresion * item.Cantidad;
                Text(preV.ToString("0.00"), new Point(710, init), StringAlignment.Far, width: 75);
                init += 16;
            }
            var parameters = GetParameter();
            Text(GetTotalLetras(header.Moneda == "S/" ? Currency.Soles : Currency.Dolares, header.Total.ToString("0.00")), new Point(120, 420));
            Text(parameters["subTotal"], new Point(290, 475));
            Text(parameters["descuento"], new Point(470, 475));
            Text(parameters["igv"], new Point(600, 475));
            Text(header.NombreEmpresaPropietaria, new Point(700, 475));
            Text(parameters["Total"], new Point(720, 475), StringAlignment.Far, width: 75);
        }

        private string GetTotalLetras(Currency moneda, string total)
        {
            var result = "SON :" + Utils.ConvertirenLetras(total) + "  " + (moneda == Currency.Soles ? "SOLES" : "DOLARES AMERICANOS");
            if (Globals.ClientSession.i_IncluirSEUOImpresionDocumentos != 0)
                result += " S.E.U.O.";

            return result;
        }

        private void Text(string texto, Point position, StringAlignment align = StringAlignment.Near, float height = 15, int width = 0)
        {
            if (width == 0) x = x2 - position.X;
            var rect = new RectangleF(position.X, position.Y, width, height);
            using (var format = new StringFormat())
            {
                format.Alignment = align;
                format.FormatFlags = StringFormatFlags.NoWrap;
                format.Trimming = StringTrimming.None;
                _graph.DrawString(texto, _font, _brush, rect, format);
            }
        }

        private Dictionary<string, string> GetParameter()
        {
            var head = _rep.First();
            var sumPrecioImprecion = _rep.Sum(a => Utils.Windows.DevuelveValorRedondeado(a.d_PrecioImpresion * a.Cantidad, 2));
            var sumPrecioVenta = _rep.Sum(a => a.PrecioVenta);
            var sbT = sumPrecioImprecion / (head.valorigv / 100.0M + 1.0M);
            var subTotal = Utils.Windows.DevuelveValorRedondeado(sbT, 2);
            var descuento = Math.Abs((sumPrecioVenta - sumPrecioImprecion) / (head.valorigv / 100.0M + 1.0M));
            var stCalculo = sbT - descuento;
            var igv = Math.Round(stCalculo * (head.valorigv / 100.0M), 2, MidpointRounding.AwayFromZero);
            var r = new Dictionary<string, string>
            {
                {"sumPrecioImpresion", sumPrecioImprecion.ToString("0.00")},
                {"sumPrecioVenta", sumPrecioVenta.ToString("0.00")},
                {"subTotal", subTotal.ToString("0.00")},
                {"descuento", descuento.ToString("0.00")},
                {"subTotalCalculo", stCalculo.ToString("0.00")},
                {"igv", igv.ToString("0.00")},
                {"Total", (stCalculo + igv).ToString("0.00")}
            };
            return r;
        }
        #endregion

    }
}
