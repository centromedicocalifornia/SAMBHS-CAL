using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteImportacionesProveeedorAnalitico
    {

        public string Moneda { get; set; }
        public string Almacen { get; set; }
        public string Proveedor { get; set; }
        public string NroRegistro { get; set; }
        public DateTime Fecha { get; set; }
        public string Documento { get; set; }
        public string NroPedido { get; set; }
        public string CodProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? CostoSinIgv { get; set; }
        public decimal? TotalCosto { get; set; }
        public string GrupoLLave { get; set; }
        public int? i_IdMoneda { get; set; }
        public int? i_IdAlmacen { get; set; }
        public string CodProveedor { get; set; }


    }
    public class ValoresImportacionCambio
    {

        public decimal Fob { get; set; }
        public decimal Flete { get; set; }
        public decimal Advalorem { get; set; }
        public decimal Seguro { get; set; }
        public decimal OtrosGastos { get; set; }
        public decimal Igv { get; set; }
        public decimal FobProducto { get; set; }
        public decimal FleteProducto { get; set; }
        public decimal SeguroProducto { get; set; }
        public decimal AdvaloremProducto { get; set; }
        public decimal IgvProducto { get; set; }
        public decimal OtrosGastosProducto { get; set; }
        public decimal TotalProducto { get; set; }
        public decimal CostoUnitarioProducto { get; set; }
        public decimal PrecioUnitarioProducto { get; set; }

    
    }
}
