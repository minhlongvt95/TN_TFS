using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.File;

namespace TN.TNM.BusinessLogic.Messages.Responses.File
{
    public class DownloadProductImageResponse: BaseResponse
    {
        public List<ProductImageResponseModel> ListProductImageResponseModel { get; set; }
    }
}
