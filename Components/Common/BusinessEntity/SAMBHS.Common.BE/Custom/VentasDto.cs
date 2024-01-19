using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class VentasDto
    {
        public string v_DescripcionProducto { get; set; }
        public float d_ValorVenta { get; set; }
        public float d_Igv { get; set; }
        public float d_Precio { get; set; }
        public int cantidad { get; set; }
    }
}
