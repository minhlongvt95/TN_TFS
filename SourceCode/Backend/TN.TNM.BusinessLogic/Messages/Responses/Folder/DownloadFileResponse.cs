using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Folder
{
    public class DownloadFileResponse:BaseResponse
    {
        public byte[] FileAsBase64 { get; set; }
        public string FileType { get; set; }
    }
}
