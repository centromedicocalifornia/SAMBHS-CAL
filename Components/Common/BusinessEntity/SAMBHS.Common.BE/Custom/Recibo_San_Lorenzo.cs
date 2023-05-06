using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SAMBHS.Common.BE.Custom
{
    public class Recibo_San_Lorenzo
    {
        public string v_IdVenta { get; set; }
        public string v_SerieDocumento { get; set; }
        public string v_CorrelativoDocumento { get; set; }
        public string v_DireccionClienteTemporal { get; set; }
        public DateTime? t_FechaRegistro { get; set; }
        public decimal d_IGV { get; set; }
        public decimal d_Total { get; set; }
        public string Cliente { get; set; }
        public string v_Concepto { get; set; }
        public int? UsuarioCrea { get; set; }
        public string v_DescripcionProducto { get; set; }
    }
}
