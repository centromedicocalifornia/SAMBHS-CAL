using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class familymedicalantecedentsDto
    {
        public String v_FamilyMedicalAntecedentsId { get; set; }

        public String v_PersonId { get; set; }

        public String v_DiseasesId { get; set; }

        public Nullable<Int32> i_TypeFamilyId { get; set; }

        public String v_Comment { get; set; }

    }
}
