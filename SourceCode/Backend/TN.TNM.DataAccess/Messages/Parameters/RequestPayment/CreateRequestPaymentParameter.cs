using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.RequestPayment
{
    public class CreateRequestPaymentParameter:BaseParameter
    {
        public Databases.Entities.RequestPayment RequestPayment { get; set; }
        public List<IFormFile> FileList { get; set; }
    }
}
