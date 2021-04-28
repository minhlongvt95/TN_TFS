using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class GetTop3PotentialCustomersParameter : BaseParameter
    {
        public Guid PersonInChangeId { get; set; }
    }
}
