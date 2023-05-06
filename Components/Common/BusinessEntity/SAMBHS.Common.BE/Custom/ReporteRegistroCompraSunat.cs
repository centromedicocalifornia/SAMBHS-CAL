using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteRegistroCompraSunat
    {

        public string Correlativo { get; set; }
        
        public DateTime? FechaVencimiento { get; set; }
        public int IdTipoDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string aniovencimiento { get; set; }
        public string CorrelativoDocumento { get; set; }
        public string NombreProveedor { get; set; }
        public int? DocIdentidad { get; set; }
        public string NroDocProveedor { get; set; }
        public decimal? TipoCambio { get; set; }
        public decimal? BaseImponible1 { get; set; }
        public decimal? Igv1 { get; set; }
        public decimal? BaseImponible2 { get; set; }
        public decimal? Igv2 { get; set; }
        public decimal? BaseImponible3 { get; set; }
        public decimal? Igv3 { get; set; }
        public decimal? ValorAdquisiciones { get; set; }
        public decimal? Total { get; set; }
        public string LlaveOrdenar { get; set; }
        public DateTime? FechaRegistroRef { get; set; }
        public int? IdTipoDocumentoRef { get; set; }
        public string TipoDocumentoRef { get; set; }
        public string SerieDocumentoRef { get; set; }
        public string CorrelativoDocumentoRef { get; set; }
        public string NroCPagoNoDomiciliario { get; set; }
        public DateTime FechaEmision { get; set; }
        public string IgvNombre { get; set; }
        public string TipoCompra { get; set; }
        public string CompraGeneradaImportacion { get; set; }
        public string NumeroRegistro { get; set; }
        public DateTime AnioAduanero { get; set; }
        public int? Estado { get; set; }
        public int? DestinoCabecera { get; set; }
        public int? DestinoGrilla { get; set; }
        public int? idMoneda { get; set; }
        public decimal? d_ValorVenta { get; set; }
        public string idCabecera { get; set; }
        public int anticipoDetalle { get; set; }
        public decimal d_Igv { get; set; }
        public int? TipoDocGenerado { get; set; }
        public string serieDocumentoGenerado { get; set; }
        public string correlativoDocumentoGenerado { get; set; }
        public string porcentajeIgv { get; set; }
        public int? monedaGenerado { get; set; }
        public decimal? TotalSolesGenerado { get; set; }
        public decimal? TotalDolaresGenerado { get; set; }
        public decimal? d_ValorVentaDetalle { get; set; }
        public decimal? d_IgvDetalle { get; set; }
        public decimal? d_PrecioVentaDetalle { get; set; }
        public int Index { get; set; }
        public decimal? TotalFinal { get; set; }
        public decimal? Isc { get; set; }
        public int DocumentoInverso { get; set; }
        public decimal? OtroTributos { get; set; }
        public int? TipoPersona { get; set; }
        public DateTime? FechaDetraccion { get; set; }
        public string CorrelativoPle { get; set; }
        public string GrupoLlave { get; set; }
        public string GrupoLlave2 { get; set; }
        public string TotalDocumentos { get; set; }
        public string SiglasMoneda { get; set; }
        public string TipoBien { get; set; }
        public int IdPais { get; set; }
        public string DobleTributacion { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public int EsDetraccion { get; set; }
        public string  CodigoDetraccion { get; set; }
        public int iCodigoDetraccion { get; set; }
        public string NumeroDetraccion { get; set; }
        public string Glosa { get; set; }
        public string CentroCosto { get; set; }
        public string NroCuenta { get; set; }
        public string  sFechaEmision { get; set; }
        public bool AfectoIgv { get; set; }
        public bool PrecioIncluyeIgv { get; set; }
        public int i_AplicarRectificacion { get; set; }
        public DateTime t_FechaRegistro { get; set; }
    
    }


    public class LibroElectronicoCompras

    {

        public string Columna1 { get; set; }
        public string Columna2 { get; set; }
        public string Columna3 { get; set; }




        public string Columna4 { get; set; }
        public string Columna5 { get; set; }
        public string Columna6 { get; set; }

        public string Columna7 { get; set; }
        public string Columna8 { get; set; }
        public string Columna9 { get; set; }
        public string Columna10 { get; set; }
        public string Columna11{ get; set; }
        public string Columna12 { get; set; }
        public string Columna13{ get; set; }
        public string Columna14 { get; set; }
        public string Columna15{ get; set; }

        public string Columna16 { get; set; }
        public string Columna17 { get; set; }
        public string Columna18 { get; set; }

        public string Columna19 { get; set; }
        public string Columna20 { get; set; }
        public string Columna21 { get; set; }


        public string Columna22 { get; set; }
        public string Columna23 { get; set; }
        public string Columna24 { get; set; }
        public string Columna25 { get; set; }
        public string Columna26 { get; set; }
        public string Columna27 { get; set; }

        public string Columna28 { get; set; }
        public string Columna29 { get; set; }
        public string Columna30 { get; set; }
        public string Columna31 { get; set; }
        public string Columna32 { get; set; }
        public string Columna33 { get; set; }


        public string Columna34 { get; set; }
        public string Columna35 { get; set; }
        public string Columna36 { get; set; }
        public string Columna37 { get; set; }
        public string Columna38 { get; set; }
        public string Columna39 { get; set; }

        public string Columna40 { get; set; }
        public string Columna41 { get; set; }
       

    
    }



    public class LibroElectronicoVentas
    {

        public string Columna1 { get; set; }
        public string Columna2 { get; set; }
        public string Columna3 { get; set; }




        public string Columna4 { get; set; }
        public string Columna5 { get; set; }
        public string Columna6 { get; set; }

        public string Columna7 { get; set; }
        public string Columna8 { get; set; }
        public string Columna9 { get; set; }
        public string Columna10 { get; set; }
        public string Columna11 { get; set; }
        public string Columna12 { get; set; }
        public string Columna13 { get; set; }
        public string Columna14 { get; set; }
        public string Columna15 { get; set; }

        public string Columna16 { get; set; }
        public string Columna17 { get; set; }
        public string Columna18 { get; set; }

        public string Columna19 { get; set; }
        public string Columna20 { get; set; }
        public string Columna21 { get; set; }


        public string Columna22 { get; set; }
        public string Columna23 { get; set; }
        public string Columna24 { get; set; }
        public string Columna25 { get; set; }
        public string Columna26 { get; set; }
        public string Columna27 { get; set; }

        public string Columna28 { get; set; }
        public string Columna29 { get; set; }
        public string Columna30 { get; set; }
        public string Columna31 { get; set; }
        public string Columna32 { get; set; }
        public string Columna33 { get; set; }
        public string Columna34 { get; set; }


       



    }
    public class Calculos
    {

        public decimal PrecioVentaOM { get; set; }
        public decimal ValorVentaOM { get; set;}
        public decimal IgvOM { get; set; }
        public decimal PrecioUnitarioOM { get; set; }
        public decimal IscOM { get; set; }
        public decimal OtrosTributosOM { get; set; }
        public int Anticipio { get; set; }
        public int DestinoGrillaDetalle { get; set; }
        
    
    
    }
    public class ReportePdtLiquidacionCompras
    {

        public int TipoDocumentoProv { get; set; }
        public string NumeroDocumentoProv { get; set; }

        public string ApePaternoProv { get; set; }
        public string ApeMaternoProv { get; set; }
        public string Nombre { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaRetencion { get; set; }
        public decimal TotalOperacion { get; set; }
        public string TipoOperacion { get; set; }
        public string NumeroRegistro { get; set; }
        public string CodigoProv { get; set; }

    }


    public class ReportePdtRetencionIgv
    {

        public int TipoDocumentoProv { get; set; }
        public string NumeroDocumentoProv { get; set; }
        public int TipoPersona { get; set; }
        public int TipoDocumento { get; set; }
        public string ApePaternoProv { get; set; }
        public string ApeMaternoProv { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaRetencion { get; set; }
        public decimal TotalOperacion { get; set; }
        public string TipoOperacion { get; set; }
        public string NumeroRegistro { get; set; }
        public string CodigoProv { get; set; }
        public decimal RentaNeta { get; set; }

    }

    public class RegistroRetenciones
    {

        public string NroRegistro { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TipoNumeroDocumento { get; set; }
        public string RucProveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public string NroDetraccion { get; set; }
        public DateTime FechaDetraccion { get; set; }
        public decimal ImporteDetraccionSoles { get; set; }
        public decimal ImporteDetraccionDolares { get; set; }
        public string Moneda { get; set; }
        public string IdCompra { get; set; }


    }

    public class ReporteCompraDaotAnalitico
    {
        public string  proveedor { get; set; }
        public string documento { get; set; }
        public string fecha { get; set; }
        public string  nroRegistro { get; set; }
        public decimal ValorVentaSoles { get; set; }
        public decimal IgvSoles { get; set; }
        public decimal TotalSoles { get; set; }
        public decimal ValorVentaDolares { get; set; }
        public decimal IgvDolares { get; set; }
        public decimal TotalDolares { get; set; }
        public string Grupo { get; set; }
        public string v_IdCompra { get; set; }
        public string v_IdProveedor { get; set; }
        public string RucProveedor { get; set; }
        public string PrimerNombre { get; set; }
        public string ApeMaterno { get; set; }
        public string ApePaterno { get; set; }
        public string RazonSocial { get; set; }
       public string SegundoNombre { get; set; }
       public int TipoPersona { get;set ; }
       public int TipoDocumentoProveedor { get; set; }
        public DateTime dFecha { get; set; }

    }

}
