using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Models.Document;
using TN.TNM.DataAccess.Models.RequestPayment;

namespace TN.TNM.DataAccess.Messages.Results.RequestPayment
{
    public class GetRequestPaymentByIdResult:BaseResult
    {
        public RequestPaymentEntityModel requestPaymentEntityModel { get; set;}
        public List<DocumentEntityModel> lstDocument { get; set; }
        public List<IFormFile> lstDoc { get; set; }
        public bool IsSendingApprove { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
    }
}
