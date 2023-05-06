using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class diarioDto : ICloneable
    {
        public string v_UsuarioModificacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string NroRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string TipoComprobante { get; set; }
        public string RegistroKey { get; set; }
       

        public List<diariodetalleDto> DiarioDetallePersonalizado { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class CuentaOrigenDto
    {
        public string NroCuenta { get; set; }
        public string CentroCostos { get; set; }
    }
}
