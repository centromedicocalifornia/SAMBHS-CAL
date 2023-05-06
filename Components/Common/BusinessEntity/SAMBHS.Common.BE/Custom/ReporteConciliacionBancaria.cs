using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public class ReporteConciliacionBancaria
    {
       public string Comprobante { get;set; }
       public string Documento { get; set; }
       public DateTime?  Fecha { get; set; }
       public string Detalle { get; set; }
       public decimal Importe { get; set; }
       public string Auxiliar { get; set; }
       public decimal ValorAuxiliar { get; set; }
    }
}
