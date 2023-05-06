using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
  public  class ReporteDocumentoPedido
    {
        public string RucCliente { get; set; }
        public string DescripcionUnidadMedida { get; set; }
        //public string CodCliente { get; set; }
        public string RazonSocial { get; set; }
        public string Direccion { get; set; }
        public string Glosa { get; set; }
        public string CondicionPago { get; set; }
        public string Unidad { get; set; }
        public string Descripcion { get; set; }
        public string CodigoArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal { get; set; }
        public decimal Valor { get; set; }
        public string Moneda { get; set; }
        public string MonedaSiglas { get; set; }
        public int i_idMoneda { get;set ; }
        public int  i_NroDias { get; set; }
        public string  StrNroDias { get; set; }
        public string  Igv { get; set; }
        public string v_IdPedidoDetalle { get; set; }

        #region Bandeja - Administrar Pedidos
        public string NroRegistro { get; set; }
        public string Documento { get; set;  }
        public string TipoDocumento { get; set; }
        //public string CodCliente { get; set; }
        public string NombreCliente{get;set;}
        #endregion 

        #region Bandeja - Consulta Pedidos
        public string NumeroPedido { get; set; }
        public string Vendedor { get; set; }
        public string CodigoCliente { get; set; }
        public string Cliente { get; set; }
        //public string NombreEmpresaPropietaria { get; set; }
      #endregion 
       public DateTime t_FechaEmision  { get; set; }
       public string v_IdPedido  { get; set; }
       public string   v_Periodo  { get; set; }
       public string v_Mes   { get; set; }
       public string   v_Correlativo { get; set; }
       public string  v_SerieDocumento { get; set; }
       public string  v_CorrelativoDocumento { get; set; }
       public int i_IdTipoDocumento  { get; set; }
       public DateTime  t_FechaVencimiento { get; set; }
       public int i_IdEstado  { get; set; }
       public DateTime   t_InsertaFecha { get; set; }
       public int  i_ActualizaIdUsuario { get; set; }
       public string  v_IdCliente { get; set; }
       public Decimal   d_PrecioVenta  { get; set; }
       public string TelefonoCliente { get; set; }
       public string UnidadSiglas { get; set; }
       public string v_Descuento { get; set; }
       public decimal ValorVenta { get; set; }
       public decimal IgvVenta { get; set; }
       public string  Item { get; set; }
       public decimal ValorVentaDetalle { get; set; }
       
    } 
    
}
