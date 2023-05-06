using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteOrdenCompraDto
    {
        public string NroOrdenCompra { get; set; }
        public DateTime Fecha { get; set; }
        public string Proveedor { get; set; }
        public string RUCProveedor { get; set; }
        public string Plazo { get; set; }
        public string CodArticulo { get; set; }
        public string DescripcionArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Total { get; set; }
        public string LugarEntrega { get; set; }
        public decimal Igv { get; set; }
        public string Obsevaciones { get; set; }
        public string Moneda { get; set; }
        public string EntidadBancaria { get; set; }
        public string NroDias { get; set; }
        public string NroCheque { get; set; }
        public string TipoCambio { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string DireccionProveedor { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }
        public decimal SubTotal { get; set; }
        public string UnidadMedida { get; set; }
        public string AdjuntarAnexo { get; set; }
    }
}
