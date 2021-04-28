using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Folder;
using TN.TNM.DataAccess.Messages.Parameters.Folder;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class UploadFileByFolderIdRequest:BaseRequest<UploadFileByFolderIdParameter>
    {
        public Guid FolderId { get; set; }
        public List<FileUploadModel> ListFile { get; set; }
        public override UploadFileByFolderIdParameter ToParameter()
        {
            var parameter = new UploadFileByFolderIdParameter()
            {
                UserId = UserId,
                FolderId = FolderId,
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
