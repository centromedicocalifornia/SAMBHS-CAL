using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class productoDto
    {
        public string NombreLinea { get; set; }
        public string NombreMarca { get; set; }
        public string NombreModelo { get; set; }
        public string NombreTalla { get; set; }
        public string NombreColor { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public decimal? stockActual { get; set; }
        public string v_IdProductoAlmacen { get; set; }
        public decimal? d_separacion { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public string EmpaqueUnidadMedida { get; set; }
        public int IdAlmacen { get; set; }
        public string TipoProducto { get; set; }
        public string NroCuenta { get; set; }
       
    }

    public class productoshortDto
    {
        public decimal? stockActual { get; set; }
        public string v_IdProductoAlmacen { get; set; }
        public decimal? d_separacion { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public string EmpaqueUnidadMedida { get; set; }
        public int IdAlmacen { get; set; }
        public string v_IdProducto { get; set; }
        public string v_Descripcion { get; set; }
        public string v_CodInterno { get; set; }
        public int? i_EsServicio { get; set; }
        public int? i_EsLote { get; set; }
        public int? i_IdTipoProducto { get; set; }
        public int? i_IdUnidadMedida { get; set; }
        public decimal? d_Empaque { get; set; }
        public int? i_EsAfectoDetraccion { get; set; }
        public string AfectoDetraccion {
            get
            {
                return (i_EsAfectoDetraccion??0) == 1 ? "SI" : "NO"; 
            } 
        }
        public string TasaDetraccion { get; set; }
        public string TopeDetraccion { get; set; }
        public decimal DTasaDetraccion {
            get
            {
                decimal d;
                if (string.IsNullOrWhiteSpace(TasaDetraccion)) return 0m;
                return decimal.TryParse(TasaDetraccion, out d) ? d : 0m;
            }
        }

        public decimal DTopeDetraccion {
            get
            {
                decimal d;
                if (string.IsNullOrWhiteSpace(TopeDetraccion)) return 0m;
                return decimal.TryParse(TopeDetraccion, out d) ? d : 0m;
            }
        }
        public decimal? d_PrecioMinSoles { get; set; }
        public decimal? d_DescuentoLP { get; set; }
        public decimal? d_Precio { get; set; }
        public decimal? d_Descuento { get; set; }
        public int? i_NombreEditable { get; set; }
        public decimal? d_Costo { get; set; }
        public decimal? StockDisponible { get; set; }
        public int? i_ValidarStock { get; set; }
        public int? i_EsAfectoPercepcion { get; set; }
        public decimal? d_TasaPercepcion { get; set; }
        public int? i_PrecioEditable { get; set; }
        public string EmpaqueUnidadMedidaFinal { get; set; }
        public int IdMoneda { get; set; }
        public string NroCuentaVenta { get; set; }
        public string NroCuentaCompra { get; set; }
        public string ValorUM { get; set; }
        public decimal PrecioVenta { get; set; }
        public string Moneda { get; set; }
        public bool EsProductoFinal { get; set; }
        public string v_NroPedidoExportacion { get; set; }
        public int i_IdAlmacen { get; set; }
        public decimal? StockActualUM { get; set; }
        public decimal? SeparacionActualUM { get; set; }
        public decimal? SaldoUM { get; set; }
        public string UM { get; set; }
        public string Observacion { get; set; }
        public bool EsAfectoIsc { get; set; }
        public int  StockMinimo { get; set; }
        public string v_Descripcion2 { get; set; }
        public decimal d_StockMinimo { get; set; }
        public string Linea { get; set; }
        public string Ubicacion { get; set; }


        public int i_SolicitarNroLoteIngreso { get; set; }
        public int i_SolicitarNroSerieIngreso { get; set; }
        public int i_SolicitaOrdenProduccionIngreso { get; set; }


        public int i_SolicitarNroSerieSalida { get; set; }
        public int i_SolicitarNroLoteSalida { get; set; }
        public int i_SolicitaOrdenProduccionSalida { get; set; }


        public string v_NroSerie { get; set; }
        public string v_NroLote { get; set; }
        public DateTime? t_FechaCaducidad { get; set; }
    }
    public class EstablecimientoNuevo
    {
        public int i_IdEstablecimiento { get; set; }
        public string NombreAlmacen { get; set; }
    }

    public partial class productorecetaDto
    {
        public string NombreInsumo { get; set; }
        public string CodInternoInsumo { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public Byte[] Foto { get; set; }
    }
    public partial class productorecetasalidaDto
    {

        public string v_Descripcion { get; set; }
        public string v_CodInterno { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
    
    }
}
