using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class nodeorganizationlocationwarehouseprofileDto
    {
        public Int32 i_NodeId { get; set; }

        
        public String v_OrganizationId { get; set; }

    
        public String v_LocationId { get; set; }

       
        public String v_WarehouseId { get; set; }

        public Nullable<Int32> i_IsDeleted { get; set; }
    }
}
