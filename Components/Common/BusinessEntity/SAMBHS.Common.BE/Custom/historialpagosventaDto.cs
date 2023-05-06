using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class historialpagosventaDto
    {
        public DateTime Fecha { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string Glosa { get; set; }
        public string Moneda { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Pago { get; set; }
        public decimal SaldoLetra { get; set; }
        public bool EsLetra { get; set; }
        public string IdDocumento { get; set; }
        public int  Estado { get; set; }
    }

    public class historialpagoscompraDto
    {
        public DateTime Fecha { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string Glosa { get; set; }
        public string Moneda { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Pago { get; set; }
        public decimal SaldoLetra { get; set; }
        public bool EsLetra { get; set; }
        public string IdDocumento { get; set; }
        public int Estado { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}
