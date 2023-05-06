using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SAMBHS.Common.Resource
{
    // Clase entidad que contiene información de las tablas del sistema. Se utiliza para la sincronización.
    [DataContract()]
    public partial class TableInfoDto
    {
        [DataMember()]
        public Int32 i_TableInfoId { get; set; }

        [DataMember()]
        public String v_Table { get; set; }

        [DataMember()]
        public Nullable<Int32> i_Sync { get; set; }

        [DataMember()]
        public String v_SyncType { get; set; }      

        [DataMember()]
        public Nullable<Int32> i_TableId { get; set; }    

        [DataMember()]
        public String v_CreatedIn { get; set; }
    
    }
}
