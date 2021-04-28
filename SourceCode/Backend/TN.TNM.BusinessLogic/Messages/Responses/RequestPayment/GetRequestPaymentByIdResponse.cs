using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.Document;
using TN.TNM.BusinessLogic.Models.RequestPayment;

namespace TN.TNM.BusinessLogic.Messages.Responses.RequestPayment
{
    public class GetRequestPaymentByIdResponse:BaseResponse
    {
        public RequestPaymentModel requestPaymentEntityModel { get; set; }
        public List<DocumentModel> lstDocument { get; set; }
        public List<IFormFile> lstDoc { get; set; }
        public bool IsSendingApprove { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
    }
}
