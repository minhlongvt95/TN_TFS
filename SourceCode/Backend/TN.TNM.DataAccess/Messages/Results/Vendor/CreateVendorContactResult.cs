using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class CreateVendorContactResult : BaseResult
    {
        public Guid? ContactId { get; set; }
    }
}
