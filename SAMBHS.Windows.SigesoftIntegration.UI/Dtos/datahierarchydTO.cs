using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class datahierarchydTO
    {
        public int i_ItemId { get; set; }
        public string v_Value1 { get; set; }
        public string v_Value2 { get; set; }
        public int? i_ParentItemId { get; set; }
    }
}
