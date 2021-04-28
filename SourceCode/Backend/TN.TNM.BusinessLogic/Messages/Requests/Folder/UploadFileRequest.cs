using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Messages.Parameters.Folder;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class UploadFileRequest : BaseRequest<UploadFileParameter>
    {
        public string FolderType { get; set; }
        public Guid ObjectId { get; set; }
        public List<FileUploadModel> ListFile { get; set; }

        public override UploadFileParameter ToParameter()
        {
            var parameter = new UploadFileParameter()
            {
                UserId = UserId,
                FolderType = FolderType,
                ObjectId = ObjectId,
                ListFile = new List<FileUploadEntityModel>()
            };

            ListFile.ForEach(item =>
            {
                var file = new FileUploadEntityModel();
                file.FileInFolder = item.FileInFolder.ToEntityModel();
                file.FileSave = item.FileSave;
                parameter.ListFile.Add(file);
            });

            return parameter;
        }
    }
}
