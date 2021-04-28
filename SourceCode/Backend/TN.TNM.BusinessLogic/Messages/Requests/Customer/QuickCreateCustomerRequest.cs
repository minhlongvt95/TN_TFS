using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class QuickCreateCustomerRequest : BaseRequest<QuickCreateCustomerParameter>
    {
        public short? CustomerType { get; set; }
        public string CustomerCode { get; set; }
        public Guid CustomerGroupId { get; set; }
        public Guid? PaymentId { get; set; }
        public string CustomerName { get; set; }
        public decimal? MaximumDebtValue { get; set; }
        public int? MaximumDebtDays { get; set; }
        public DateTime CreatedDate { get; set; }
        public override QuickCreateCustomerParameter ToParameter()
        {
            return new QuickCreateCustomerParameter()
            {
                UserId = UserId,
                CustomerCode = CustomerCode,
                CustomerName = CustomerName,
                CustomerType = CustomerType,
                MaximumDebtDays = MaximumDebtDays,
                MaximumDebtValue = MaximumDebtValue,
                PaymentId = PaymentId,
                CustomerGroupId = CustomerGroupId,
                CreatedDate = CreatedDate
            };
        }
    }
}
