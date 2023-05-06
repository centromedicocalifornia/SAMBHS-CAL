using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteRegistroVentaContable
    {
        public string Correlativo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string CorrelativoDocumento { get; set; }
        public string NombreCliente { get; set; }
        public int DocIdentidad { get; set; }
        public string NroDocCliente { get; set; }
        public decimal Valor_Export { get; set; }
        public decimal Base_Impon_Oper_Grav { get; set; }
        public decimal Importe_Exonerada { get; set; }
        public decimal Importe_Inafecta { get; set; }
        public decimal Igv_Y_O_Ipm { get; set; }
        public decimal Total { get; set; }
        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
        public string NombreMoneda { get; set; }
        public int IdMoneda { get; set; }
        public string IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int IdTipoDocumento { get; set; }
        public decimal TipoCambio { get; set; }
        public string IgvNombre { get; set; }
        public string Documento { get; set; }
        public DateTime? FechaRegistroRef { get; set; }
        public string  TipoDocumentoRef { get; set; }
        public string SerieDocumentoRef { get; set; }
        public string CorrelativoDocumentoRef { get; set; }
        public string LlaveOrdenar { get; set; }
        public string GrupoLlave { get; set; }
        public string GrupoLlaveNombre { get; set; }
        public string IdCabecera { get; set; }
        public string IdDetalle { get; set; }
        public int i_idEstado { get; set; }
        public int TipoVenta { get; set; }
        public int TipoOperacionCabecera { get; set; }
        public int TipoOperacionGrilla { get; set; }
        public int TipoOperacionGrilla2Dig { get; set; }
        public decimal IgvCabecera { get; set; }
        public decimal ValorVentaDetalle { get; set; }
        public decimal PrecioVentaDetalle { get; set; }
        public decimal ValorVenta { get; set; }
        public int AnticipioDetalle { get; set; }
        public decimal IgvDetalle { get; set; }
        public int DocumentoInverso { get; set; }
       public decimal OtrosTributosC { get; set; }
        //public decimal OtrosTributosD { get; set; }
        public decimal ISC { get; set; }
        public int TipoOperacionNoGravadaGrilla { get; set; }
        public decimal Otros_Tributos_No_Forman_Parte_B { get; set; }
        public int Index { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int TipoPersona { get; set; }
        public int  idTipoDocumentoRef { get; set; }
        public string CorrelativoPle { get; set; }
        public string NroDocumentoAlPle { get; set; }
        public decimal ValorFobEmbarcado { get; set; }
       // public decimal TipoCambioCalculo { get; set; }
        public string NroRegistro { get; set; }
        public string NroDocAl { get; set; }
        public string SiglasMoneda { get; set; }
        public string IdCliente { get; set; }
        public int IntCorrelativoDocumento { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public decimal IscDetalle { get; set; }
        public string IdProductoDetalle { get; set; }
        public string Cuenta { get; set; }
        public double  Base_Impon_Oper_GravConsolidado { get; set; }
        public double ? TotalConsolidado { get; set; }
        public double Igv_Y_O_IpmConsolidado { get; set; }
        public string v_IdVenta { get; set; }
        
        public decimal Visa { get; set; }
        public decimal Mastercard { get; set; }
        public decimal Ncr { get; set; }
        public decimal Efectivo { get; set; }
        public decimal Vale {get ;set;}
        public string DocCobranza { get; set; }
        public string CuentaContableCobranza { get; set; }
        public decimal d_PrecioVenta { get; set; }
        public bool EsOtrosTributos { get; set; }
        public bool AfectoIgv { get; set; }
        public bool PrecioIncluyeIgv { get; set; }
        


    }




    public class CalculosVenta
    {

        public decimal PrecioVentaOM { get; set; }
        public decimal ValorVentaOM { get; set; }
        public decimal IgvOM { get; set; }
        public decimal PrecioUnitarioOM { get; set; }
        public decimal IscOM { get; set; }
        public decimal OtrosTributosOM { get; set; }
        public int AnticipioDetalle { get; set; }
        public int TipoOperacionGrilla2Dig { get; set; }



    }
    
    public class TipoOperacionSunat {

        public int ItemId { get; set; }
        public int CodigoSunat { get; set; }
    }
    public class FormaPagoCobranzas
    { 
       public decimal   Efectivo {get;set;}
       public decimal  Mastercard { get; set; }
       public decimal Visa { get; set; }
       public decimal  Monto { get; set; }
       public string  DocCobranza { get; set; }
       public decimal Vale { get; set; }
       public decimal Ncr { get; set; }
       public string v_IdVenta { get; set; }
      
       public string FormaPago { get; set; }
       public string CuentaContableCobranza { get; set; }
    
    }




    public class ReporteRegistroVentaConsolidado
    {
        public string Correlativo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string CorrelativoDocumento { get; set; }
        public string NombreCliente { get; set; }
        public int DocIdentidad { get; set; }
        public string NroDocCliente { get; set; }
        public double Valor_Export { get; set; }
        public double Base_Impon_Oper_Grav { get; set; }
        public double Importe_Exonerada { get; set; }
        public double Importe_Inafecta { get; set; }
        public double Igv_Y_O_Ipm { get; set; }
        public double Total { get; set; }
        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
        public string NombreMoneda { get; set; }
        public int IdMoneda { get; set; }
        public string IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int IdTipoDocumento { get; set; }
        public decimal TipoCambio { get; set; }
        public string IgvNombre { get; set; }
        public string Documento { get; set; }
        public DateTime? FechaRegistroRef { get; set; }
        public string TipoDocumentoRef { get; set; }
        public string SerieDocumentoRef { get; set; }
        public string CorrelativoDocumentoRef { get; set; }
        public string LlaveOrdenar { get; set; }
        public string GrupoLlave { get; set; }
        public string GrupoLlaveNombre { get; set; }
        public string IdCabecera { get; set; }
        public string IdDetalle { get; set; }
        public int i_idEstado { get; set; }
        public int TipoVenta { get; set; }
        public int TipoOperacionCabecera { get; set; }
        public int TipoOperacionGrilla { get; set; }
        public int TipoOperacionGrilla2Dig { get; set; }
        public decimal IgvCabecera { get; set; }
        public decimal ValorVentaDetalle { get; set; }
        public decimal ValorVenta { get; set; }
        public int AnticipioDetalle { get; set; }
        public decimal IgvDetalle { get; set; }
        public int DocumentoInverso { get; set; }
        public double OtrosTributosC { get; set; }
        public double OtrosTributosD { get; set; }
        public double ISC { get; set; }
        public int TipoOperacionNoGravadaGrilla { get; set; }
        public double Otros_Tributos_No_Forman_Parte_B { get; set; }
        public int Index { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int TipoPersona { get; set; }
        public int idTipoDocumentoRef { get; set; }
        public string CorrelativoPle { get; set; }
        public string NroDocumentoAlPle { get; set; }
        public double ValorFobEmbarcado { get; set; }
        // public decimal TipoCambioCalculo { get; set; }
        public string NroRegistro { get; set; }
        public string NroDocAl { get; set; }
        public string SiglasMoneda { get; set; }
        public string IdCliente { get; set; }
        public int IntCorrelativoDocumento { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public decimal IscDetalle { get; set; }
        public string IdProductoDetalle { get; set; }
        public string Cuenta { get; set; }

    }
}
