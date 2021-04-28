using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class DownloadTemplatePotentialCustomerResponse : BaseResponse
    {
        public byte[] ExcelFile { get; set; }
        public string NameFile { get; set; }
    }
}

