using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class ProductoCustom
    {
        //public ProductoCustom();
        //public ProductoCustom(string v_IdProducto, string v_IdLinea, string v_IdMarca, string v_IdModelo, string v_CodInterno, string v_Descripcion, decimal? d_Empaque, int? i_IdUnidadMedida, decimal? d_Peso, string v_Ubicacion, string v_Caracteristica, string v_CodProveedor, string v_Descripcion2, int? i_IdTipoProducto, string v_RutaFoto, int? i_EsAfectoPercepcion, decimal? d_TasaPercepcion, int? i_EsAfectoDetraccion, int? i_EsServicio, int? i_EsActivoFijo, int? i_NombreEditable, int? i_PrecioEditable, int? i_EsActivo, int? i_EsLote, int? i_ValidarStock, decimal? d_PrecioVenta, decimal? d_PrecioCosto, decimal? d_StockMinimo, decimal? d_StockMaximo, int? i_IdProveedor, int? i_IdTipo, int? i_IdUsuario, int? i_IdTela, int? i_IdEtiqueta, int? i_IdCuello, int? i_IdAplicacion, int? i_IdArte, int? i_IdColeccion, int? i_IdTemporada, int? i_Anio, int? i_Eliminado, int? i_InsertaIdUsuario, DateTime? t_InsertaFecha, int? i_ActualizaIdUsuario, DateTime? t_ActualizaFecha, string v_IdColor, string v_IdTalla, byte[] b_Foto, string v_Modelo, short? i_EsAfectoIsc, int? i_CantidadFabricacionMensual, string v_NroPartidaArancelaria, int? i_IndicaFormaParteOtrosTributos, string v_NroParte, string v_NroOrdenProduccion, int? i_IdTipoTributo, decimal? d_Utilidad, decimal? d_PrecioMayorista, int? i_IdPerfilDetraccion, int? i_SolicitarNroSerieIngreso, int? i_SolicitarNroLoteIngreso, int? i_SolicitaOrdenProduccionIngreso, int? i_SolicitarNroSerieSalida, int? i_SolicitarNroLoteSalida, int? i_SolicitaOrdenProduccionSalida, string v_AccionFarmaco, string v_Presentacion, string v_Concentracion, string v_PrincipioActivo);

        public byte[] b_Foto { get; set; }
        public decimal? d_Empaque { get; set; }
        public decimal? d_Peso { get; set; }
        public decimal? d_PrecioCosto { get; set; }
        public decimal? d_PrecioMayorista { get; set; }
        public decimal? d_PrecioVenta { get; set; }
        public decimal? d_separacion { get; set; }
        public decimal? d_StockMaximo { get; set; }
        public decimal? d_StockMinimo { get; set; }
        public decimal? d_TasaPercepcion { get; set; }
        public decimal? d_Utilidad { get; set; }
        public string EmpaqueUnidadMedida { get; set; }
        public int? i_ActualizaIdUsuario { get; set; }
        public int? i_Anio { get; set; }
        public int? i_CantidadFabricacionMensual { get; set; }
        public int? i_Eliminado { get; set; }
        public int? i_EsActivo { get; set; }
        public int? i_EsActivoFijo { get; set; }
        public int? i_EsAfectoDetraccion { get; set; }
        public short? i_EsAfectoIsc { get; set; }
        public int? i_EsAfectoPercepcion { get; set; }
        public int? i_EsLote { get; set; }
        public int? i_EsServicio { get; set; }
        public int? i_IdAplicacion { get; set; }
        public int? i_IdArte { get; set; }
        public int? i_IdColeccion { get; set; }
        public int? i_IdCuello { get; set; }
        public int? i_IdEtiqueta { get; set; }
        public int? i_IdPerfilDetraccion { get; set; }
        public int? i_IdProveedor { get; set; }
        public int? i_IdTela { get; set; }
        public int? i_IdTemporada { get; set; }
        public int? i_IdTipo { get; set; }
        public int? i_IdTipoProducto { get; set; }
        public int? i_IdTipoTributo { get; set; }
        public int? i_IdUnidadMedida { get; set; }
        public int? i_IdUsuario { get; set; }
        public int? i_IndicaFormaParteOtrosTributos { get; set; }
        public int? i_InsertaIdUsuario { get; set; }
        public int? i_NombreEditable { get; set; }
        public int? i_PrecioEditable { get; set; }
        public int? i_SolicitaOrdenProduccionIngreso { get; set; }
        public int? i_SolicitaOrdenProduccionSalida { get; set; }
        public int? i_SolicitarNroLoteIngreso { get; set; }
        public int? i_SolicitarNroLoteSalida { get; set; }
        public int? i_SolicitarNroSerieIngreso { get; set; }
        public int? i_SolicitarNroSerieSalida { get; set; }
        public int? i_ValidarStock { get; set; }
        public int IdAlmacen { get; set; }
        public string NombreColor { get; set; }
        public string NombreLinea { get; set; }
        public string NombreMarca { get; set; }
        public string NombreModelo { get; set; }
        public string NombreTalla { get; set; }
        public string NroCuenta { get; set; }
        public decimal? stockActual { get; set; }
        public DateTime? t_ActualizaFecha { get; set; }
        public DateTime? t_InsertaFecha { get; set; }
        public string TipoProducto { get; set; }
        public string v_AccionFarmaco { get; set; }
        public string v_Caracteristica { get; set; }
        public string v_CodInterno { get; set; }
        public string v_CodProveedor { get; set; }
        public string v_Concentracion { get; set; }
        public string v_Descripcion { get; set; }
        public string v_Descripcion2 { get; set; }
        public string v_IdColor { get; set; }
        public string v_IdLinea { get; set; }
        public string v_IdMarca { get; set; }
        public string v_IdModelo { get; set; }
        public string v_IdProducto { get; set; }
        public string v_IdProductoAlmacen { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public string v_IdTalla { get; set; }
        public string v_Modelo { get; set; }
        public string v_NroOrdenProduccion { get; set; }
        public string v_NroParte { get; set; }
        public string v_NroPartidaArancelaria { get; set; }
        public string v_Presentacion { get; set; }
        public string v_PrincipioActivo { get; set; }
        public string v_RutaFoto { get; set; }
        public string v_Ubicacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
    }
}
