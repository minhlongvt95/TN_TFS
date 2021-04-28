using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Folder
{
    public class GetAllFolderDefaultResult:BaseResult
    {
        public List<FolderEntityModel> ListFolderDefault { get; set; }
    }
}
