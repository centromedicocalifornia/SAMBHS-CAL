using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class ReporteLibroBanco
    {

        public string Moneda { get; set; }
        public string NumeroCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public string Comprobante { get; set; }
        public string Documento { get; set; }
        public int iDia { get; set; }
        public string Dia { get; set; }
        public string Detalle { get; set; }
        public decimal? Debe { get; set; }
        public decimal? Haber { get; set; }
        public decimal? Saldo { get; set; }
        public decimal? SaldoSegunBanco { get; set; }
        public int NumeroCuentaInt { get; set; }
        public string v_IdTesoreria { get; set; }
        public string v_IdTesoreriaDetalle { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal SaldoFinal { get; set; }
    }
    public partial class CierreMensualBancos
    {

        public int Moneda { get; set; }
       
        public decimal d_Cambio { get; set; }
        public decimal d_Importe { get; set; }
      
        public string v_NroCuenta { get; set; }
    }

}
