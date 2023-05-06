namespace SAMBHS.Common.BE
{
    public partial class planillavariablesingresosDto
    {
        public string CodigoConcepto { get; set; }
        public string DescripcionConcepto { get; set; }
    }

    public partial class planillavariablesdescuentosDto
    {
        public string CodigoConcepto { get; set; }
        public string DescripcionConcepto { get; set; }
    }

    public partial class planillavariablesaportacionesDto
    {
        public string CodigoConcepto { get; set; }
        public string DescripcionConcepto { get; set; }
        public bool Guardado { get; set; }
    }
}
