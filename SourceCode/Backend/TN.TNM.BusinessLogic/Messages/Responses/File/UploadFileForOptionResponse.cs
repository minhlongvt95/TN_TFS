using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.File;

namespace TN.TNM.BusinessLogic.Messages.Responses.File
{
    public class UploadFileForOptionResponse : BaseResponse
    {
        public List<FileNameExistsModel> ListFileNameExists { get; set; }
    }
}
