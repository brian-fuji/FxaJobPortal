using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Models
{

    public partial class EmailStatus
    {
        public int EmailStatusId { get; set; }
        public int? EmailId { get; set; }
        public DateTime? Timestamp { get; set; }
        public int OriginalEmailId { get; set; }
        public string Response { get; set; }
        public string Event { get; set; }
        public string EmailAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime? TransactionDate { get; set; }
        public virtual EmailTransaction Email { get; set; }
    }
}
