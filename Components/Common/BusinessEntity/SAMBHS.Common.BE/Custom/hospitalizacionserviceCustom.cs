﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class hospitalizacionserviceCustom
    {
        public string v_HospitalizacionServiceId { get; set; }
        public string v_HopitalizacionId { get; set; }
        public string v_ServiceId { get; set; }
        public int? i_IsDeleted { get; set; }
        public int? i_InsertUserId { get; set; }
        public DateTime? d_InsertDate { get; set; }
        public int? i_UpdateUserId { get; set; }
        public DateTime? d_UpdateDate { get; set; }
        public string v_ComentaryUpdate { get; set; }
    }
}
