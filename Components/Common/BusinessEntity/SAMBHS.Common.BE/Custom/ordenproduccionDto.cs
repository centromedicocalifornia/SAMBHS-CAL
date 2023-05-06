using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public partial  class ordenproduccionDto
    {
       public string NombreProducto { get; set; }
       public string CodigoProducto { get; set; }
       public string UsuarioCreacion { get; set; }
       public string UsuarioModificacion { get; set; }
       public string NroRegistro { get; set; }
    }
}
