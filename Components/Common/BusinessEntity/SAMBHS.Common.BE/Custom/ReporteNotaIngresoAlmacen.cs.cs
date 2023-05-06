using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public class ReporteNotaIngresoAlmacen
    {

        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
        public string IdMovimiento { get; set; }
        public DateTime Fecha { get; set; }
        public decimal TipoCambio { get; set; }
        public string OrdeCompra { get; set; }      
        public string DescripcionMotivo { get; set; }
        public string Detalle { get; set; }
        public string Concepto { get; set; }
        public string Periodo { get; set; }
        public string Mes { get; set; }
        public string Correlativo { get; set; }
        public string IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string UnidadMedida { get; set; }
        public decimal CantidadDetalle { get; set; }
        public decimal PrecioDetalle { get; set; }
        public decimal TotalDetalle { get; set; }
        public string NombreMoneda { get; set; }
        public int IdMoneda { get; set; }
        public string Guia { get; set; }
        public int? Documento { get; set; }
        public string NumeroDocumento { get; set; }
        public string v_IdCliente { get; set; }
        public string v_NroOrdenCompra { get; set; }
    }
}
