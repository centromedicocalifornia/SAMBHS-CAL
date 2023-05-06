using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
     public class ReporteRegistroVentaClienteResumen
    {
        public string IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string NroDocCliente { get; set; }
        public decimal Total { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalD { get; set; }
        public decimal DescuentoD { get; set; }
        public int IdTipoDocumento { get; set; }
        public decimal TipoCambio { get; set; }
        public int IdMoneda { get; set; }
        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
     


    }
     public class EstadisticasVentas
     {
         public string CodigoProducto { get; set; }
         public string DescripcionProducto { get; set; }
         public string UnidadMedida { get; set; }
         public string Mes { get; set; }
         public decimal dCantidad{ get; set; }
         public string Periodo { get; set; }
         public string v_IdProducto { get; set; }
         public DateTime FechaRegistro { get; set; }
         public decimal dCantidadSaldoInicial { get; set; }
         public decimal dCantidad1 { get; set; }
         public decimal dCantidadC1 { get; set; }
         public decimal dCantidad2 { get; set; }
         public decimal dCantidadC2 { get; set; }
         public decimal dCantidad3 { get; set; }
         public decimal dCantidadC3 { get; set; }
         public decimal dCantidad4 { get; set; }
         public decimal dCantidadC4 { get; set; }
         public decimal dCantidad5 { get; set; }
         public decimal dCantidadC5 { get; set; }


         public decimal dCantidad6 { get; set; }
         public decimal dCantidadC6 { get; set; }
         public decimal dCantidad7 { get; set; }
         public decimal dCantidadC7 { get; set; }
         public decimal dCantidad8 { get; set; }
         public decimal dCantidadC8 { get; set; }
         public decimal dCantidad9 { get; set; }
         public decimal dCantidadC9 { get; set; }

         public decimal dCantidad10 { get; set; }
         public decimal dCantidadC10 { get; set; }


         public decimal dCantidad11 { get; set; }
         public decimal dCantidadC11 { get; set; }

         public decimal dCantidad12 { get; set; }
         public decimal dCantidadC12 { get; set; }
     


         public decimal dCantidadEnero { get; set; }
         public decimal dCantidadFebrero { get; set; }
         public decimal dCantidadMarzo { get; set; }
         public decimal dCantidadAbril { get; set; }
         public decimal dCantidadMayo { get; set; }
         public decimal dCantidadJunio { get; set; }
         public decimal dCantidadJulio { get; set; }
         public decimal dCantidadAgosto { get; set; }
         public decimal dCantidadSetiembre { get; set; }
         public decimal dCantidadOctubre { get; set; }
         public decimal dCantidadNoviembre { get; set; }
         public decimal dCantidadDiciembre { get; set; }
         public decimal Total { get; set; }
         public decimal TotalCompras { get; set; }
         public decimal? StockActual { get; set; }
         public string Linea { get; set; }
         public string Marca { get; set; }
         public string ValorUM { get; set; }
         public string v_IdProductoDetalle { get; set; }
         public string NroDocumento { get; set; }
         public int idAlmacen { get; set; }
         public int pa_IdAlmacen { get; set; }
         public string UnidadMedidaPa { get; set; }
         public string TipoEstadtistica { get; set; }


         public decimal dCantidadEneroCompras { get; set; }
         public decimal dCantidadFebreroCompras { get; set; }
         public decimal dCantidadMarzoCompras { get; set; }
         public decimal dCantidadAbrilCompras { get; set; }
         public decimal dCantidadMayoCompras { get; set; }
         public decimal dCantidadJunioCompras { get; set; }
         public decimal dCantidadJulioCompras { get; set; }
         public decimal dCantidadAgostoCompras { get; set; }
         public decimal dCantidadSetiembreCompras { get; set; }
         public decimal dCantidadOctubreCompras { get; set; }
         public decimal dCantidadNoviembreCompras { get; set; }
         public decimal dCantidadDiciembreCompras { get; set; }
         public decimal dCantidadC { get; set; }
         public string IdMovimientoDetalle { get; set; }
     }
     public class MesesEstadisticas
     {

         public string  Mes { get; set; }
         public DateTime FechaRegistro { get; set; }
         public string Periodo { get; set; }
         public string  iMes { get; set; }
     
     }
     public class ConsultaVentasExportacion
     {
         public string Fecha { get; set; }
         public string Cliente { get; set; }
         public string Producto { get; set; }
         public string NroFactura { get; set; }
         public decimal Fob { get; set; }
         public decimal Flete { get; set; }
         public decimal CifoCfr { get; set; }
         public decimal Comision { get; set; }
         public string Naviera { get; set; }
         public string NroBL { get; set; }
         public string  FechaBL { get; set; }
         public string Contenedor { get; set; }
         public string Banco { get; set; }
         public decimal Cantidad { get; set; }
         public string UM { get; set; }
     
     }
}
