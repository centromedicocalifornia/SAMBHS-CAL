using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class EsoDto
    {
        public int EsoId { get; set; }
        public string Nombre { get; set; }
        public string Id { get; set; }

        public string Consultorio { get; set; }


    }

    public class EsoDtoProt
    {
        public int? GroupId { get; set; }
        public int? EsoId { get; set; }
        public string Nombre { get; set; }
        public string Consultorio { get; set; }
        public string Id { get; set; }

    }

    public class tipoServicioforServicio
    {
        public int TipoServicio { get; set; }
        public int Servicio { get; set; }

    }
}
