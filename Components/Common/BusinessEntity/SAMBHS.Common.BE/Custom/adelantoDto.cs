
namespace SAMBHS.Common.BE
{
    public partial class adelantoDto
    {
        public string Moneda {
            get { return (i_IdMoneda ?? 1) == 1 ? "S/." : "US$"; }
        }
        public string v_UsuarioCreacion { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string NombreCliente { get; set; }
        public bool Seleccion { get; set; }
        public string NroRegistro { get; set; }
    }
}
