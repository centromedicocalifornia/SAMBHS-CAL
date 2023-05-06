using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class SystemUser
    {
        public int i_SystemUserId { get; set; }
        public string v_PersonId { get; set; }
        public string v_UserName { get; set; }
        public string v_Password { get; set; }
        public string v_SecretQuestion { get; set; }
        public int i_IsDeleted { get; set; }
        public int i_InsertUserId { get; set; }
        public DateTime d_InsertDate { get; set; }
        public int i_UpdateUserId { get; set; }
        public DateTime d_UpdateDate { get; set; }
        public int i_SystemUserTypeId { get; set; }
        public string i_RolVentaId { get; set; }
        public string v_ComentaryUpdate { get; set; }
    }
}
