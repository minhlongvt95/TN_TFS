using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.RequestPayment;

namespace TN.TNM.BusinessLogic.Messages.Requests.RequestPayment
{
    public class FindRequestPaymentRequest : BaseRequest<FindRequestPaymentParameter>
    {
        public string RequestCode { get; set; }
        public List<Guid> StatusList { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? OrganizationId { get; set; }

        public override FindRequestPaymentParameter ToParameter()
        {
            return new FindRequestPaymentParameter()
            {
                RequestCode = RequestCode,
                StatusList = StatusList,
                StartDate = StartDate,
                EndDate = EndDate,
                EmployeeId = EmployeeId,
                PaymentId = PaymentId,
                OrganizationId = OrganizationId
            };
        }
    }
}
