using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.File;

namespace TN.TNM.BusinessLogic.Messages.Requests.File
{
    public class UploadFileForOptionRequest : BaseRequest<UploadFileForOptionParameter>
    {
        public List<IFormFile> FileList { get; set; }
        public string Option { get; set; }
        public override UploadFileForOptionParameter ToParameter()
        {
            return new UploadFileForOptionParameter()
            {
                FileList = FileList,
                UserId = UserId,
                Option = Option
            };
        }
    }
}
