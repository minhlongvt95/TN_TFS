using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.File
{
    public class UploadFileForOptionParameter : BaseParameter
    {
        public List<IFormFile> FileList { get; set; }
        public string Option { get; set; }
    }
}
