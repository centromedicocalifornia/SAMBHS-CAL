using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class Categoria
    {
        public string v_CategoryName { get; set; }

        public int? i_CategoryId { get; set; }
        public string v_ComponentName { get; set; }
        public string v_CodigoSegus { get; set; }

        public string v_ComponentId { get; set; }
        public string v_ServiceComponentId { get; set; }
        public string v_ServiceComponentStatusName { get; set; }
        public string v_QueueStatusName { get; set; }
        public int i_ServiceComponentStatusId { get; set; }
        public List<ComponentDetailList> Componentes { get; set; }

        public decimal r_Price { get; set; }
        public decimal d_SaldoPaciente { get; set; }
        public decimal d_SaldoAseguradora { get; set; }
    }
}
