using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class ServiceComponent
    {
        public string v_ComponentId { get; set; }
        public string v_Name { get; set; }
        public string v_Protocol { get; set; }
        public float r_Price { get; set; }
        public int i_MedicoTratanteId { get; set; }
        public int i_IsFact { get; set; }
    }
}
