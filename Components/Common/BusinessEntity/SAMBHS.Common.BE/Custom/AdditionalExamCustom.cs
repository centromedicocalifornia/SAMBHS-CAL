using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class AdditionalExamDto
    {

        public string v_AdditionalExamId { get; set; }
        public string v_ServiceId { get; set; }
        public string v_PersonId { get; set; }
        public string v_ProtocolId { get; set; }
        public string v_ComponentId { get; set; }
        public string v_Commentary { get; set; }
        public int i_IsProcessed { get; set; }
        public int i_IsNewService { get; set; }
        public int i_IsDeleted { get; set; }
        public int i_InsertUserId { get; set; }
        public DateTime d_InsertDate { get; set; }
        public int i_UpdateUserId { get; set; }
        public DateTime d_UpdateDate { get; set; }

    }
    public class AdditionalExamCustom
    {

        public string AdditionalExamId { get; set; }
        public string ServiceId { get; set; }
        public string PersonId { get; set; }
        public string ProtocolId { get; set; }
        public string ComponentId { get; set; }
        public string Commentary { get; set; }
        public int IsProcessed { get; set; }
        public int IsNewService { get; set; }

    }
}
