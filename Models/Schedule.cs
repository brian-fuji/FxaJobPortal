using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class Schedule
    {
        public IList<ScheduleDay> ScheduleDays { get; set; }
        public int ScheduleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RunType { get; set; }
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ScheduleDaysString { get; set; }

        public bool IsSelected { get; set; }

        public Schedule()
        {

        }

        public Schedule (Dictionary<string, object> items)
        {
            this.ScheduleId = (int)items["ScheduleId"];
            this.Name = (string)items["Name"];
            this.Description = (string)items["Description"];
            this.RunType = (string)items["RunType"];
            this.Status = (int)items["Status"];
            this.StartDate = (DateTime)items["StartDate"];
            this.EndDate = (DateTime)items["EndDate"];
        }

        public void UpdateFromSchedule(Schedule schedule)
        {
            this.ScheduleId = schedule.ScheduleId;
            this.Name = schedule.Name;
            this.Description = schedule.Description;
            this.RunType = schedule.RunType;
            this.Status = schedule.Status;
            this.StartDate = schedule.StartDate;
            this.EndDate = schedule.EndDate;

            ScheduleDays.Clear();

            if (schedule.ScheduleDays != null)
            {
                foreach(var s in schedule.ScheduleDays)
                {
                    ScheduleDays.Add(new ScheduleDay().UpdateFromScheduleDay(s));
                }
            }
        }
    }

    public class ScheduleResponse : BaseResponse
    {
        public IList<Schedule> Data { get; set; }
    }
}
