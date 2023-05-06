using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteDocumentoTicket
    {

        public string NombreCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string NroDocCliente { get; set; }
        public string NroRegistro { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public int TipoDoc { get; set; }
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
        public decimal d_PrecioImpresion { get; set; }
        public decimal valorigv { get; set; }
        public string valorigvAux { get; set; }
        public decimal TipoCambio { get; set; }
        public string d_Igv { get; set; }
        public string Pedido { get; set; }
        public string GuiaRemision { get; set; }
        public int PorcentajeIgv { get; set; }
        public string v_IdVenta { get; set; }
        public string FormaPago { get; set; }
    }
}
