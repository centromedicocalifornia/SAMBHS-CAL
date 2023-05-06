using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteRegistroCompraProveedorAnalitico
    {
        
        public string IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string NroDocProveedor { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdTipoDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string NroCuenta { get; set; }
        public decimal CantidadDetalle { get; set; }
        public decimal PrecioDetalle { get; set; }
        public decimal ValorVentaDetalle { get; set; }
        public decimal PrecioDetalleD { get; set; }
        public decimal ValorVentaDetalleD { get; set; }
        public decimal TipoCambio { get; set; }
        public int IdMoneda { get; set; }
        public string CorrelativoDocumento { get; set; }
        public string Grupollave { get; set; }


    }
}
