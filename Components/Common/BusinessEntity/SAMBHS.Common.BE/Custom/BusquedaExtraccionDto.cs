using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class BusquedaExtraccionDto
    {
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public string NombreCliente { get; set; }
        public string ID { get; set; }
        public int Estado { get; set; }
        public decimal Total { get; set; }
        public string  Moneda { get; set; }
        public int iTipoDocumento { get; set; }
        public string Origen { get; set; }
        public string v_Concepto { get; set; }
    }
}
