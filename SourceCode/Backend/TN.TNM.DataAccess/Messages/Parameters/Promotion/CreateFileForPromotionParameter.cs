using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class CreateFileForPromotionParameter : BaseParameter
    {
        public string FolderType { get; set; }
        public Guid? ObjectId { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }
    }
}
