using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class letraspagardetalleDto
    {
        public string NroDocLetra { get; set; }
        public string Moneda { get; set; }
        public string Estado { get; set; }
        public string Ubicacion { get; set; }
        public string NroUnico { get; set; }
        public decimal Saldo { get; set; }
    }
}
