using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class DownloadTemplateImportCustomerResponse: BaseResponse
    {
        public byte[] TemplateExcel { get; set; }
        public string FileName { get; set; }
    }
}
