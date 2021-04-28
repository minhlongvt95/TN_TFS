using System;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class GetTop3WeekQuotesOverdueModel
    {
        public Guid QuoteId { get; set; }
        public string QuoteCode { get; set; }
        public string QuoteName { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? SendQuoteDate { get; set; }
        public DateTime? IntendedQuoteDate { get; set; }
        public decimal Amount { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public DateTime IntendedQuoteDateWeek { get; set; }

    }
}
