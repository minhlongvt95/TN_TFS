using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Folder
{
    public class DeleteFileParameter : BaseParameter
    {
        public Guid FileInFolderId { get; set; }
    }
}
