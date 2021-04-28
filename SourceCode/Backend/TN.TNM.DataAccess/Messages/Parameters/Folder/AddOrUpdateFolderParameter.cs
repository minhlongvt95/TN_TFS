using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.DataAccess.Messages.Parameters.Folder
{
    public class AddOrUpdateFolderParameter:BaseParameter
    {
        public List<FolderEntityModel> ListFolder { get; set; }
    }
}
