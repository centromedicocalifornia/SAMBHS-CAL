using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public class ReporteVentaCobranza
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
        public decimal ? MontoCobrar { get; set; }
        public decimal? MontoCobrarDolares { get; set; }
        public decimal Valor { get; set; }
        public decimal? IdTipoDocumentoCobranzaDetalle { get; set; }
        public string MonedaCobranza { get; set; }
        public int? i_FormaPagoCobranzaDetalle { get; set; }
        public string v_FormaPagoCobranzaDetalle { get; set; }
        public decimal? MontoSolesDia { get; set; }
        public decimal? MontoSolesOtroDia { get; set; }
        public decimal? MontoDolaresDia { get; set; }
        public decimal? MontoDolaresOtroDia { get; set; }
        public string LineaResumen { get; set; }
        public string  NombreFormaPagoResumen { get; set; }
        public decimal? MontoSolesResumen { get; set; }
        public decimal? MontoDolaresResumen { get; set; }
        public decimal? MontoSolesOtroDiaResumen { get; set; }
        public decimal? MontoDolaresOtroDiaResumen { get; set; }
        public decimal? MontoSolesDocumentoResumen { get; set; }
        public decimal? MontoDolaresDocumentoResumen { get; set; }
        public decimal? MontoSolesOtroDiaDocumentoResumen { get; set; }
        public decimal? MontoDolaresOtroDiaDocumentoResumen { get; set; }
        public string FormatoDias { get; set; }
        public string FormatoMonedas { get; set; }
        public DateTime FechaInsercion { get; set; }
        public string Grupo { get; set; }
        public string TipNumDocumento { get; set; }
       
    }
   public class ComboAgrupado {

       public string  Value { get; set; }
       public string Nombre { get; set; }
     
   }
}
