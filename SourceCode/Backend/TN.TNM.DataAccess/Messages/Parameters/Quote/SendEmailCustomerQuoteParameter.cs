using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class SendEmailCustomerQuoteParameter : BaseParameter
    {
        public List<string> ListEmail { get; set; }
        public string TitleEmail { get; set; }
        public string ContentEmail { get; set; }
        public Guid QuoteId { get; set; }
        public string Base64Pdf { get; set; }
    }
}
