using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteTransferenciaAlmacen
    {
        public string AlmacenOrigen { get; set; }
        public string AlmacenDestino { get; set; }
        public DateTime Fecha { get; set; }
        public decimal? TipoCambio { get; set; }
        public string DescripcionMotivo { get; set; }
        public string NombreMoneda { get; set; }
        public string Glosa { get;set; }
        public string CodProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public string UnidadMedida { get; set; }
        public string Guia { get; set; }
        public int  Documento { get; set; }
        public decimal? CantidadDetalle { get; set; }
        public decimal? PrecioDetalle { get; set; }
        public decimal? TotalDetalle { get; set; }
        public string Pedido { get; set; }
        public string Correlativo { get; set; }
        public string NumeroDocumento { get; set; }
    }
}
