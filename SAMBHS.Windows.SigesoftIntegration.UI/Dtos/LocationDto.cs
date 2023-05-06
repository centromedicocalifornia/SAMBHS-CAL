using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class LocationDto
    {
        public string LocationId { get; set; }
        public string Nombre { get; set; }
        public string v_OrganizationId { get; set; }
        public string v_Name { get; set; }
        public Nullable<Int32> i_IsDeleted { get; set; }

    }
}
