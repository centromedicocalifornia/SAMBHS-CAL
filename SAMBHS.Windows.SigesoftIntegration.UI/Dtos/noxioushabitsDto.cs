using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class noxioushabitsDto
    {
        public String v_NoxiousHabitsId { get; set; }

        public String v_PersonId { get; set; }

        public Nullable<Int32> i_TypeHabitsId { get; set; }

        public String v_Frequency { get; set; }

        public String v_Comment { get; set; }

        public String v_DescriptionHabit { get; set; }

        public String v_DescriptionQuantity { get; set; }

    }
}
