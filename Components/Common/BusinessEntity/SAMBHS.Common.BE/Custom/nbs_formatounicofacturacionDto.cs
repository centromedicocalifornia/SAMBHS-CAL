using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class nbs_formatounicofacturacionDto
    {
        public string NroDocumentoCliente { get; set; }
        public string NroDocumentoClienteFacturacion { get; set; }
        public string NombreCliente { get; set; }
        public string NombreClienteFacturacion { get; set; }
        public string UsuarioResponsable { get; set; }
        public string UsuarioCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public string DireccionClienteFacturacion { get; set; }
        public int i_IdTipoDocumentoCliente { get; set; }
        public int i_FacturadoF { get; set; }
        public string NroDocumentoVenta { get; set; }
        public string v_IdFormatoUnicoFacturacionDetalle { get; set; }
    }
}
