using System;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class UpdateStatusCustomerCareRequest :BaseRequest<UpdateStatusCustomerCareParameter>
    {
        public Guid CustomerCareId { get; set; }
        public Guid StatusId { get; set; }
        public bool IsSendNow { get; set; }
        public DateTime? SendDate { get; set; }
        public TimeSpan? SendHour { get; set; }
        public string TypeCusCareCode { get; set; }

        public override UpdateStatusCustomerCareParameter ToParameter()
        {
            return new UpdateStatusCustomerCareParameter
            {
                CustomerCareId = CustomerCareId,
                StatusId = StatusId,
                IsSendNow = IsSendNow,
                SendDate = SendDate,
                SendHour = SendHour,
                TypeCusCareCode = TypeCusCareCode,
                UserId = UserId
            };
        }
    }
}
