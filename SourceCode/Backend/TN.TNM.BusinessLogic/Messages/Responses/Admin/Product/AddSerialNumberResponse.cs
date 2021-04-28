using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class AddSerialNumberResponse: BaseResponse
    {
        public List<string> ListSerialNumber { get; set; }
    }
}
