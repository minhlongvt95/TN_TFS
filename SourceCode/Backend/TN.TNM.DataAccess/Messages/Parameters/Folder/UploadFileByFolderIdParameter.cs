using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Folder
{
    public class UploadFileByFolderIdParameter:BaseParameter
    {
        public Guid FolderId { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }
}
