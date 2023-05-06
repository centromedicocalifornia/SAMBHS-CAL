using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class guiaremisiondetalleDto
    {
        public string v_CodInterno { get; set; }
        public string v_Descripcion { get; set; }
        public string Empaque { get; set; }
        public string UMEmpaque { get; set; }
        public string i_IdUnidadMedidaProducto { get; set; }
        public string NroCuenta { get; set; }
        public int i_EsServicio { get; set; }
    }
    public partial class temporalventadetalleDto
    {
        public int i_EsServicio { get; set; }
        public decimal? d_Valor { get; set; }
        public string v_Descuento { get; set; }
    }

    public class Gridtemporalventadetalle
    {

        public int v_IdTemporalVentaD { get; set; }
        public int i_IdAlmacen { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public int i_IdUnidadEmpaque { get; set; }
        public decimal d_Cantidad { get; set; }
        public int i_IdUnidadMedida { get; set; }
        public decimal d_CantidadEmpaque { get; set; }
        public decimal d_Precio { get; set; }
        public decimal d_Total { get; set; }
        public decimal d_CantidadBulto { get; set; }
        public int i_IdTipoBulto { get; set; }
        public string v_Observacion { get; set; }
        public string v_IdMovimientoDetalle { get; set; }
        public decimal? d_Valor { get; set; }
        public string v_Descuento { get; set; }
        public decimal? d_Descuento { get; set; }

        public decimal? d_ValorVenta { get; set; }
        public decimal? d_Igv { get; set; }
        public string v_IdGuiaRemisionDetalle { get; set; }

    }

    public class CalculoGuiaInternTotales
    {

        public decimal Subtotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public decimal Descuento { get; set; }
    
    }

}
