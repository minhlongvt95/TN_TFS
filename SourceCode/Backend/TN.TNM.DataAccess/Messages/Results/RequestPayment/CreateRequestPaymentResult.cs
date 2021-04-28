using System;

namespace TN.TNM.DataAccess.Messages.Results.RequestPayment
{
    public class CreateRequestPaymentResult:BaseResult
    {
        public Guid RequestPaymentId { get; set; }
    }
}
