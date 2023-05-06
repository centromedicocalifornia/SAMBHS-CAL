using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class clienteDto
    {
        /// <summary>
        /// Devuelve el apellido parterno, materno, primer nombre y razon social concatenados.
        /// </summary>
        public string NombreRazonSocial
        {
            get { return (v_ApePaterno + " " + v_ApeMaterno + " " + v_PrimerNombre + " " + v_RazonSocial).Trim(); }
        }

        public string TipoDocumento { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string Ubigeo {get;set ;}
    }

    public class clienteshortDto
    {
        public string NombreRazonSocial { get; set; }
        public string TipoDocumento { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_IdCliente { get; set; }
        public string v_CodCliente { get; set; }
        public string v_NroDocIdentificacion { get; set; }
        public int? i_IdTipoIdentificacion { get; set; }
        public int? i_IdTipoPersona { get; set; }
        public int? i_IdLista { get; set; }
        public int? i_IdSexo { get; set; }
        public int? i_IdPais { get; set; }
        public int? i_IdProvincia { get; set; }
        public int? i_IdDistrito { get; set; }
        public int? i_IdDepartamento { get; set; }
        public int? i_Nacionalidad { get; set; }
        public string v_FlagPantalla { get; set; }
        public string v_RazonSocial { get; set; }
        public string v_Correo { get; set; }
        public string v_TelefonoFijo { get; set; }
        public string v_TelefonoMovil { get; set; }
        public DateTime t_FechaNacimiento { get; set; }
        public DateTime? t_ActualizaFecha { get; set; }
        public DateTime? t_InsertaFecha { get; set; }
        public string v_PrimerNombre { get; set; }
        public string v_SegundoNombre { get; set; }
        public string v_ApePaterno { get; set; }
        public string v_ApeMaterno { get; set; }
        public string v_Direccion { get; set; }
        public string TipoDocumentoTrabajadores { get; set; }
        public int? i_ParameterId { get; set; }
        public int? i_Activo { get; set; }
        public int? i_EsProveedorServicios { get; set; }
        public int? i_IdDireccionCliente { get; set; }
  
    }

    public class ClienteDireccion
    {
        public string direccion { get; set; }
        public string idcliente { get; set; }
    }

    public partial class clientedireccionesDto
    {

        public string Zona { get; set; }
    }
    public partial class clienteDto
    {
        public int i_IdDireccionCliente { get; set; }
    }
    public partial class trabajadorDto
    {
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string Via { get; set; }
       
    
    }
    public partial class relacionusuarioclienteDto
    {
        public string Cliente { get; set; }
        public string CodigoCliente { get; set; }
        public string DireccionCliente { get; set; }
        public string User { get; set; }
    
    }
}
