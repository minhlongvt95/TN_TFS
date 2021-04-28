using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Folder
{
    public class UploadFileParameter : BaseParameter
    {
        public string FolderType { get; set; }
        public Guid ObjectId { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }
}
