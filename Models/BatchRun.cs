using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class BatchRun
    {
        public Batch Batch { get; set; }
        public int RunNumber { get; set; }
        public int? BatchId { get; set; }
        public string BatchNumber { get; set; }
        public string BatchAbbr { get; set; }
        public int? HashTotal { get; set; }
        public int? DocumentCount { get; set; }
        public int? PageCount { get; set; }
        public string DistributedData { get; set; }
        public bool? Reprocessed { get; set; }
        public string Status { get; set; }
        public string Filename { get; set; }
        public string Filepath { get; set; }
        public DateTime? RunDate { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public List<BatchRunStatus> BatchRunStatus { get; set; }

        // From Batch
        public string BatchBusName { get { return Batch?.BatchBusName; } }
    }

    public class BatchRunResponse : BaseResponse
    {
        public IList<BatchRun> Data { get; set; }
    }
}
