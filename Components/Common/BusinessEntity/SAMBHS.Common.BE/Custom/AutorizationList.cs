using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public class AutorizationList
    {
        public string V_Description { get; set; }
        public int? I_ParentId { get; set; }
        public int I_ApplicationHierarchyId { get; set; }
        public string V_Form { get; set; }
        public int? I_ApplicationHierarchyTypeId { get; set; }
        public string V_RoleName { get; set; }
        public int? i_RoleId { get; set; }
        public int? i_Level { get; set; }
        public string v_Code { get; set; }
    }
}
