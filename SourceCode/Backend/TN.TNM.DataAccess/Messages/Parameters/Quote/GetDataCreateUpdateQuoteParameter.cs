using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetDataCreateUpdateQuoteParameter : BaseParameter
    {
        public Guid? QuoteId { get; set; }
    }
}
