using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class pedidoDto
    {

        public string RucCliente { get; set; }
        public string CodCliente { get; set; }
        public string RazonSocial { get; set; }
        public int? IdLista { get; set; }
        public int TipoDocumentoCliente { get; set; }
        public string Direccion { get; set; }
        public string Glosa { get; set; }
        //public string Vendedor { get; set; }
        public string CondicionPago { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public string CodigoArticulo { get; set; }

        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal { get; set; }
        public bool DespachadoTotalmente { get; set; }

        #region Bandeja - Administrar Pedidos
        public string NroRegistro { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }

        public string NombreCliente { get; set; }
        public string NroFactura { get; set; }
        public string Moneda { get; set; }
        public decimal Total { get; set; }
        public string Origen { get; set; }
        public string NroDocCliente { get; set; }

        #endregion

        #region Bandeja - Consulta Pedidos
        public string NumeroPedido { get; set; }
        public string Vendedor { get; set; }
        public string CodigoCliente { get; set; }
        public string Cliente { get; set; }
        public string NombreEmpresaPropietaria { get; set; }
        #endregion

        #region QueryCompilado
        public partial class CompiladoResultPedido
        {
            public string Nombre { get; set; }
            public string CodigoInterno { get; set; }
            public decimal? Empaque { get; set; }
            public string UMEmpaque { get; set; }
            public int? i_IdUnidadMedida { get; set; }
            public string v_IdProductoAlmacen { get; set; }
            public decimal? d_StockActual { get; set; }
            public decimal? d_SeparacionTotal { get; set; }
            public int? i_EsServicio { get; set; }
            public int? i_EsAfectoDetraccion { get; set; }
            public int? i_NombreEditable { get; set; }
            public int? i_ValidarStock { get; set; }
            public int? i_IdUnidadMedidaEmpaque { get; set; }
            public string v_Periodo { get; set; }
        }
        #endregion
    }

    public partial class BindingPedidoDetalle
    {
        public string v_NombreProducto { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public string v_IdPedidoDetalle { get; set; }
        public decimal? Empaque { get; set; }
        public string UMEmpaque { get; set; }
        public int? i_IdAlmacen { get; set; }
        public decimal? d_Cantidad { get; set; }
        public int? i_IdUnidadMedida { get; set; }
        public decimal? d_PrecioUnitario { get; set; }
        public decimal? d_PrecioVenta { get; set; }
        public int? i_NroUnidades { get; set; }
        public string v_Observacion { get; set; }
        public string v_IdProductoAlmacen { get; set; }
        public string v_IdPedido { get; set; }
        public decimal? d_SeparacionTotal { get; set; }
        public decimal? d_StockActual { get; set; }
        public int? i_EsAfectoDetraccion { get; set; }
        public int? i_EsServicio { get; set; }
        public string v_CodigoInterno { get; set; }
        public decimal? d_DescuentoLP { get; set; }
        public int? EsNombreEditable { get; set; }
        public int? EsRedondeo { get; set; }
        public decimal? d_Valor { get; set; }
        public decimal? d_Descuento { get; set; }
        public decimal? d_ValorVenta { get; set; }
        public decimal? d_Igv { get; set; }
        public decimal? d_PrecioVentag { get; set; }
        public int? i_ValidarStock { get; set; }
        public int? i_IdUnidadMedidaProducto { get; set; }
        public decimal? d_CantidadEmpaque { get; set; }
        public int? i_Eliminado { get; set; }
        public int? i_InsertaIdUsuario { get; set; }
        public DateTime? t_InsertaFecha { get; set; }
        public int? i_PrecioEditable { get; set; }
        public string v_Descuento { get; set; }
        public string NroCuenta { get; set; }
        public string NroPedido { get; set; }
        public decimal CantidadOriginal { get; set; }
        public decimal CantidadFacturada { get; set; }
        public int? i_IdTipoOperacion { get; set; }
        public int? i_LiberacionUsuario { get; set; }
        public DateTime? t_FechaLiberacion { get; set; }
        public DateTime? t_FechaCaducidad { get; set; }
        public int i_SolicitarNroLoteIngreso { get; set; }
        public int i_SolicitarNroSerieIngreso { get; set; }
        public int i_SolicitaOrdenProduccionIngreso { get; set; }
        public int i_SolicitarNroSerieSalida { get; set; }
        public int i_SolicitarNroLoteSalida { get; set; }
        public int i_SolicitaOrdenProduccionSalida { get; set; }





        public string v_NroSerie { get; set; }
        public string v_NroLote { get; set; }
        public string v_NroPedido { get; set; }

    }
    public class PedidosVentas
    {
        public string SeriePedido { get; set; }
        public string CorrelativoPedido { get; set; }
        public string TodoNumeroPeedido { get; set; }
        public string NroFactura { get; set; }
        public string TipoDocumento { get; set; }
        public DateTime FechaEmisionDocVenta { get; set; }
        public string ProductoDetalle { get; set; }
        public decimal CantidadVendida { get; set; }
        public string UnidadMedidaVenta { get; set; }
        public string UM { get; set; }
        public int i_IdTipoDocumento { get; set; }
        public int i_IdTipoDocumentoReferencia { get; set; }
        public string v_NumeroDocumentoReferencia { get; set; }

    }

    public partial class pedidodetalleDto
    {
        public decimal CantidadFacturada { get; set; }
    }

    public partial class cajachicaDto
    {
        public string NroRegistro { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string TipoDocumento { get; set; }
        public string Estado { get; set; }
        public string Responsable { get; set; }
        public string v_IdVendedor { get; set; }
    }
    public partial class cajachicadetalleDto
    {

        public string CodigoAnexo { get; set; }
        public int i_RequiereAnexo { get; set; }
        public int i_RequiereTipoDocumento { get; set; }
        public int i_RequiereNumeroDocumento { get; set; }
    }


}
