using System;
using TN.TNM.DataAccess.Messages.Parameters.RequestPayment;

namespace TN.TNM.BusinessLogic.Messages.Requests.RequestPayment
{
    public class GetRequestPaymentByIdRequest:BaseRequest<GetRequestPaymentByIdParameter>
    {
        public Guid RequestPaymentId { get; set; }

        public override GetRequestPaymentByIdParameter ToParameter()
        {
            return new GetRequestPaymentByIdParameter
            {
                RequestPaymentId = RequestPaymentId,
                UserId = this.UserId
            };
        }
    }
}
