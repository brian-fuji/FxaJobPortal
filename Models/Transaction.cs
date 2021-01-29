using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class Transaction
    {
        public int DocumentId { get; set; }
        public int BatchRunNumber { get; set; }
        public BatchRun BatchRun { get; set; }
        public string BatchAbbr { get; set; }
        public string TransactionType { get; set; }
        public DateTime? IssueDate { get; set; }
        public string PolicyNumber { get; set; }
        public string CoverType { get; set; }
        public string NoticeType { get; set; }
        public string Status { get; set; }
        public string Metadata { get; set; }
        public DateTime? TransactionDate { get; set; }
    }

    public class TransactionResponse : BaseResponse
    {
        public IList<Transaction> Data { get; set; }
    }
}
