using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class carteraclienteDto
    {
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_CodigoCliente { get; set; }
        public string v_ClienteNombreRazonSocial { get; set; }
        public string v_TipoDocumento { get; set; }
        public string v_NroDocumento { get; set; }
        public string NombreVendedor { get; set; }
    }
}
