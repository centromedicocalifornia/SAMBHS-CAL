using System.Globalization;

namespace SAMBHS.Common.BE
{
    public partial class planillavaloresproyeccionquintaDto
    {
        public string NombreMes
        {
            get
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i_IdMes ?? 1);
            }
        }
    }
}
