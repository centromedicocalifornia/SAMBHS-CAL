using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class ReporteResumenAlmacen
    {

        public string codigoProducto { get; set; }
        public string descripcionProducto { get; set; }
        public string unidadMedida { get; set; }
        public int? tipoMovimiento { get; set; }
        public decimal? cantidad { get; set; }
        public decimal? precio { get; set; }
        public decimal? valor { get; set; }
        public string v_almacen { get; set; }
        public int? IdAlmacen { get; set; }
        public string linea { get; set; }
        public string pedido { get; set; }
        public decimal? cantidadAnterior{get;set;}
        public decimal? precioAnterior { get; set; }
        public decimal? valorAnterior { get; set; }
        public decimal? cantidadEntrada { get; set; }
        public decimal? precioEntrada { get; set; }
        public decimal? precioSalida { get; set; }
        public decimal? valorEntrada { get; set; }
        public decimal? cantidadSalida { get; set; }
        public decimal? valorSalida { get; set; }
        public int? pIntMoneda { get; set; }
        public string pstrMoneda { get; set; }
        public int? Devolucion { get; set; }
        public decimal? tipoCambio{get;set;}
        
        public DateTime Fecha { get; set; }
        public decimal? cantidadActual { get; set; }
        public decimal? precioActual { get; set; }
        public decimal? valorActual { get; set; }
        public string v_IdMovimiento { get; set; }
        public string v_IdMovimientoDetalle { get; set; }
        public string v_IdProducto { get; set; }
    }
}
