using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class ListaLineas
    {
        public string v_IdLinea { get; set; }
        public string v_Nombre { get; set; }
    }

    public class ServiceComponentDat
    {
        public string ServiceComponent { get; set; }
        public int? MedicoTratante { get; set; }
        public int? MedicoIndica { get; set; }
    }
}
