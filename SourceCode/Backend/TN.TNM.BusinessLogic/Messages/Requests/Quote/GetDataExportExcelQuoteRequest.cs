using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetDataExportExcelQuoteRequest : BaseRequest<GetDataExportExcelQuoteParameter>
    {
        public Guid QuoteId { get; set; }
        public override GetDataExportExcelQuoteParameter ToParameter()
        {
            return new GetDataExportExcelQuoteParameter()
            {
                QuoteId = QuoteId,
                UserId = UserId
            };
        }
    }
}
