using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class ventaDto
    {
        public string NombreCliente { get; set; }
        public string CorreoCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string NroDocCliente { get; set; }
        public string NroRegistro { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string Moneda { get; set; }
        public string Vendedor { get; set; }
        public string Direccion { get; set; }
        public string CondicionPago { get; set; }
        public decimal ValorVenta { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public decimal Descuento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public List<ventadetalleDto> _ventadetalleDto { get; set; }
        public string CodigoArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Unidad { get; set; }
        public decimal d_Igvdetalle { get; set; }
        public decimal PrecioVenta { get; set; }
        public string NombreEmpresaPropietaria { get; set; }
        public decimal SaldoPendiente { get; set; }
        public bool FlagPendientePorCobrar { get; set; }
        public string RegistroKey { get; set; }
        public decimal Saldo { get; set; }
        public int TipoPersonaCliente { get; set; }
        public string v_MethodPayment { get; set; }
        public int? i_IdTipoIdentificacion { get; set; }
        public string v_NroFUF { get; set; }
        public string TieneGRM { get; set; }
        public string Origen { get; set; }
        public string v_Ticket { get; set; }
        public int? Almacen { get; set; }
        public IEnumerable<string> DsKardexs { get; set; }
        public string Kardexs { get; set; }
    }

    public partial class CompiladoResult
    {
        public string CodigoInterno { get; set; }
        public decimal? Empaque { get; set; }
        public string UMEmpaque { get; set; }
        public int? i_EsServicio { get; set; }
        public int? i_EsAfectoDetraccion { get; set; }
        public int? i_EsNombreEditable { get; set; }
        public string NombreCuenta { get; set; }
        public string Descripcion { get; set; }
        public int? i_EsAfectoPercepcion { get; set; }
        public decimal? d_TasaPercepcion { get; set; }
        public int? i_IdUnidadMedida { get; set; }

    }

    public class MonitoreoVentasDto
    {
        public string v_IdVenta { get; set; }
        public string Almacen { get; set; }
        public string Vendedor { get; set; }
        public DateTime? Fecha { get; set; }
        public string Documento { get; set; }
        public string Serie { get; set; }
        public string Correlativo { get; set; }
        public string Condicion { get; set; }
        public string Impresion { get; set; }
        public string Moneda { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal? Total { get; set; }
        public int i_IdEstablecimiento { get; set; }
        public string v_IdVendedor { get; set; }
    }

    public class VentaDetalles
    {

        public string NombreCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string NroDocCliente { get; set; }
        public string IdCliente { get; set; }
        public string Direccion { get; set; }
        public string CodigoProducto { get; set; }


    }
    public partial class nbs_ventakardexDto
    {
       public  int Index { get; set; }
    }
}
