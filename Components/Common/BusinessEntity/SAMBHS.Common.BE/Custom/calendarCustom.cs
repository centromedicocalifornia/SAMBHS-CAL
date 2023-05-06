using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class calendarCustom
    {
        public string v_CalendarId { get; set; }
        public string v_PersonId { get; set; }
        public string v_ServiceId { get; set; }
        public DateTime? d_DateTimeCalendar { get; set; }
        public DateTime? d_CircuitStartDate { get; set; }
        public DateTime? d_EntryTimeCM { get; set; }
        public int? i_ServiceTypeId { get; set; }
        public int? i_CalendarStatusId { get; set; }
        public int? i_ServiceId { get; set; }
        public string v_ProtocolId { get; set; }
        public int? i_NewContinuationId { get; set; }
        public int? i_LineStatusId { get; set; }
        public int? i_IsVipId { get; set; }
        public int? i_IsDeleted { get; set; }
        public int? i_InsertUserId { get; set; }
        public DateTime? d_InsertDate { get; set; }
        public int? i_UpdateUserId { get; set; }
        public DateTime? d_UpdateDate { get; set; }
        public DateTime? d_SalidaCM { get; set; }
        public string v_ComentaryUpdate { get; set; }
    }
}
