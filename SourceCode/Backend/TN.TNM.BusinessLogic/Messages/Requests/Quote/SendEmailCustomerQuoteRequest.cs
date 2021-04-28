using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class SendEmailCustomerQuoteRequest : BaseRequest<SendEmailCustomerQuoteParameter>
    {
        public List<string> ListEmail { get; set; }
        public string TitleEmail { get; set; }
        public string ContentEmail { get; set; }
        public Guid QuoteId { get; set; }
        public string Base64Pdf { get; set; }

        public override SendEmailCustomerQuoteParameter ToParameter()
        {
            return new SendEmailCustomerQuoteParameter
            {
                ListEmail = this.ListEmail,
                TitleEmail = this.TitleEmail,
                ContentEmail = this.ContentEmail,
                QuoteId = this.QuoteId,
                Base64Pdf = Base64Pdf,
                UserId = this.UserId
            };
        }
    }
}
