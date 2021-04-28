using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.File
{
    public class UploadFileParameter : BaseParameter
    {
        public List<IFormFile> FileList { get; set; }
    }
}
