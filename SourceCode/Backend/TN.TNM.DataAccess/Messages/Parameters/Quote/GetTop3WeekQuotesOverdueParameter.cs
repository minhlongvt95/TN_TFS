using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetTop3WeekQuotesOverdueParameter : BaseParameter
    {
        public Guid PersonInChangeId { get; set; }
    }
}
