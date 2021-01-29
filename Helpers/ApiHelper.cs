using FxaPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Helpers
{
    public class ApiHelper
    {
        public Schedule ScheduleToValue(IDictionary<string, object> newValue)
        {
            var schedule = new Schedule();

            try
            {
                if (newValue.ContainsKey("Name"))
                    schedule.Name = (string)newValue["Name"];

                if (newValue.ContainsKey("Description"))
                    schedule.Description = (string)newValue["Description"];

                if (newValue.ContainsKey("RunType"))
                    schedule.RunType = (string)newValue["RunType"];
                Console.WriteLine("RunType");
                if (newValue.ContainsKey("Status"))
                {
                    var status = newValue["Status"].ToString();
                    Console.WriteLine(status);
                    int.TryParse(status, out int val);
                    schedule.Status = val;
                }
                Console.WriteLine("Status");
                if (newValue.ContainsKey("StartDate"))
                    schedule.StartDate = (DateTime)newValue["StartDate"];

                Console.WriteLine("StartDate");
                if (newValue.ContainsKey("EndDate"))
                    schedule.EndDate = (DateTime)newValue["EndDate"];

                Console.WriteLine("EndDate");

            } catch (Exception ex)
            {
                Console.WriteLine($"Problem converting data: {ex.Message}");
            }

            return schedule;
        }
    }
}
