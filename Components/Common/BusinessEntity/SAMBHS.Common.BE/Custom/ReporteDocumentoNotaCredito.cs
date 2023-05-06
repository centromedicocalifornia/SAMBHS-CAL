using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
 public   class ReporteDocumentoNotaCredito
    {
        public string NombreCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string NroDocCliente { get; set; }
        public string NroRegistro { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }

        public string Moneda { get; set; }
        public string Vendedor { get; set; }
        public string Direccion { get; set; }
        public string CondicionPago { get; set; }
        public decimal ValorVenta { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public decimal Descuento { get; set; }
        public DateTime FechaRegistro { get; set; }

        public string CodigoArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Unidad { get; set; }
        public decimal d_Igvdetalle { get; set; }
        public decimal PrecioVenta { get; set; }
        public string NombreEmpresaPropietaria { get; set; }
        public decimal d_Valor { get; set; }
        public decimal d_ValorVenta { get; set; }
        public decimal d_Descuento { get; set; }
        public string DocumentoRef { get; set; }
        public string TipoDocumentoRef { get; set; }
        public DateTime FechaRegistroRef { get; set; }
        public string Concepto { get; set; }
        public string  PorcentajeIvg { get; set; }
        public string SerieDocumento { get; set; }
        public string Observacion { get; set; }
        public string FechaLetras { get; set; }
        public string CantidadLetras { get; set; }
        public int Mes { get; set; }
        public DateTime DFechaVencimiento { get; set; }
        public string SFechaVencimiento { get; set; }
        public DateTime DFechaOC { get; set; }
        public string SFechaOC { get; set; }
        public string NroOrdenCompra { get; set; }
        public string Ubigeo { get; set; }
        public int IdDepartamento { get; set; }
        public int IdProvincia { get; set; }
        public int IdDistrito { get; set; }
        public string UnidadSiglas { get; set; }
        public string MesEmisionLetras { get; set; }
        public decimal PrecioDscto { get; set; }
        public int i_PrecioIncluyeIgv { get; set; }
       
        
    }
}
