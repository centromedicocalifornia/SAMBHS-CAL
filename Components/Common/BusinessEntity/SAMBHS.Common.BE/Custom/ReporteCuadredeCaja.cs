﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
  public  class ReporteCuadredeCaja
    {
      public string Documento { get; set; }
      public string CondicionPago { get; set; }
      public decimal TotalS { get; set; }
      public decimal TotalD { get; set; }
      public decimal TotalGS {  get; set; }
      public int? i_ClienteEsAgente { get; set; }
      public int? i_IdTipoDocumento{ get; set; }


    }
}
