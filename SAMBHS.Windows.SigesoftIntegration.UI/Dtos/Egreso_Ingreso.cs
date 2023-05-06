using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class Egreso_Ingreso
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
        public string UsuarioCrea { get; set; }
        public string v_DescripcionProducto { get; set; }
    }
}
