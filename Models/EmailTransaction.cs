using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class EmailTransaction
    {
        public int EmailId { get; set; }
        public int DocumentId { get; set; }
        public string UniqueId { get; set; }
        public string Environment { get; set; }
        public string EmailAddress { get; set; }
        public string Attachments { get; set; }
        public DateTime? ScheduleRunDate { get; set; }
        public int? ScheduleRunTime { get; set; }
        public DateTime? ActualRunDate { get; set; }
        public int? ActualRunTime { get; set; }
        public string PolicyNumber { get; set; }
        public string Status { get; set; }
        public DateTime? TransactionDate { get; set; }

        public virtual ICollection<EmailStatus> EmailStatus { get; set; }

    }

    public class EmailTransactionResponse : BaseResponse
    {
        public IList<EmailTransaction> Data { get; set; }
    }
}
