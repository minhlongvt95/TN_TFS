using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Results.Contract
{
    public class UploadFileResult : BaseResult
    {
        public List<FileInFolderEntityModel> ListFileInFolder { get; set; }
    }
}
