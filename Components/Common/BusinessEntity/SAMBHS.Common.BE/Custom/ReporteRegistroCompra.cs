using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
  public  class ReporteRegistroCompra
    {

      public string NombreEmpresaPropietaria { get; set; }
      public string RucEmpresaPropietaria { get; set; }
      public string IdProveedor { get; set; }
      public string NombreProveedor { get; set; }
      public string NroDocProveedor { get; set; }
      public decimal ValorVenta  { get; set; }
      public decimal ValorVentaD { get; set; }
      public int IdTipoDocumento { get; set; }
      public decimal TipoCambio{ get; set; }
      public int IdMoneda { get; set; }




    }
}
