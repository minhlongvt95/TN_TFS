using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetDataExportExcelQuoteParameter : BaseParameter
    {
        public Guid QuoteId { get; set; }
    }
}
