using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
   public partial class systemuserDto  : ICloneable
    {  


       public string v_InsertUser { get; set; }
       public string v_UpdateUser { get; set; }
       public string v_PersonName { get; set; }
       public string NroDocumento { get; set; }

       public object Clone()
       {
           return MemberwiseClone();
       }
    }
}
