namespace SAMBHS.Common.BE
{
    public class Anexo
    {
        public string IdAnexo { get; set; }
        public string NombresApellidos { get; set; }
        public string NroDocumento { get; set; }
        public string Codigo { get; set; }
        public string Flag { get; set; }
        public TipoAnexo Tipo {
            get
            {
                switch (Flag)
                {
                    case "C":
                        return TipoAnexo.Cliente;

                    case "V":
                        return TipoAnexo.Proveedor;

                    default:
                        return TipoAnexo.Trabajador;
                }
            }
        }
    }

    public enum TipoAnexo
    {
        Cliente,
        Proveedor,
        Trabajador,
        Vendedor,
    }
}
