using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Folder
{
    public class CreateFolderParameter:BaseParameter
    {
        public FolderEntityModel FolderParent { get; set; }
        public string FolderName { get; set; }
    }
}
