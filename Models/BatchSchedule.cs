using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class BatchSchedule
    {
        public object Batch { get; set; }
        public int ScheduleId { get; set; }
        public int BatchScheduleId { get; set; }
        public int BatchId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class BatchScheduleResponse : BaseResponse
    {
        public IList<BatchSchedule> Data { get; set; }
    }
}
