using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class guiaremisionDto
    {

        public string NombreTransportista {get; set ;}
        public string RucTransportista { get; set; }
        public string CodTransportista { get; set; }
        public string MarcaTracto { get; set; }
        public string PlacaTracto { get; set; }
        public string ConstInscripcion { get; set; }
        public string NombreChofer { get; set; }
        public string NumLicencia { get; set; }
        public string NombreAgencia { get; set; }
        public string DireccionAgencia { get; set; }
        public string RucAgencia { get; set; }
        public string NombreCliente {get;set;}
        public string CodigoCliente {get;set;}
        public string NroDocCliente { get; set; }

        #region Bandeja
        public string v_NumeroRegistro { get; set; }
        public string v_Documento { get; set; }
        public string AgenciaTransportes { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_Guia { get; set; }
        public string TipoDocumento { get;set; }
        public string TipoDocumentoReferencia { get; set; }
        public string Origen { get; set; }
        public string AlmacenDestino { get; set; }
        public string Establecimiento { get; set; }
        public string Detalles { get; set; }
        #endregion

        public string Moneda { get; set; }
    }
    public class CantidadesGuiaRemision
    {
        public decimal CantidadAdministrativa { get; set; }
        public decimal CantidadEmpaqueAdministrativa { get; set; }
    
    }
}
