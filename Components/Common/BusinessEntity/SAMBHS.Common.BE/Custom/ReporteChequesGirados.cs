using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteChequesGirados
    {
        public string Cuenta { get; set; }
        public string MonedaReporte { get; set; }
        public string NumeroComprobante { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; }
        public decimal? ImporteSoles { get; set; }
        public decimal? ImporteDolares { get; set; }
        public int Moneda { get; set; }
        public string CuentaString { get; set; }
        public int? CuentaInt { get; set; }
        public decimal? Importe { get;set; }
    }
}
