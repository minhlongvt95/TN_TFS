using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Folder
{
    public class CreateFolderResult : BaseResult
    {
        public FolderEntityModel Folder { get; set; }
    }
}
