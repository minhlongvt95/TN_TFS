using System;

namespace TN.TNM.DataAccess.Messages.Parameters.RequestPayment
{
    public class GetRequestPaymentByIdParameter:BaseParameter
    {
        public Guid RequestPaymentId { get; set; }
    }
}
