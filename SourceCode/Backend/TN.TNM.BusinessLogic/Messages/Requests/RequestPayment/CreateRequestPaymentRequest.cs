using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.RequestPayment;
using TN.TNM.DataAccess.Messages.Parameters.RequestPayment;

namespace TN.TNM.BusinessLogic.Messages.Requests.RequestPayment
{
    public class CreateRequestPaymentRequest : BaseRequest<CreateRequestPaymentParameter>
    {
        public RequestPaymentModel RequestPayment { get; set; }
        public List<IFormFile> FileList { get; set; }

        public override CreateRequestPaymentParameter ToParameter()
        {
            return new CreateRequestPaymentParameter
            {
                RequestPayment = RequestPayment.ToEntity(),
                FileList=FileList,
                UserId = this.UserId
            };
        }
    }
}
