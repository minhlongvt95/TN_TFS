using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.File;

namespace TN.TNM.BusinessLogic.Messages.Requests.File
{
    public class UploadFileRequest : BaseRequest<UploadFileParameter>
    {
        public List<IFormFile> FileList { get; set; }

        public override UploadFileParameter ToParameter()
        {
            return new UploadFileParameter()
            {
                FileList = FileList
            };
        }
    }
}
