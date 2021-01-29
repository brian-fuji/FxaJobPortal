using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class ScheduleDay
    {
        public List<object> BatchRun { get; set; }

        public Schedule Schedule { get; set; }
        public int ScheduleDayId { get; set; }
        public int ScheduleId { get; set; }
        public int DayOfWeek { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public bool IsSelected { get; set; }

        public ScheduleDay UpdateFromScheduleDay(ScheduleDay day)
        {
            this.ScheduleDayId = day.ScheduleDayId;
            this.ScheduleId = day.ScheduleId;
            this.DayOfWeek = day.DayOfWeek;
            this.StartTime = day.StartTime;
            this.EndTime = day.EndTime;

            return this;
        }
    }

    public class ScheduleDayResponse : BaseResponse
    {
        public IList<ScheduleDay> Data { get; set; }
    }
}
