using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.RequestPayment;
using TN.TNM.DataAccess.Messages.Parameters.RequestPayment;

namespace TN.TNM.BusinessLogic.Messages.Requests.RequestPayment
{
    public class EditRequestPaymentRequest:BaseRequest<EditRequestPaymentParameter>
    {
        public RequestPaymentModel RequestPayment { get; set; }
        public List<IFormFile> FileList { get; set; }
        public List<string> lstDocument { get; set; }
        public override EditRequestPaymentParameter ToParameter()
        {
            return new EditRequestPaymentParameter
            {
                RequestPayment = RequestPayment.ToEntity(),
                FileList=FileList,
                lstDocument= lstDocument,
                UserId = this.UserId
            };

        }
    }
}
