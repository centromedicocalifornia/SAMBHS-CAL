using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteResumenMovimiento
    {


        public string codigoProducto { get; set; }
        public string v_NombreProducto { get; set; }
        public DateTime t_Fecha { get; set; }
        public decimal? d_Cantidad { get; set; }
        public int? i_EsDevolucion { get; set; }
        public int? i_IdTipoMotivo { get; set; }
        public string NroPedido { get; set; }
        public string v_IdLinea { get; set; }
        public int? IdMoneda { get; set; }
        public decimal? TipoCambio { get; set; }
        public int? pIntAlmacen { get; set; }
        public int? i_IdUnidad { get; set; }
        public string Unidad { get; set; }
        public int? i_IdTipoMovimiento { get; set; }
        public decimal? d_Precio { get; set; }
        public decimal? Ingreso_Cantidad { get; set; }
        public decimal? Ingreso_Precio { get; set; }
        public decimal? Ingreso_Total { get; set; }
        public decimal? Salida_Cantidad { get; set; }
        public decimal? Salida_Precio { get; set; }
        public decimal? Salida_Total { get; set; }
        public decimal? Saldo_Cantidad { get; set; }
        public decimal? Saldo_Precio { get; set; }
        public decimal? Saldo_Total { get; set; }
        public decimal? PrecioValorizado { get; set; }
        public string v_Almacen { get; set; }
        public string v_IdMovimientoDetalle { get; set; }
        public int TipoMotivo { get; set; }
    }
}
