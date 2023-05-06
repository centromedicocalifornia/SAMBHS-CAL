using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
  public partial  class nbs_ordentrabajoDto
    {
     public  string RucCliente { get; set; }
     public string DireccionCliente { get; set; }
     public string RazonSocialCliente { get; set; }
     public string Responsable { get; set; }
     public string UsuarioCreacion { get; set; }
     public string UsuarioModificacion { get; set; }
     public string CodigoCliente { get; set; }
     public int UsadoFUF { get; set; }
     public string NroFUF { get; set; }
   
    }
}
