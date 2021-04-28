using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class QuickCreateCustomerParameter : BaseParameter
    {
        public short? CustomerType { get; set; }
        public string CustomerCode { get; set; }
        public Guid CustomerGroupId { get; set; }
        public Guid? PaymentId { get; set; }
        public string CustomerName { get; set; }
        public decimal? MaximumDebtValue { get; set; }
        public int? MaximumDebtDays { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
