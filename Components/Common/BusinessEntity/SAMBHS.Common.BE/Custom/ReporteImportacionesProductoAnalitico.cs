using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public  class ReporteImportacionesProductoAnalitico
    {
        public string Moneda { get; set; }
        public string Almacen { get; set; }
        public string Proveedor { get; set; }
        public string NroRegistro { get; set; }
        public DateTime Fecha { get; set; }
        public string Documento { get; set; }
        public string NroPedido { get; set; }
        public string Producto { get; set; }
       
        public decimal? Cantidad { get; set; }
        public decimal? CostoSinIgv { get; set; }
        public decimal? TotalCosto { get; set; }
        public string GrupoLLave { get; set; }
        public int? i_IdMoneda { get; set; }
        public int? i_IdAlmacen { get; set; }
        public string CodProveedor { get; set; }
        public string CodProducto { get; set; }

    }
}
