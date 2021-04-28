using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Folder
{
    public class GetAllFolderActiveResult : BaseResult
    {
        public List<FolderEntityModel> ListFolderActive { get; set; }
    }
}
