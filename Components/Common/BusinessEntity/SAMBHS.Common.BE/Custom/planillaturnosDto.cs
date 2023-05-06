using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class planillaturnosdetalleDto
    {
        public bool Asistencia { get; set; }    
        public TimeSpan HorasTrabajadas { get; set; }
        public string DiaSemana
        {
            get
            {
                var cult = new CultureInfo("es-ES");
                return cult.DateTimeFormat.DayNames[i_IdDia ?? 1];
            }
        }
    }

    public partial class planillaturnosDto
    {
        public List<planillaturnosdetalleDto> Detalle { get; set; }
    }
}
