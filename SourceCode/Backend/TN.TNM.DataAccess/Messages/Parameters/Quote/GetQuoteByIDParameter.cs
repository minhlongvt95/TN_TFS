using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetQuoteByIDParameter:BaseParameter
    {
        public Guid QuoteId { get; set; }
        public string ObjectType { get; set; }
    }
}
