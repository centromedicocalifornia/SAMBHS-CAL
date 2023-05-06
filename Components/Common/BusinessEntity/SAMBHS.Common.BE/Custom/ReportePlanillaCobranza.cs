using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public class ReportePlanillaCobranza :ICloneable 
    {

        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string NombreCliente { get; set; }
        public string Glosa { get; set; }
        public string Cheq { get; set; }
        public string Moneda { get; set; }
        public decimal? TotalFacturado { get; set; }
        public decimal? TotalFacturadoDolares { get; set; }
        public decimal? MontoPagado { get; set; }
        public decimal? MontoPagadoDolares { get; set; }
        public decimal? Saldo { get; set; }
        public decimal? SaldoDolares { get; set; }
        public int? idTipoDocumento { get; set; }
        public string MedioPago { get; set; }
        public int? MedioPagoCobranza { get; set; }
        public string v_IdCobranzaPendiente { get; set; }
        public decimal? MontoCobrar { get; set; }
        public decimal? MontoCobrarDolares { get; set; }
        public decimal Valor { get; set; }
        public decimal? IdTipoDocumentoCobranzaDetalle { get; set; }
        public string MonedaCobranza { get; set; }
        public string Grupollave { get; set; }
        public string NombreGrupo { get; set; }
        public string TipoDocumentoCobranza { get; set; }
        public string NroDocumentoCobranza { get; set; }
        public int IdTipoDocumentoReferenciaCobranzaDetalle { get; set; }
        public string NumeroDocumentoReferenciaCobranzaDetalle { get; set; }
        public string FormaPago { get; set; }
        public decimal? MontoPagadoF { get; set; }
        public decimal? MontoPagadoDolaresF{ get; set; }
        public string TipoDocumentoVenta { get; set; }
        public DateTime? FechaOrigen { get; set; }
        public string Vendedor { get; set; }
        public string UsuarioCreacion { get; set; }
        public decimal? Retencion { get; set; }
        public string MonedaRetencion { get; set; }
        public string DocumentoVenta { get; set; }
        public int UsadoDocumentoInverso { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }

    }
}
