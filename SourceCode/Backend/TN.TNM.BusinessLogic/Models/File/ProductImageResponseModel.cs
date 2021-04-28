using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Models.File
{
    public class ProductImageResponseModel
    {
        public byte[] FileAsBase64 { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}
