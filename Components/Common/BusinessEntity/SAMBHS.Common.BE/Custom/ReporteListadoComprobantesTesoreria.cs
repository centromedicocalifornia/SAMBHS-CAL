using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteListadoComprobantesTesoreria
    {

        public string NumeroComprobanteTesoreria { get; set; }
        public string TipoMovimiento { get; set; }
        public string NumeroyNombreBanco { get; set; }
        public string Banco { get; set; }
        public string Nombre { get; set; }
        public string Concepto { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroCheque { get; set; }
        public decimal? Importe { get; set; }
        public string Moneda { get; set; }
        public string Cuenta { get; set; }
        public string CentroCostos { get; set; }
        public int? PartidaCaja { get; set; }
        public string Detalle { get; set; }
        public decimal? MonedaExtranjera { get; set; }
        public decimal? TipoCambio { get; set; }
        public decimal? Debe { get; set; }
        public decimal? Haber { get; set; }
        public string DocumentoReferencia { get; set; }
        public string DebeString { get; set; }
        public string HaberString { get; set; }
        public string v_Naturaleza { get; set; }
        public int? i_IdTipoDocumento { get; set; }
        public int? i_IdTipoMovimiento { get; set; }
        public decimal? d_Importe { get; set; }
        public string NumeroDocumento { get; set; }
        public string v_IdTesoreria { get; set; }
        public string TotalLetras { get; set; }

    }

    public class ReporteRetenciones
    {
        public string Proveedor { get; set; }
        public string FechaTransaccion { get; set; }
        public DateTime  dFechaTransaccion { get; set; }
        public string TipoDocSustentatorio { get; set; }
        public string NumeroDocumentoSustentatorio { get; set; }
        public decimal ImporteDocumentoSustentatorio { get; set; }
        public string TipoTransaccion { get; set; }
        public int IdTipoTransaccion { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Saldo { get; set; }
        public decimal ImporteDocRetencion { get; set; }
        public string  TipoDocReferencia { get; set; }
        public string SerieDocReferencia { get; set; }
        public string CorrelativoDocReferencia { get; set; }

        public int TipoDocNotaCredito { get; set; }
        public string SerieDocNotaCredito { get; set; }
        public string CorrelativoDocNotaCredito { get; set; }

       
        public int IdTipoDocumentoSustentatorio { get; set; }
        public string ComprobanteRetencion { get; set; }
        public string IdProveedor { get; set; }
        public string IdCompra { get; set; }
        public string Anexo { get; set; }
        public int  IdTipoDocumentoAnexo { get; set; }
        public string NumeroDocumentoAnexo { get; set; }
        public string Llave { get; set; }
        public string LLaveReferencia { get; set; }
        public string RucProveedor { get; set; }
        public string NombresProveedor { get; set; }
        public string NroCompra { get; set; }
        public string NroRetencion { get; set; }
        public string NroTesoreria { get; set; }
        public string ApePaternoProveedor { get; set; }
        public string ApeMaternoProveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public string CorrelativoDocSustentatorio { get; set; }
        public string SerieDocSustentatorio { get; set; }
        public string  TipoDocumentoCompra { get; set; }
        public string SerieDocumentoCompra { get; set; }
        public string CorrelativoDocumentoCompra { get; set; }
        public string FechaEmisionDocumentoCompra { get; set; }
        public decimal ValorTotalDocumentoCompra { get; set; }
        public  string  SerieComprobanteRetencion { get; set; }
        public string CorrelativoComprobanteRetencion { get; set; }
        public string v_IdDocumentoRetencionDetalle { get; set; }
        public decimal ImporteTotalDocumentoSustentatorio { get; set; }
        public string v_IdDocumentoRetencion { get; set; }
        public string IdDiarioDetalle { get; set; }
        public string v_NroCuenta { get; set; }
       
       
        


    }
}
