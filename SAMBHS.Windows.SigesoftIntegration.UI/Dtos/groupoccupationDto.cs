using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class groupoccupationDto
    {
        public String v_GroupOccupationId { get; set; }

        public String v_LocationId { get; set; }

        public String v_Name { get; set; }

        public Nullable<Int32> i_IsDeleted { get; set; }
    }
}
