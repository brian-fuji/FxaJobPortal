using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{

    public class Batch
    {
        public IList<object> BatchRun { get; set; }
        public IList<object> BatchSchedule { get; set; }
        public int BatchId { get; set; }
        public string BatchBusName { get; set; }
        public string BatchAbbr { get; set; }
        public bool AllowPostal { get; set; }
        public bool AllowEmail { get; set; }
        public bool AllowSms { get; set; }

        public Batch()
        {

        }

        public Batch(int id, Dictionary<string, object> values)
        {
            this.BatchId = id;
            this.BatchBusName = Convert.ToString(values["BatchBusName"]);
            this.BatchAbbr = Convert.ToString(values["BatchAbbr"]).ToUpper();
        }

        public void UpdateFromBatch(Batch b)
        {
            BatchBusName = b.BatchBusName;
            BatchAbbr = b.BatchAbbr;
            AllowPostal = b.AllowPostal;
            AllowEmail = b.AllowEmail;
            AllowSms = b.AllowSms;
        }
    }

    public class BatchResponse : BaseResponse
    {
        public IList<Batch> Data { get; set; }
    }
}
