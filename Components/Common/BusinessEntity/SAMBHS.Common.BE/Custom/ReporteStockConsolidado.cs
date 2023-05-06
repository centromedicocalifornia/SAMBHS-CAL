using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteStockConsolidado
    {
        public string linea { get; set; }
        public string codigoProducto { get; set; }
        public string descripcionProducto { get; set; }
        public string IdProducto { get; set; }
        public string almacen1 { get; set; }
        public string almacen2 { get; set; }
        public string almacen3 { get; set; }
        public string almacen4 { get; set; }
        public string almacen5 { get; set; }
        public string almacen6 { get; set; }
        public string almacen7 { get; set; }
        public string almacen8 { get; set; }
        public string almacen9 { get; set; }
        public string almacen10 { get; set; }
        public string almacen11 { get; set; }
        public decimal? cantidadAlmacen1 { get; set; }
        public decimal? cantidadAlmacen2 { get; set; }
        public decimal? cantidadAlmacen3 { get; set; }
        public decimal? cantidadAlmacen4 { get; set; }
        public decimal? cantidadAlmacen5 { get; set; }
        public decimal? cantidadAlmacen6 { get; set; }
        public decimal? cantidadAlmacen7 { get; set; }
        public decimal? cantidadAlmacen8 { get; set; }
        public decimal? cantidadAlmacen9 { get; set; }
        public decimal? cantidadAlmacen10 { get; set; }
        public decimal? cantidadAlmacen11 { get; set; }

        public int? tipoMovimiento { get; set; }
        public int? devolucion { get; set; }
        public int? idAlmacen { get; set; }
        public decimal? cantidad { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public string unidad { get; set; }
        // Agregate Hormiga
        public string Marca { get; set; }
        public string Temporada { get; set; }
        public string Coleccion { get; set; }
        public string Op { get; set; }
        public string Talla { get; set; }
        public string Genero { get; set; }
        public string Color { get; set; }
        public decimal PVenta { get; set; }
        public decimal PCosto { get; set; }
        public string Material { get; set; }
        public int Anio { get; set; }
        public decimal PrecioMayorista { get; set; }
        public DateTime Fecha { get; set; }
        public string IdMovimiento { get; set; }
        public string ValorUM { get; set; }
        public bool  EsCantidadCeroAlmacen { get; set; }
    }

    public class ReporteStockConsolidadoCajasUnidades
    {

        public string linea { get; set; }
        public string codigoProducto { get; set; }
        public string descripcionProducto { get; set; }
        public string almacen1 { get; set; }
        public string almacen2 { get; set; }
        public string almacen3 { get; set; }
        public string almacen4 { get; set; }
        public string almacen5 { get; set; }
        public string almacen6 { get; set; }
        public string almacen7 { get; set; }
        public string almacen8 { get; set; }
        public string almacen9 { get; set; }
        public string almacen10 { get; set; }
        public decimal? cantidadAlmacen1 { get; set; }
        public decimal? cantidadAlmacen2 { get; set; }
        public decimal? cantidadAlmacen3 { get; set; }
        public decimal? cantidadAlmacen4 { get; set; }
        public decimal? cantidadAlmacen5 { get; set; }
        public decimal? cantidadAlmacen6 { get; set; }
        public decimal? cantidadAlmacen7 { get; set; }
        public decimal? cantidadAlmacen8 { get; set; }
        public decimal? cantidadAlmacen9 { get; set; }
        public decimal? cantidadAlmacen10 { get; set; }
        public decimal? cajasAlmacen1 { get; set; }
        public decimal? cajasAlmacen2 { get; set; }
        public decimal? cajasAlmacen3 { get; set; }
        public decimal? cajasAlmacen4 { get; set; }
        public decimal? cajasAlmacen5 { get; set; }
        public decimal? cajasAlmacen6 { get; set; }
        public decimal? cajasAlmacen7 { get; set; }
        public decimal? cajasAlmacen8 { get; set; }
        public decimal? cajasAlmacen9 { get; set; }
        public decimal? cajasAlmacen10 { get; set; }
        public int? tipoMovimiento { get; set; }
        public int? devolucion { get; set; }
        public int? idAlmacen { get; set; }
        public decimal? cantidad { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public decimal? d_Empaque { get; set; }
        public int? i_idUnidadMedidaEmpaque { get; set; }
        public string v_idUnidadMedidaEmpaque { get; set; }
        public decimal? d_CantidadEmpaque { get; set; }
        public int? i_IdUnidadMedidaElejido { get; set; }
        public string v_IdUnidadMedidaElejido { get; set; }
        public decimal? cajas { get; set; }
        public decimal? unidad { get; set; }
        public decimal? d_Cantidad { get; set; }
        public int? cajasInt { get; set; }
        public DateTime Fecha { get; set; }
        public string idMovimiento { get; set; }
        public string idMovimientoDetalle { get; set; }
    }

    public class ReporteTransferencias
    {

        public string NroTransferencia { get; set; }
        public string NroIngreso { get; set; }
        public string NroSalida { get; set; }
        public string IdMovimiento { get; set; }
        public int TipoMovimiento { get; set; }
        public string Key { get; set; }
    
    }

    public class ProductoStock
    {
        public string v_IdProducto { get; set; }
        public string v_CodInterno { get; set; }
        public string v_Descripcion { get; set; }
        public string v_ProductoDetalleId { get; set; }
        public decimal d_StockActual { get; set; }
    }
}
