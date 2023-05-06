using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public class ReporteBalanceMensual
    {
       public string pstrCuenta { get; set; }
       public string pstrDenominacion { get; set; }
       public decimal debeSaldoAnterior { get; set; }
       public decimal haberSaldoAnterior { get; set; }
       public decimal debeMovimientoMes { get; set; }
       public decimal haberMovimientoMes { get; set; }
       public decimal debeSumaAcumulada { get; set; }
       public decimal haberSumaAcumulada { get; set; }
       public decimal deudorSaldosActuales { get; set; }
       public decimal acreedorSaldosActuales { get; set; }
       public string mes { get; set; }
       public decimal ImporteDebeSoles { get; set; }
       public decimal ImporteDebeDolares { get; set; }
       public decimal ImporteHaberDolares { get; set; }
       public decimal ImporteHaberSoles { get; set; }
       public int imputable { get; set; }
       public decimal d_Importe { get; set; }
       public decimal d_Cambio { get; set; }
       public string Grupo1 { get; set; }
       public string Grupo2 { get; set; }
     
   }

}
