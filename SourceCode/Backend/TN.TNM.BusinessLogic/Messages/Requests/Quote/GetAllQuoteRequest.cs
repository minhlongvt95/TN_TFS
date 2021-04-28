using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetAllQuoteRequest : BaseRequest<GetAllQuoteParameter>
    {
        public string QuoteCode { get; set; }
        //public string CustomerName { get; set; }
        public string ProductCode { get; set; }
        //public Guid? Seller { get; set; }
        public List<Guid?> QuoteStatusId { get; set; }
        //public DateTime? OrderDateStart { get; set; }
        //public DateTime? OrderDateEnd { get; set; }

        public override GetAllQuoteParameter ToParameter()
        {
            return new GetAllQuoteParameter
            {
                ProductCode=this.ProductCode,
                QuoteCode=this.QuoteCode,
                QuoteStatusId=this.QuoteStatusId,
                UserId=this.UserId
            };
        }
    }
}
