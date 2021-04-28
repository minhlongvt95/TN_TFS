using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Folder
{
    public class AddOrUpdateFolderResult : BaseResult
    {
        public List<FolderEntityModel> ListFolder { get; set; }
    }
}
