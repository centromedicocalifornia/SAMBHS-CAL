using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteComprasLineaAnalitico
    {
        public string NumeroRegistro { get; set; }
        public string Producto { get; set; }
        public DateTime Fecha { get; set; }
        public string NroDocumento { get; set; }
        public string Cuenta { get; set; }
        public decimal? Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string NroPedido { get; set; }
        public decimal? VentaSoles { get; set; }
        public decimal? VentasDolares { get; set; }
        public string GrupoLLave { get; set; }
        public string CodigoProducto { get; set; }
        public string CodigoProveedor { get; set; }
        public int IdAlmacen { get; set; }
        public int IdEstablecimiento { get; set; }
        public int pIntTipoCompra { get; set; }
        public string pstrAlmacen { get; set; }
        public string pstrTipoCompra { get; set; }
        public string  pIntLinea { get; set; }
        public string Linea { get; set; }
        public string Marca { get; set; }
        public string v_Marca { get; set; }
        public decimal? VentaSolesCalculos { get; set; }
        public decimal? VentasDolaresCalculoss { get; set; }
        public int TipoDocumento { get; set; }
        public bool EsDocInverso { get; set; }
    }

    public class ReporteComprasGastoAnalitico
    {
        public string GrupoLLave1 { get; set; }
        public string GrupoLLave2 { get; set; }
        public string NroRegistro { get; set; }
        public string sFecha { get; set; }
        public string Documento { get; set; }
        public string Cuenta { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string NroPedido { get; set; }
        public decimal VentaSoles { get; set; }
        public decimal VentasDolares { get; set; }
        public string Articulo { get; set; }
        public string IdCompra { get; set; }
        public int TipoDocumento { get; set; }
    
    
    }
}
