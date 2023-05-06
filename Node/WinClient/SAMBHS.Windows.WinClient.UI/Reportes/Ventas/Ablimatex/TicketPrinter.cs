using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Par = System.Collections.Generic.KeyValuePair<float, string>;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas.Ablimatex
{
    public abstract class TicketPrinter
    {
        #region Fields
        protected string Family = "Verdana";

        #region Ticket
        protected Graphics graph;
        protected Font font;
        protected float HeightLetter;
        private Brush brush;
        protected float currentY;
        protected float x;
        protected float x2;
        #endregion
        #endregion

        #region Init
        protected TicketPrinter()
        {
            brush = Brushes.Black;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Prints Document.
        /// </summary>
        public abstract void Print();

        protected void SetFooter(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return;
            using (var reader = new StringReader(texto))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    if (str == string.Empty) currentY += HeightLetter;
                    else Justified(str);
                }
            }
        }

        protected void SetRelative(float w, params KeyValuePair<float, string>[] args)
        {
            var xt = x;
            foreach (var arg in args)
            {
                x = arg.Key * w;
                SetLine(arg.Value, args.Last().Equals(arg)? StringAlignment.Far : StringAlignment.Near);
                currentY -= HeightLetter;
            }
            currentY += HeightLetter;
            x = xt;
        }

        protected void SetForPercent(float w, params KeyValuePair<float, string>[] args)
        {
            var xy = x2;
            var xt = x;
            x2 = x;
            foreach (var arg in args)
            {
                var l = arg.Key * w;
                x2 += l;
                SetLine(arg.Value, args.Last().Equals(arg) ? StringAlignment.Far : StringAlignment.Near);
                x += l;
                currentY -= HeightLetter;
            }
            currentY += HeightLetter;
            x = xt;
            x2 = xy;
        }

        protected void SetLine(string text, StringAlignment align = StringAlignment.Near)
        {
            var rect = new RectangleF(x, currentY, x2 - x, HeightLetter);
            using (var format = new StringFormat())
            {
                format.Alignment = align;
                format.FormatFlags = StringFormatFlags.NoWrap;
                format.Trimming = StringTrimming.None;
                graph.DrawString(text, font, brush, rect, format);
                currentY += HeightLetter;
            }
        }

        protected void SetText(string text)
        {
            var widht = x2 - x;
            var h = graph.MeasureString(text, font, (int)widht).Height;
            var rect = new RectangleF(x, currentY, widht, h);
            using (var format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                graph.DrawString(text, font, brush, rect, format);
                currentY += h;
            }
        }

        protected void Justified(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return;
            var w = x2 - x;
            var words = texto.Split(' ');
            float acum = 0;
            int i = 0;
            var lines = new Dictionary<int, Tuple<string, float>>();
            for (; i < words.Length; i++)
            {
                var wl = graph.MeasureString(words[i], font).Width;
                acum += wl;
                if (acum > w)
                    break;
                lines.Add(i, new Tuple<string, float>(words[i], wl));
            }
            var xr = x;
            var extraSpace = w - lines.Sum(r => r.Value.Item2);
            var numSpaces = lines.Count - 1;
            if (lines.Count > 1) extraSpace /= numSpaces;
            foreach (var item in lines)
            {
                graph.DrawString(item.Value.Item1, font, brush, xr, currentY);
                xr += item.Value.Item2 + extraSpace;
            }
            currentY += HeightLetter;
            Justified(string.Join(" ", words.Skip(i)));
        }

        protected void SetFont(Font f, Graphics g)
        {
            font = f;
            HeightLetter = f.GetHeight(g);
        }
        #endregion
    }

    /// <summary>
    /// Class TicketPrinter.
    /// </summary>
    public class Ticket : TicketPrinter
    {
        #region Fields
        private List<ReporteDocumentoTicket> _repTicket;
        private readonly IEnumerable<string> _idVentas;
        private ReporteAlmacen _repAlmacen;
        #endregion

        #region Init
        public Ticket(IEnumerable<string> idVentas) 
        {
            _idVentas = idVentas;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prints Documents.
        /// </summary>
        public override void Print()
        {
            if (_idVentas == null || !_idVentas.Any()) return;
            var aptitudeCertificat3 = new AlmacenBL().ReporteAlmacen(Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1);
            _repAlmacen = aptitudeCertificat3.FirstOrDefault();
            foreach (var id in _idVentas)
            {
                _repTicket = new Venta.BL.VentaBL().ReporteDocumentoTicket(id);
                if(_repTicket == null || _repTicket.Count == 0) continue;
                using (var pdoc = new PrintDocument())
                {
                    //var psize = new PaperSize("Custom", 250, 800);
                    //pdoc.DefaultPageSettings.PaperSize = psize;
                    //pdoc.DefaultPageSettings.PaperSize.Height += 2 * repTicket.Count * (int)(new Font(family, 10).GetHeight());
                    switch (Globals.ClientSession.v_RucEmpresa)
                    {
                        case Constants.RucHormiguita:
                            pdoc.PrintPage += pdoc_PrintPage;
                            break;
                        case Constants.RucChayna:
                            pdoc.PrintPage += pdoc_PrintPageChayna;
                            break;
                        case Constants.RucJulioEduardoRojasDavila:
                            pdoc.PrintPage += pdoc_PrintPageJulioEduardo;
                            break;
                        case Constants.RucNotariaBecerrSosaya:
                            pdoc.PrintPage += pdoc_PrintNotaria;
                            break;
                        default:
                            pdoc.PrintPage += pdoc_PrintPage;
                            break;
                    }
                    var f = _repTicket.First();
                    var serie = f.Documento.Split('-')[0].Trim();
                    Tuple<string, int> config = (from r in Globals.ListaEstablecimientoDetalle
                                                 where r.i_IdTipoDocumento == f.TipoDoc && !string.IsNullOrEmpty(r.v_NombreImpresora) && r.v_Serie.Trim() == serie
                        select new Tuple<string, int>(r.v_NombreImpresora, r.i_ImpresionVistaPrevia ?? 0)
                    ).FirstOrDefault();
                    try
                    {
                        if (config != null)
                        {
                            pdoc.PrinterSettings.PrinterName = config.Item1;
                            if (config.Item2 == 1)
                            {
                                //PrintDialog pd = new PrintDialog();
                                //var psize = new System.Drawing.Printing.PaperSize("Custom", 100, 200);
                                //pd.Document = pdoc;
                                //pd.Document.DefaultPageSettings.PaperSize = psize;
                                using (var pp = new PrintPreviewDialog {Document = pdoc})
                                {
                                    if (pp.ShowDialog() != DialogResult.OK) return;
                                }
                            }
                        }
                        else
                        {
                            if (
                                UltraMessageBox.Show(
                                    "NO SE ENCONTRO CONFIGURACIÓN DE IMPRESION,¿Desea usar Impresora Predeterminada?",
                                    Botones: MessageBoxButtons.YesNo, Icono: MessageBoxIcon.Question) == DialogResult.No)
                                return;
                        }
                        pdoc.Print();
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
            graph = e.Graphics;
            currentY = e.PageBounds.Top;
            const float width = 250.0f;
            //x = e.PageBounds.Width * 0.02f;
            //x2 = e.PageBounds.Width - x;

            x = 0;
            x2 = width - x;
            var header = _repTicket.First();

            SetFont(new Font(Family, 9, FontStyle.Bold), graph);

            using (var reader = new StringReader(_repAlmacen.v_NombreComercial))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    if (str == string.Empty) currentY += HeightLetter;
                    else SetLine(str, StringAlignment.Center);
                }
            }
            SetFont(new Font(Family, 8), graph);
            currentY += 5;
            SetLine("RUC: " + _repAlmacen.v_NumeroSerieTicket);
            currentY += HeightLetter;
            Justified(_repAlmacen.v_Direccion);
            currentY += 10;
            SetLine("NRO SERIE");
            SetLine("NRO TICKET");
            SetLine("EMITIDO");
            currentY -= 3 * HeightLetter;
            x = width * 0.40f;
            SetLine(":" + UserConfig.Default.NroTicket);
            SetLine(":" + header.Documento);
            SetLine(":" + header.FechaRegistro.ToString("dd/MM/yyyy"));
            x = 0;
            currentY += 8;
            SetLine("CODIGO               CANT.      P.U.      IMP");
            currentY -= 5;
            SetLine(new string('-', 55));
            var sumCant = 0M;
            foreach (var item in _repTicket)
            {
                if (item.CodigoArticulo.All(r => r == '0')) continue;
                SetLine(item.Descripcion);
                SetLine(string.Concat(
                        item.CodigoArticulo.PadRight(16),
                        item.Cantidad.ToString("0.00").PadLeft(6),
                        item.Precio.ToString("0.00").PadLeft(11),
                        item.PrecioVenta.ToString("0.00").PadLeft(9)));
                sumCant += item.Cantidad;
            }
            SetLine(new string('-', 55));
            if (header.NroDocCliente.Length == 11)
            {
                SetLine("TOTAL PRENDAS: " + sumCant.ToString("0.00").PadLeft(11), StringAlignment.Far);
                SetLine("SUB TOTAL:        " + header.ValorVenta.ToString("0.00").PadLeft(11), StringAlignment.Far);
                SetLine("IGV:                   " + header.Igv.ToString("0.00").PadLeft(11), StringAlignment.Far);
                SetLine("TOTAL:               " + header.Total.ToString("0.00").PadLeft(11), StringAlignment.Far);
            }
            else
                SetLine("TOTAL PRENDAS" + sumCant.ToString("0.00").PadLeft(8) + "   TOTAL:" + header.Total.ToString("0.00").PadLeft(9));
            currentY += HeightLetter;
            //SetLine(DateTime.Now.ToString("dd/MM/yyyy"));
            SetLine("PAGO");
            SetLine("VENDEDOR");
            SetLine("CLIENTE");
            if (header.NroDocCliente.Length == 11)
            {
                SetLine("RUC");
                SetLine("DIRECCION");
                currentY -= 2 * HeightLetter;
            }
            currentY -= 3 * HeightLetter;
            x = width * 0.4f;
            //SetLine(DateTime.Now.ToString("h:mm tt"));
            SetLine(header.FormaPago);
            SetLine(header.Vendedor);
            SetLine(header.NombreCliente);
            if (header.NroDocCliente.Length == 11)
            {
                SetLine(header.NroDocCliente);
                SetLine(header.Direccion);
            }
            x = 0;
            currentY += 2 * HeightLetter;
            SetFont(new Font(Family, 7), graph);
            SetLine("ESTIMADO CLIENTE");
            currentY += HeightLetter;
            SetFooter(Globals.ClientSession.GlosaTicket);
            font.Dispose();
            //SetText("TODO CAMBIO REALIZADO SE REALIZA HASTA 7 DIAS DESPUES DE REALIZADA LA COMPRA, PRESENTANDO TICKET ORIGINAL Y DOCUMENTO DE IDENTIDAD, EL PRODUCTO, ACCESORIO Y EMPAQUES DEBE DE ESTAR EN LAS CONDICIONES ORIGINALES Y SIN SEÑALES DE USO");
            //currentY += HeightLetter;
            //SetText("NO SE ACEPTA DEVOLUCIONES DE DINERO LOS PRODUCTOS EN DESCUENTO SE CAMBIAN SOLO POR TALLA COLORR O HASTA AGOTAR STOCK, LA PROMOCION 2X1, VENTA FINAL Y LIQUIDACION EXTREMA NO ESTAN SUJETOS A CAMBIOS NI DEVOLUCIONES\nGRACIAS POR SU COMPRA");
        }

        private void pdoc_PrintPageChayna(object sender, PrintPageEventArgs e)
        {
            graph = e.Graphics;
            currentY = e.PageBounds.Top;
            x = e.PageBounds.Width * 0.02f;
            x2 = e.PageBounds.Width - x;
            var header = _repTicket.First();

            SetFont(new Font(Family, 9, FontStyle.Bold), graph);
            SetLine(Globals.ClientSession.TckRzs, StringAlignment.Center);
            SetFont(new Font(Family, 8), graph);
            currentY += 5;
            SetLine("RUC: " + Globals.ClientSession.TckRuc, StringAlignment.Center);
            currentY += HeightLetter;
            Justified(Globals.ClientSession.TckDireccion);
            currentY += 10;
            SetLine("NRO SERIE");
            SetLine("NRO TICKET");
            currentY -= 2 * HeightLetter;
            x = (x2 - x) * 0.40f;
            SetLine(":" + UserConfig.Default.NroTicket);
            SetLine(":" + header.Documento);
            x = e.PageBounds.Width * 0.02f;
            currentY += 8;
            SetLine("CODIGO               CANT.      P.U.      IMP");
            currentY -= 5;
            SetLine(new string('-', 55));
            foreach (var item in _repTicket)
            {
                if (item.CodigoArticulo.All( r => r == '0')) continue;
                SetLine(item.Descripcion);
                SetLine(string.Concat(
                        item.CodigoArticulo.PadRight(19),
                        item.Cantidad.ToString("0.00").PadLeft(6),
                        item.Precio.ToString("0.00").PadLeft(11),
                        item.PrecioVenta.ToString("0.00").PadLeft(9)));
            }
            SetLine(new string('-', 55));
            SetLine("TOTAL:" + header.Total.ToString("0.00").PadLeft(10), StringAlignment.Far);
            currentY += HeightLetter;
            SetLine("NOMBRE");
            SetLine("RUC/DNI");
            SetLine("FECHA");
            SetLine("VENDEDOR");
            currentY -= 4 * HeightLetter;
            x = (x2 - x) * 0.4f;
            SetLine(header.NombreCliente);
            SetLine(header.NroDocCliente);
            SetLine(DateTime.Now.ToString("dd/MM/yyyy h:mm tt"));
            SetLine(header.Vendedor);
            x = e.PageBounds.Width * 0.02f;
            currentY += 2*HeightLetter;
            SetFooter(Globals.ClientSession.GlosaTicket);
            font.Dispose();
        }

        private void pdoc_PrintPageJulioEduardo(object sender, PrintPageEventArgs e)
        {
            graph = e.Graphics;
            currentY = e.PageBounds.Top;
            x = e.PageBounds.Width * 0.02f;
            x2 = e.PageBounds.Width - x;
            var header = _repTicket.First();
            SetFont(new Font(Family, 9, FontStyle.Bold), graph);
            SetLine(Globals.ClientSession.TckRzs, StringAlignment.Center);
            SetFont(new Font(Family, 8), graph);
            currentY += 5;
            SetLine("RUC: " + Globals.ClientSession.TckRuc, StringAlignment.Center);
            currentY += HeightLetter;
            Justified(Globals.ClientSession.TckDireccion);
            currentY += 10;
            SetLine("NRO SERIE");
            SetLine("NRO TICKET");
            currentY -= 2 * HeightLetter;
            x = (x2 - x) * 0.40f;
            SetLine(":" + UserConfig.Default.NroTicket);
            SetLine(":" + header.Documento);
            x = e.PageBounds.Width * 0.02f;
            currentY += 8;
            SetLine("CODIGO               CANT.      P.U.      IMP");
            currentY -= 5;
            SetLine(new string('-', 55));
            foreach (var item in _repTicket)
            {
                if (item.CodigoArticulo.All(r => r == '0')) continue;
                SetLine(item.Descripcion);
                SetLine(string.Concat(
                        item.CodigoArticulo.PadRight(19),
                        item.Cantidad.ToString("0.00").PadLeft(6),
                        item.Precio.ToString("0.00").PadLeft(11),
                        item.PrecioVenta.ToString("0.00").PadLeft(9)));
            }
            SetLine(new string('-', 55));
            SetLine("TOTAL:" + header.Total.ToString("0.00").PadLeft(10), StringAlignment.Far);
            currentY += HeightLetter;
            SetLine("NOMBRE");
            SetLine("RUC/DNI");
            SetLine("FECHA");
            SetLine("VENDEDOR");
            currentY -= 4 * HeightLetter;
            x = (x2 - x) * 0.4f;
            SetLine(header.NombreCliente);
            SetLine(header.NroDocCliente);
            SetLine(DateTime.Now.ToString("dd/MM/yyyy h:mm tt"));
            SetLine(header.Vendedor);
            x = e.PageBounds.Width * 0.02f;
            currentY += 2 * HeightLetter;
            SetFooter(Globals.ClientSession.GlosaTicket);
            font.Dispose();
        }

        private void pdoc_PrintNotaria(object sender, PrintPageEventArgs e)
        {
            var width = 250.0f;
            graph = e.Graphics;
            currentY = e.PageBounds.Top;
            x = 0;
            x2 = width - x;
            var header = _repTicket.First();
            SetFont(new Font(Family, 9, FontStyle.Bold), graph);
            SetLine(Globals.ClientSession.TckRzs, StringAlignment.Center);
            SetFont(new Font(Family, 8), graph);
            currentY += 5;
            SetLine("RUC: " + Globals.ClientSession.TckRuc, StringAlignment.Center);
            currentY += HeightLetter;
            Justified(Globals.ClientSession.TckDireccion);
            currentY += 10;
            SetLine("NRO SERIE");
            SetLine("NRO IRPE");
            currentY -= 2 * HeightLetter;
            x = (x2 - x) * 0.40f;
            SetLine(":" + UserConfig.Default.NroTicket);
            SetLine(":" + header.Documento);
            x = 0;
            currentY += 8;
            SetRelative(width, new KeyValuePair<float, string>(0, "CODIGO"),
                new KeyValuePair<float, string>(0.42f, "CANT"), new KeyValuePair<float, string>(0.62f, "P.U."), new KeyValuePair<float, string>(0.78f, "IMP"));
            //SetLine("CODIGO               CANT.      P.U.      IMP");
            currentY -= 5;
            SetLine(new string('-', 55));
            foreach (var item in _repTicket)
            {
                if (item.CodigoArticulo.All(r => r == '0')) continue;
                SetLine(item.Descripcion);
                SetRelative(width, new KeyValuePair<float, string>(0, item.CodigoArticulo),
                new KeyValuePair<float, string>(0.42f, item.Cantidad.ToString("0.00")), new KeyValuePair<float, string>(0.62f, item.Precio.ToString("0.00")), new KeyValuePair<float, string>(0.78f, item.PrecioVenta.ToString("0.00")));
                //SetLine(string.Concat(
                //        item.CodigoArticulo.PadRight(17),
                //        item.Cantidad.ToString("0.00").PadLeft(6),
                //        item.Precio.ToString("0.00").PadLeft(11),
                //        item.PrecioVenta.ToString("0.00").PadLeft(9)));
            }

            SetLine(new string('-', 55));
            SetLine("TOTAL:" + header.Total.ToString("0.00").PadLeft(10), StringAlignment.Far);
            currentY += HeightLetter;
            SetLine("NOMBRE");
            SetLine("RUC/DNI");
            SetLine("DIRECCION");
            SetLine("FECHA");
            SetLine("C. PAGO");
            SetLine("F. PAGO");
            SetLine("VENDEDOR");
            currentY -= 7 * HeightLetter;
            x = (x2 - x) * 0.4f;
            SetLine(header.NombreCliente);
            SetLine(header.NroDocCliente);
            SetLine(header.Direccion);
            SetLine(DateTime.Now.ToString("dd/MM/yyyy h:mm tt"));
            SetLine(header.CondicionPago);
            SetLine(header.FormaPago);
            SetLine(header.Vendedor);
            x = 0;
            currentY += 2 * HeightLetter;

            SetFooter(Globals.ClientSession.GlosaTicket);
            font.Dispose();
        }
        #endregion
    }

    /// <summary>
    /// Class TicketPrinter.
    /// </summary>
    public class TicketIrpe : TicketPrinter
    {
        #region Fields
        private List<ReporteDocumentoFactura> _repTicket;
        private readonly IEnumerable<string> _idVentas;
        #endregion

        #region Init
        public TicketIrpe(IEnumerable<string> idVentas)
        {
            _idVentas = idVentas;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prints Documents.
        /// </summary>
        public override void Print()
        {
            if (_idVentas == null || !_idVentas.Any()) return;
            foreach (var id in _idVentas)
            {
                var opRes = new OperationResult();
                _repTicket = new Venta.BL.VentaBL().ReporteDocumentoVenta(ref opRes, id);
                if (_repTicket == null || _repTicket.Count == 0) continue;
                using (var pdoc = new PrintDocument())
                {
                    pdoc.PrintPage += pdoc_Print;

                    var f = _repTicket.First();
                    var serie = f.Documento.Split('-')[0].Trim();
                    Tuple<string, int> config = (from r in Globals.ListaEstablecimientoDetalle
                                                 where r.i_IdTipoDocumento == (int)DocumentType.Irpe/*int.Parse(f.TipoDocumento)*/ && !string.IsNullOrEmpty(r.v_NombreImpresora) && r.v_Serie.Trim() == serie
                                                 select new Tuple<string, int>(r.v_NombreImpresora, r.i_ImpresionVistaPrevia ?? 0)
                    ).FirstOrDefault();
                    try
                    {
                        if (config != null)
                        {
                            pdoc.PrinterSettings.PrinterName = config.Item1;
                            if (config.Item2 == 1)
                            {
                                //PrintDialog pd = new PrintDialog();
                                //var psize = new System.Drawing.Printing.PaperSize("Custom", 100, 200);
                                //pd.Document = pdoc;
                                //pd.Document.DefaultPageSettings.PaperSize = psize;
                                using (var pp = new PrintPreviewDialog { Document = pdoc })
                                {
                                    if (pp.ShowDialog() != DialogResult.OK) return;
                                }
                            }
                        }
                        else
                        {
                            if (
                                UltraMessageBox.Show(
                                    "NO SE ENCONTRO CONFIGURACIÓN DE IMPRESION,¿Desea usar Impresora Predeterminada?",
                                    Botones: MessageBoxButtons.YesNo, Icono: MessageBoxIcon.Question) == DialogResult.No)
                                return;
                        }
                        pdoc.Print();
                    }
                    catch (Exception er)
                    {
                        UltraMessageBox.Show(er.Message, "Error en Impresion", Icono: MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void pdoc_Print(object sender, PrintPageEventArgs e)
        {
            var width = 250.0f;
            graph = e.Graphics;
            currentY = e.PageBounds.Top;
            x = 0;
            x2 = width - x;
            var header = _repTicket.First();
            SetFont(new Font(Family, 9, FontStyle.Bold), graph);
            SetLine(Globals.ClientSession.TckRzs, StringAlignment.Center);
            SetFont(new Font(Family, 8), graph);
            currentY += 5;
            SetLine("RUC: " + Globals.ClientSession.TckRuc, StringAlignment.Center);
            currentY += HeightLetter;
            Justified(Globals.ClientSession.TckDireccion);
            currentY += 10;
            SetLine("NRO SERIE");
            SetLine("NRO IRPE");
            currentY -= 2 * HeightLetter;
            x = (x2 - x) * 0.40f;
            SetLine(":" + UserConfig.Default.NroTicket);
            SetLine(":" + header.Documento);
            x = 0;
            currentY += 8;
            SetRelative(width, new KeyValuePair<float, string>(0, "CODIGO"),
                new KeyValuePair<float, string>(0.28f, "CANT"), new KeyValuePair<float, string>(0.52f, "P.U."), new KeyValuePair<float, string>(0.67f, "IMP"));
            currentY -= 5;
            SetLine(new string('-', 55));
            foreach (var item in _repTicket)
            {
                if (item.CodigoArticulo.All(r => r == '0')) continue;
                SetText(item.Descripcion);
                //SetLine(item.Descripcion);
                SetRelative(width, new KeyValuePair<float, string>(0, item.CodigoArticulo),
                new KeyValuePair<float, string>(0.28f, item.Cantidad.ToString("0.00")), new KeyValuePair<float, string>(0.52f, item.Precio.ToString("0.00")), new KeyValuePair<float, string>(0.67f, item.PrecioVenta.ToString("0.00")));
            }

            SetLine(new string('-', 55));
            SetLine("TOTAL:" + header.Total.ToString("0.00").PadLeft(10), StringAlignment.Far);
            currentY += HeightLetter;

            foreach (var s in new Dictionary<string, string>(){ 
            { "NOMBRE", header.NombreCliente},
            { "RUC/DNI", header.NroDocCliente}, 
            { "DIRECCION", header.Direccion}, 
            { "FECHA", DateTime.Now.ToString("dd/MM/yyyy h:mm tt")}, 
            { "C. PAGO", header.CondicionPago },
            {"F. PAGO", header.FormaPagoCobranza}})
            {
                x = 0;
                SetLine(s.Key);
                x = (x2 - x) * 0.38f;
                currentY -= HeightLetter;
                SetText(s.Value);
            }

            x = 0;
            SetText(header.Kardex);
            if (string.IsNullOrEmpty(header.Kardex)) currentY += HeightLetter;
            SetLine("CAJERO");
            currentY -= HeightLetter;
            x = (x2 - x) * 0.38f;
            SetLine(header.Vendedor);
            currentY += 2 * HeightLetter;
            x = 0;
            SetFooter(Globals.ClientSession.GlosaTicket);
            font.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Class for Credit Note.
    /// </summary>
    /// <seealso cref="SAMBHS.Windows.WinClient.UI.Reportes.Ventas.Ablimatex.TicketPrinter" />
    public class TicketNcr : TicketPrinter
    {
        #region Fields
        private List<ReporteDocumentoNotaCredito> _repTicket;
        private readonly IEnumerable<string> _idVentas;
        private ReporteAlmacen _repAlmacen;
        #endregion

        #region Construct
        public TicketNcr(IEnumerable<string> idVentas)
        {
            _idVentas = idVentas;
        }
        #endregion

        #region Methods
        public override void Print()
        {
           
            if (_idVentas == null || !_idVentas.Any()) return;
            _repAlmacen = new AlmacenBL().ReporteAlmacen(Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1).FirstOrDefault();
            foreach (var idVenta in _idVentas)
            {
                _repTicket = new Venta.BL.VentaBL().ReporteDocumentoNotaCredito(idVenta);
                using (var pdoc = new PrintDocument())
                {
                    pdoc.PrintPage += pdoc_PrintPage;

                    var f = _repTicket.First();
                    var serie = f.Documento.Split('-')[0].Trim();
                    var config = (from r in Globals.ListaEstablecimientoDetalle
                                  where r.i_IdTipoDocumento == (int)DocumentType.NotaCredito/*int.Parse(f.TipoDocumento)*/ && !string.IsNullOrEmpty(r.v_NombreImpresora) && r.v_Serie.Trim() == serie
                                  select new Tuple<string, int>(r.v_NombreImpresora, r.i_ImpresionVistaPrevia ?? 0)
                    ).FirstOrDefault();
                    try
                    {
                        if (config != null)
                        {
                            pdoc.PrinterSettings.PrinterName = config.Item1;
                            if (config.Item2 == 1)
                                using (var pp = new PrintPreviewDialog { Document = pdoc })
                                    if (pp.ShowDialog() != DialogResult.OK) return;
                        }
                        else
                            if (UltraMessageBox.Show("NO SE ENCONTRO CONFIGURACIÓN DE IMPRESION,¿Desea usar Impresora Predeterminada?",
                                    Botones: MessageBoxButtons.YesNo, Icono: MessageBoxIcon.Question) == DialogResult.No)
                                return;
                        pdoc.Print();
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
            graph = e.Graphics;
            currentY = e.PageBounds.Top;
            const float width = 250.0f;
            x = 0;
            x2 = width - x;
            var header = _repTicket.First();

            SetFont(new Font(Family, 9, FontStyle.Bold), graph);

            using (var reader = new StringReader(_repAlmacen.v_NombreComercial))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    if (str == string.Empty) currentY += HeightLetter;
                    else SetLine(str, StringAlignment.Center);
                }
            }
            SetFont(new Font(Family, 8), graph);
            currentY += 5;
            SetLine("RUC: " + _repAlmacen.v_NumeroSerieTicket);
            currentY += HeightLetter;
            Justified(_repAlmacen.v_Direccion);
            currentY += 10;
            SetRelative(width, new Par(0, "NRO SERIE"), new Par(0.45f, UserConfig.Default.NroTicket));
            SetRelative(width, new Par(0, "NOTA DE CREDITO"), new Par(0.45f, header.Documento));
            SetRelative(width, new Par(0, "EMITIDO"), new Par(0.45f, header.FechaRegistro.ToString("dd/MM/yyyy")));
            SetRelative(width, new Par(0, "DOC. REF."), new Par(0.45f, header.TipoDocumentoRef + " " + header.DocumentoRef));
            SetRelative(width, new Par(0, "FECHA DOC. REF."), new Par(0.45f, header.FechaRegistroRef.ToString("dd/MM/yyyy")));
            currentY += 8;
            SetRelative(width,
                new Par(0, "CODIGO"),
                new Par(0.40f, "CANT."),
                new Par(0.57f, "P.U."),
                new Par(0.77f, "IMP"));
            Separator();
            var sumCant = 0M;
            foreach (var item in _repTicket)
            {
                if (item.CodigoArticulo.All(r => r == '0')) continue;
                SetLine(item.Descripcion);
                SetForPercent(width,
                    new Par(0.40f, item.CodigoArticulo),
                    new Par(0.17f, item.Cantidad.ToString("0.00")),
                    new Par(0.20f, item.Precio.ToString("0.00")),
                    new Par(0.23f, item.PrecioVenta.ToString("0.00")));
                sumCant += item.Cantidad;
            }
            Separator();
            SetLine("TOTAL PRENDAS: " + sumCant.ToString("0.00").PadLeft(11), StringAlignment.Far);
            SetLine("SUB TOTAL:        " + header.ValorVenta.ToString("0.00").PadLeft(11), StringAlignment.Far);
            SetLine("IGV:                   " + header.Igv.ToString("0.00").PadLeft(11), StringAlignment.Far);
            SetLine("TOTAL:               " + header.Total.ToString("0.00").PadLeft(11), StringAlignment.Far);
            currentY += HeightLetter;

            //SetLine(DateTime.Now.ToString("dd/MM/yyyy  h:mm tt"));
            SetRelative(width, new Par(0, "VENDEDOR"), new Par(0.35f, header.Vendedor), new Par());
            SetRelative(width, new Par(0, "CLIENTE"), new Par(0.35f, header.NombreCliente), new Par());
            var isFact = header.NroDocCliente.Length == 11;
            if (isFact)
            {
                SetRelative(width, new Par(0, "RUC"), new Par(0.35f, header.NroDocCliente), new Par());
                SetRelative(width, new Par(0, "DIRECCION"), new Par(0.35f, header.Direccion), new Par());
            }
            currentY += HeightLetter;
            Separator();
            SetRelative(width, new Par(0, "CLIENTE"), new Par(0.35f, header.NombreCliente), new Par());
            SetRelative(width, new Par(0, isFact ? "RUC" : "DNI"), new Par(0.35f, header.NroDocCliente), new Par());
            currentY += 2 * HeightLetter;
            SetRelative(width, new Par(0, "FIRMA"), new Par(0.35f, new string('.', 50)), new Par());
            Separator();
            font.Dispose();
        }
        private void Separator()
        {
            currentY += 3;
            graph.DrawLine(Pens.Black, x, currentY, x2, currentY);
            currentY += 3;
        }
        #endregion

    }
}
