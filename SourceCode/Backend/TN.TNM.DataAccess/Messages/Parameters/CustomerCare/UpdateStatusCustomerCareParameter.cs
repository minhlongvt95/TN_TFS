using System;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class UpdateStatusCustomerCareParameter : BaseParameter
    {
        public Guid CustomerCareId { get; set; }
        public Guid StatusId { get; set; }
        public bool IsSendNow { get; set; }
        public DateTime? SendDate { get; set; }
        public TimeSpan? SendHour { get; set; }
        public string TypeCusCareCode { get; set; }
    }
}
