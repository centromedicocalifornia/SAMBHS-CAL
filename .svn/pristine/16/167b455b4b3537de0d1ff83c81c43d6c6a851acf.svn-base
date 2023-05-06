using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using CrystalDecisions.Shared;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
using System.Globalization;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using ZXing;
using ZXing.PDF417;
using ZXing.PDF417.Internal;
using ImageFormat = System.Drawing.Imaging.ImageFormat;


namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoBoleta : Form
    {
        #region Fields
        private readonly List<string> _idVentas;
        private readonly bool _impresionVistaPrevia;
        CrystalDecisions.CrystalReports.Engine.ReportDocument rp;
        
        #endregion

        #region Constructor
        public frmDocumentoBoleta(List<string> idVenta, bool vistaPrevia)
        {
            InitializeComponent();
            _idVentas = idVenta;
            _impresionVistaPrevia = vistaPrevia;
            ImpresionDirecto();
            FormClosing += frmDocumentoBoleta_FormClosing;
        }

        void frmDocumentoBoleta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rp != null)
                rp.Dispose();
        }
        #endregion

        #region Method
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
                //this.Visible = false;   
               
                if (_idVentas != null)
                {
                    foreach (var idVenta in _idVentas)
                    {
                        var empresa1 = new NodeBL().ReporteEmpresa().FirstOrDefault();
                        if (empresa1 == null) continue;
                        var ruc = empresa1.RucEmpresaPropietaria;
                        var vta = new VentaBL().ObtenerVentaCabecera(ref objOperationResult, idVenta);
                        rp = vta.i_IdTipoDocumento.Value == 3 ? ReportesUtils.DevolverReporte(ruc, TiposReportes.Boleta) : ReportesUtils.DevolverReporte(ruc, TiposReportes.NotaIngreso);
                        var qr = GetQrCodeArray(vta);
                        var config = new ConfiguracionFacturacionBL().GetConfiguracion(out objOperationResult);
                        if (rp == null)
                        {
                            UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var aptitudeCertificate = new VentaBL().ReporteDocumentoVenta(ref objOperationResult,idVenta);
                        aptitudeCertificate.ForEach(v => v.CodigoQR = qr);

                        var firstaptitude = aptitudeCertificate.FirstOrDefault();
                        if(firstaptitude == null) continue;
                        var totalEnNumero = firstaptitude.Total.ToString("0.00");

                        var totalesenLetras = "";
                        if (firstaptitude.Moneda == "S/")
                            totalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos ==0 ? "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " SOLES " : "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " SOLES " + "S.E.U.O.";
                        else if (firstaptitude.Moneda == "US$")
                            totalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " DOLARES AMERICANOS " : "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " DOLARES AMERICANOS " + "S.E.U.O.";
                        using (var ds1 = new DataSet())
                        {
                            using (var dt = Utils.ConvertToDatatable(aptitudeCertificate))
                            {
                                dt.TableName = "dsDocumentoBoleta";

                                ds1.Tables.Add(dt);
                                rp.SetDataSource(ds1);
                                rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
                                rp.SetParameterValue("CantidadDeciamlPrecio", (int)Globals.ClientSession.i_PrecioDecimales);
                                rp.SetParameterValue("Dia", firstaptitude.FechaRegistro.Day.ToString("00"));
                                rp.SetParameterValue("Mes", new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(firstaptitude.FechaRegistro.Month.ToString("00"))).ToUpper());  // TextBox con el valor del Parametro);
                                rp.SetParameterValue("Anio", ruc == Constants.RucRollavel | ruc == Constants.RucJorplast ? firstaptitude.FechaRegistro.Year.ToString().Substring(2, 2) : firstaptitude.FechaRegistro.Year.ToString().Substring(3, 1));
                                rp.SetParameterValue("TotalesenLetras", totalesenLetras);
                                rp.SetParameterValue("_ResolucionIntendencia", config.v_Resolucion != null ? config.v_Resolucion : "-");

                                var fieldsName = (from ParameterField item in rp.ParameterFields select item.Name).ToList();

                                if (fieldsName.Contains("MesNum"))
                                    rp.SetParameterValue("MesNum", firstaptitude.FechaRegistro.Month.ToString("00"));
                                if (fieldsName.Contains("YearFull"))
                                    rp.SetParameterValue("YearFull", firstaptitude.FechaRegistro.Year.ToString());
                                try
                                {
                                    if (!_impresionVistaPrevia)
                                    {

                                        var impresora =
                                            Globals.ListaEstablecimientoDetalle.Where(
                                                x =>
                                                    x.i_IdTipoDocumento == (int)TiposDocumentos.BoletaVenta &&
                                                    !string.IsNullOrEmpty(x.v_NombreImpresora)
                                                    && x.v_Serie.Trim() == firstaptitude.SerieDocumento.Trim()).ToArray();
                                        if (impresora.Length > 0)
                                        {
                                            var nombreImpresora = impresora[0].v_NombreImpresora.Trim();
                                            rp.PrintOptions.PrinterName = nombreImpresora;
                                            try
                                            {
                                                rp.PrintToPrinter(1, false, 1, 1);
                                            }
                                            catch
                                            {
                                                // ignored
                                            }
                                        }
                                        else
                                        {
                                            // rp.PrintOptions.PrinterName = "Boletas_tisn";
                                            rp.PrintToPrinter(1, false, 1, 1);
                                        }

                                    }

                                }
                                catch
                                {
                                    UltraMessageBox.Show("El nombre de impresora no existe ", "SISTEMA", MessageBoxButtons.OK,
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
                        //rp.PrintOptions.PrinterName = @"\\192.168.1.41\Boleta";

                        //----------

                        //CrystalDecisions.ReportAppServer.ClientDoc.ISCDReportClientDocument rptClientDoc;
                        //rptClientDoc = rp.ReportClientDocument;
                        //System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
                        //pDoc.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("PapelBoleta", 210, 117);
                        //CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions rasPROpts = new CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions();
                        //CrystalDecisions.ReportAppServer.ReportDefModel.PrintOptions MYPRTOpts = new CrystalDecisions.ReportAppServer.ReportDefModel.PrintOptions();
                        //MYPRTOpts.PrinterName = "Facturas";
                        //MYPRTOpts.DissociatePageSizeAndPrinterPaperSize = true;
                        //MYPRTOpts.PaperSize = (CrPaperSizeEnum)pDoc.DefaultPageSettings.PaperSize.Width;
                        //MYPRTOpts.PaperSize = (CrPaperSizeEnum)pDoc.DefaultPageSettings.PaperSize.Height;
                        //MYPRTOpts.PaperOrientation = CrPaperOrientationEnum.crPaperOrientationPortrait;
                        //MYPRTOpts.PaperSource = CrPaperSourceEnum.crPaperSourceTractor;
                        //rptClientDoc.PrintOutputController.ModifyPrintOptions(MYPRTOpts);
                        //rptClientDoc.PrintOutputController.PrintReport(rasPROpts);
                        //---------
                        //rp.PrintToPrinter(1, false, 1, 1);
                    }

                    //this.Close();
                }
                else
                {
                    UltraMessageBox.Show("El documento no se puede imprimir por falta de datos  ", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Se produjo un error  al imprimir  documento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "frmDocumentoBoleta.\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
            }
        }     
        #endregion

        private void frmDocumentoBoleta_Load(object sender, EventArgs e)
        {

        }

       
    }

    public class DocBoleta
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

        public DocBoleta(IEnumerable<string> idVentas)
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
                                  where r.i_IdTipoDocumento == (int)TiposDocumentos.BoletaVenta && !string.IsNullOrEmpty(r.v_NombreImpresora) && r.v_Serie.Trim() == serie
                                  select new Tuple<string, int>(r.v_NombreImpresora, r.i_ImpresionVistaPrevia ?? 0)
                    ).FirstOrDefault();
                    try
                    {
                        if (config != null)
                        {
                            var sizes = pdoc.PrinterSettings.PaperSizes;

                            pdoc.PrinterSettings.PrinterName = config.Item1;
                            pdoc.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("BOLETA v1", 850, 397); //396

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            _graph = e.Graphics;
            float width = e.PageBounds.Width;
            x = 0;
            x2 = width - x;
            _font = new Font(Family, 8, FontStyle.Regular);
            var header = _rep.First();
            Text(header.FechaRegistro.ToString("d"), new Point(50, 90));
            Text(DateTime.Now.ToShortTimeString(), new Point(110, 90));
            Text(header.NombreCliente, new Point(50, 105));
            Text(header.Direccion, new Point(50, 120));

            Text(header.NroDocCliente, new Point(200, 90));
            Text(header.Vendedor, new Point(340, 105));
            Text(header.CondicionPago, new Point(490, 120));
            Text(header.Documento, new Point(700, 120));
            _font.Dispose();
            _font = new Font(Family, 8, FontStyle.Regular);
            var init = 160;
            var formatCant = "0." + new string('0', Globals.ClientSession.i_CantidadDecimales ?? 2);
            var formatPrecio = "0." + new string('0', Globals.ClientSession.i_PrecioDecimales ?? 2);
            foreach (var item in _rep)
            {
                if (item.CodigoArticulo.All(r => r == '0')) continue;
                Text(item.CodigoArticulo, new Point(5, init));
                Text(item.Cantidad.ToString(formatCant), new Point(100, init),StringAlignment.Far, width: 35);
                Text(item.Unidad, new Point(140, init));
                Text(item.Descripcion, new Point(210, init), width : 390);
                Text(item.d_PrecioImpresion.ToString(formatPrecio), new Point(600, init), StringAlignment.Far, width: 110);
                var preV = item.d_PrecioImpresion * item.Cantidad;
                Text(preV.ToString("0.00"), new Point(730, init), StringAlignment.Far, width: 65);
                init += 20;
            }
            var parameters = GetParameter();
            int inf = 340;
            Text(parameters["subTotal"], new Point(460, inf));
            Text(parameters["descuento"], new Point(610, inf));
            Text(header.NombreEmpresaPropietaria, new Point(725, inf));
            Text(header.Total.ToString("0.00"), new Point(730, inf), StringAlignment.Far, width: 65);
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
            var sumPrecioImprecion = _rep.Sum(a => Math.Round(a.d_PrecioImpresion * a.Cantidad, 2, MidpointRounding.AwayFromZero));
            var descuento = Math.Abs(sumPrecioImprecion - head.Total);
            var r = new Dictionary<string, string>
            {
                {"subTotal", sumPrecioImprecion.ToString("0.00")},
                {"descuento", descuento.ToString("0.00")},
            };
            return r;
        }
        #endregion

    }

}
