using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class BatchRunStatus
    {
        public int BatchRunStatusId { get; set; }
        public int BatchRunId { get; set; }
        public string Status { get; set; }
        public DateTime? StatusDate { get; set; }
    }
}
