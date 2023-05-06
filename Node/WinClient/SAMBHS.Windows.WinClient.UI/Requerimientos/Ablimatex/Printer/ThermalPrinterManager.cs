using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using ARTCODGENERATOR.UI;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.Ablimatex.Printer
{
    /// <summary>
    /// Class par imprimer Etiquetas en impresora Termica.
    /// </summary>
    public class ThermalPrinterManager : IDisposable
    {
        #region Fields
        private Size _size;
        private readonly double _widthChar = 26;
        private bool _disposed;
        private readonly string _printername;
        private readonly CancellationToken _token;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = Size;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ThermalPrinterManager"/> class.
        /// </summary>
        /// <param name="printerName">Name of the printer.</param>
        /// <exception cref="ArgumentException">Not found Printer</exception>
        public ThermalPrinterManager(string printerName)
            : this(printerName, CancellationToken.None)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThermalPrinterManager"/> class.
        /// </summary>
        /// <param name="printerName">Name of the printer.</param>
        /// <param name="token"></param>
        /// <exception cref="ArgumentException">Not found Printer</exception>
        public ThermalPrinterManager(string printerName, CancellationToken token)
        {
            _size = new Size(520, 270);
            _printername = printerName;
            _token = token;
        }
        #endregion

        #region Public Method

        /// <summary>
        /// Opens the Printer.
        /// </summary>
        /// <exception cref="ArgumentException">Not Found Printer</exception>
        public void Open()
        {
            if (TSCLIB.openport(_printername) == 0)
                throw new ArgumentException("Not Found Printer");
            //TSCLIB.sendcommand("INITIALPRINTER"); // restore printer settings to defaults.
        }

        /// <summary>
        /// Prints all.
        /// </summary>
        /// <param name="products">The products.</param>
        public void Print(IEnumerable<Product> products)
        {
            const int maxCant = 3;
            LoadSettings();

            var accum = new int();
            foreach (var product in products)
            {
                if (_token.IsCancellationRequested) return;

                while (product.Cantidad-- > 0)
                {
                    PrintSize(product, accum++);
                    if (accum != maxCant) continue;
                    accum = 0;
                    Thread.Sleep(200);
                    TSCLIB.printlabel("1", "1");
                    Thread.Sleep(50);
                    TSCLIB.clearbuffer();
                    Thread.Sleep(1250);
                }
            }
            // Print Residuary.
            if (accum <= 0) return;

            TSCLIB.printlabel("1", "1");
            TSCLIB.clearbuffer();
        }
        #endregion

        #region Private Method

        private void LoadSettings()
        {
            TSCLIB.clearbuffer();
            //TSCLIB.setup("100", "22", "4", "8", "0", "0", "0");
            TSCLIB.sendcommand("SIZE 99.0 mm, 33.3 mm");// Dimmensions of Label
            TSCLIB.sendcommand("DIRECTION 1,0");
            TSCLIB.sendcommand("GAP 0 mm, 0 mm");
            TSCLIB.sendcommand("SPEED 4");
            TSCLIB.sendcommand("DENSITY 8");
        }

        private void PrintSize(Product product, int position)
        {
            var size = new[,] { { 22, 50, 50 }, { 287, 320, 320 }, { 555, 580, 580 } };

            PrintLines(product.Descripcion, size[position, 0], 5);

            TSCLIB.sendcommand(string.Format("BARCODE {0:#}, {1:#},\"128\",40,2,0,1,1,\"{2}\"", size[position, 1], 95, product.Codigo));
            var price = "S/." + product.Precio.ToString("0.00");

            TSCLIB.sendcommand(string.Format("TEXT {0:#}, {1:#},\"2\",0,1,2,\"{2}\"", size[position, 2], 162, price));
        }

        private void PrintLines(string text, int x, int y)
        {
            var lines = GetLines(text);
            if (lines.Length > 3)
                lines = lines.Take(3).ToArray();

            var mount = 30 * (3 - lines.Length) / 2;
            foreach (var line in lines)
            {
                y += mount;
                TSCLIB.sendcommand(string.Format("TEXT {0:#}, {1:#}, \"1\", 0, 1, 2, \"{2}\"", x, y, line));
                y += 28; // Height for Line.
            }
        }

        private string[] GetLines(string text)
        {
            var lines = new Queue<string>();
            var words = text.Split(' ');
            var line = string.Empty;
            foreach (var word in words)
            {
                var widhText = (line.Length + word.Length + 1) * _widthChar; // 1 of whitespace
                if (widhText > _size.Width && !string.IsNullOrEmpty(line))
                {
                    lines.Enqueue(line);
                    line = word;
                    continue;
                }
                line += " " + word;
            }

            if (!string.IsNullOrEmpty(line))
                lines.Enqueue(line);
            return lines.ToArray();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ThermalPrinterManager()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            TSCLIB.clearbuffer();
            TSCLIB.closeport();
            _disposed = true;
        }
        #endregion

    }

    /// <summary>
    /// Product Model for printer.
    /// </summary>
    public class Product
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}
