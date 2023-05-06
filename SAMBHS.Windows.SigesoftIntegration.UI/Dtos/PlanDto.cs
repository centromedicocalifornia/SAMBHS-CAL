using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class PlanDto
    {
        public int i_PlanId { get; set; }  
        public string v_OrganizationSeguroId { get; set; }
        public string v_ProtocoloId { get; set; }
        public string v_IdUnidadProductiva { get; set; }  
        public int i_EsDeducible { get; set; }  
        public int i_EsCoaseguro { get; set; }
        public decimal d_Importe { get; set; }      
    }
}
