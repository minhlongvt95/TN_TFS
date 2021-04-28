using System.Collections.Generic;
using TN.TNM.DataAccess.Models.RequestPayment;

namespace TN.TNM.DataAccess.Messages.Results.RequestPayment
{
    public class FindRequestPaymentResult:BaseResult
    {
        public List<RequestPaymentEntityModel> RequestList { get; set; }
    }
}
