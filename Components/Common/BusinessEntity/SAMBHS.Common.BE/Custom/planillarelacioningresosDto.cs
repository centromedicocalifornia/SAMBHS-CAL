namespace SAMBHS.Common.BE
{
    public partial class planillarelacioningresosDto
    {
        public string NombreConcepto { get; set; }
        public string TipoPlanilla { get; set; }
        public string CentroCosto { get; set; }
        public string _Cuenta { get; set; }
    }

    public partial class planillarelacionesdescuentosafpDto
    {
        public string NombreConcepto { get; set; }
        public string RegimenLaboral { get; set; }
        public string _Cuenta { get; set; }
    }

    public partial class planillarelaciondescuentosDto
    {
        public string NombreConcepto { get; set; }
        public string TipoPlanilla { get; set; }
        public string _Cuenta { get; set; }
    }

    public partial class planillarelacionesnetopagarDto
    {
        public string TipoPlanilla { get; set; }
        public string _Cuenta { get; set; }
    }

    public partial class planillarelacionesaportacionesDto
    {
        public string NombreConcepto { get; set; }
        public string TipoPlanilla { get; set; }
        public string CentroCosto { get; set; }
        public string _CuentaA { get; set; }
        public string _CuentaB { get; set; }
    }
}
