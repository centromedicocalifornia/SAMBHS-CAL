using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class componentDto
    {
        public String v_ComponentId { get; set; }
        public String v_Name { get; set; }
        public Nullable<Int32> i_CategoryId { get; set; }
        public Nullable<Single> r_BasePrice { get; set; }
        public Nullable<Single> r_PriceSegus { get; set; }
        public Nullable<Int32> i_DiagnosableId { get; set; }
        public Nullable<Int32> i_IsApprovedId { get; set; }
        public Nullable<Int32> i_ComponentTypeId { get; set; }
        public Nullable<Int32> i_UIIsVisibleId { get; set; }
        public Nullable<Int32> i_UIIndex { get; set; }
        public Nullable<Int32> i_ValidInDays { get; set; }
        public Nullable<Int32> i_IsDeleted { get; set; }
        public string v_IdUnidadProductiva { get; set; }
        public string v_CodigoSegus { get; set; }
    }
}
