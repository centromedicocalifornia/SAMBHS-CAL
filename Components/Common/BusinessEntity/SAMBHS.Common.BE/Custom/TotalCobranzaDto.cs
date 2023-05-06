using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class TotalCobranzaDto
    {
       
        public decimal? FacturasDia { get; set; }
        public decimal? BoletasDia { get; set; }
        public decimal? NotasCreditoDia { get; set; }
        public decimal? NotasDebitoDia { get; set; }
        public decimal? FacturasOtroDia { get; set; }
        public decimal? BoletasOtroDia { get; set; }
        public decimal? NotasCreditoOtroDia { get; set; }
        public decimal? NotasDebitoOtroDia { get; set; }
        public decimal? FacturasDiaDolares { get; set; }
        public decimal? BoletasDiaDolares { get; set; }
        public decimal? NotasCreditoDiaDolares { get; set; }
        public decimal? NotasDebitoDiaDolares { get; set; }
        public decimal? FacturasOtroDiaDolares { get; set; }
        public decimal? BoletasOtroDiaDolares { get; set; }
        public decimal? NotasCreditoOtroDiaDolares { get; set; }
        public decimal? NotasDebitoOtroDiaDolares { get; set; }
        public decimal? MontoSolesDia { get; set; }
        public decimal? MontoSolesOtroDia { get; set; }
        public decimal? MontoDolaresDia { get; set; }
        public decimal? MontoDolaresOtroDia { get; set; }
        public string MonedaCobranza { get; set; }
        public int? i_FormaPagoCobranzaDetalle { get; set; }
        public string v_FormaPagoCobranzaDetalle { get; set; }
        public int? idTipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Grupo { get; set; }




    }
}
