using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class SmsTransaction
    {
        public int SmsId { get; set; }
        public int DocumentId { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
        public string Message { get; set; }
        public string UniqueId { get; set; }
        public string ProviderId { get; set; }
        public DateTime? ScheduleRunDate { get; set; }
        public int ScheduleRunTime { get; set; }
        public DateTime? ActualRunDate { get; set; }
        public int ActualRunTime { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    public class SmsTransactionResponse : BaseResponse
    {
        public IList<SmsTransaction> Data { get; set; }
    }
}
