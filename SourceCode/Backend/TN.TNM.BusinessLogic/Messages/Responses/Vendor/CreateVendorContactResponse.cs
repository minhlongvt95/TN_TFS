using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class CreateVendorContactResponse: BaseResponse
    {
        public Guid? ContactId { get; set; }
    }
}
