using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class importaciondetalleproductoDto
    {
        public int EsServicio { get; set; }
        public string KeyRegistro { get; set; }
        public string v_NombreProducto { get; set; }
        public string IdOrdenCompra { get; set; }
        public string v_CodigoInterno { get; set; }
        public string  Empaque { get; set; }
        public string UMEmpaque { get; set; }
        public int i_IdUnidadMedidaProducto { get; set; }
        public string v_CodCliente { get; set; }
        
    }
}
