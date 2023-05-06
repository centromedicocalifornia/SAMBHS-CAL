using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public class ReporteRegistroVentaClienteAnalitico
    {

        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
        public string NombreMoneda { get; set; }
        public int IdMoneda { get; set; }
        public string NombreAlmacen { get; set; }
        public string NombreCliente { get; set; }
        public string NroDocCliente { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string NombreVendedor { get; set; }
        public string IdVendedor { get; set; }
        public string IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal CantidadDetalle { get; set; }
        public decimal PrecioDetalle { get; set; }
        public decimal ValorDetalle { get; set; }
        public decimal ValorVentaDetalle { get; set; }
        public decimal DescuentoDetalle { get; set; }
        public decimal PrecioVentaDetalle { get; set; }
        public string UnidadMedida { get; set; }
        public int IdTipoDocumento { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal IgvDetalle { get; set; }
        public decimal PrecioDetalleD { get; set; }
        public decimal ValorDetalleD { get; set; }
        public decimal ValorVentaDetalleD { get; set; }
        public decimal DescuentoDetalleD { get; set; }
        public decimal PrecioVentaDetalleD { get; set; }
        public decimal IgvDetalleD { get; set; }
        public string CorrelativoDocumento { get; set; }
        public string Grupollave { get; set; }
        public int i_Anticipio { get; set; }
        public string CondicionVenta { get; set; }
        public string DocumentoContable { get; set; }
       

    }
   public class ReporteVentaClienteMensual
   {

       public string CodigoCliente { get; set; }
       public string NombreCliente { get; set; }
       public string MesRegistro { get; set; }
       public string IdCliente { get; set; }
       public decimal PrecioVenta { get; set; }
       public string Departamento { get; set; }
       public decimal  MesEnero { get; set; }
       public decimal MesFebrero { get; set; }
       public decimal MesMarzo { get; set; }
       public decimal MesAbril { get; set; }
       public decimal MesMayo { get; set; }
       public decimal MesJunio { get; set; }
       public decimal MesJulio { get; set; }
       public decimal MesAgosto { get; set; }
       public decimal MesSetiembre { get; set; }
       public decimal MesOctubre { get; set; }
       public decimal MesNoviembre { get; set; }
       public decimal MesDiciembre { get; set; }
       public decimal Total { get; set; }
       public string Grupo { get; set; }
       public string IdVenta { get; set; }
       public int DocInterno { get; set; }
       public int DocContable{ get; set; }
   
   }
}
