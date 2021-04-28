using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.RequestPayment;

namespace TN.TNM.BusinessLogic.Messages.Responses.RequestPayment
{
    public class FindRequestPaymentResponse:BaseResponse
    {
        public List<RequestPaymentModel> RequestList { get; set; }
    }
}
