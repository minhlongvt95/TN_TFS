using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class DownloadTemplateProductServiceResponse : BaseResponse
    {
        public byte[] TemplateExcel { get; set; }
        public string FileName { get; set; }
    }
}
