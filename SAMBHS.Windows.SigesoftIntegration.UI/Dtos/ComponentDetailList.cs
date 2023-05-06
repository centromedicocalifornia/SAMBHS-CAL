using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class ComponentDetailList
    {
        public string v_ComponentId { get; set; }
        public string v_ComponentName { get; set; }
        public string v_ServiceComponentId { get; set; }
        public int i_PriceIsRecharged { get; set; }
        public string r_Price { get; set; }
        public string d_SaldoPaciente { get; set; }
        public string d_SaldoAseguradora { get; set; }
    }
}
