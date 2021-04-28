using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.RequestPayment
{
    public class FindRequestPaymentParameter:BaseParameter
    {
        public string RequestCode { get; set; }
        public List<Guid> StatusList { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}
