using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   

    public partial class conceptoDto
    {
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_NombreArea { get; set; }
        public string CuentaVenta { get; set; }
    }

    public partial class conceptoscajachicaDto
    {
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string NombreCuenta { get; set; }

    }
}
