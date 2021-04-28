using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetTop3QuotesOverdueParameter : BaseParameter
    {
        public Guid PersonInChangeId { get; set; }
    }
}
