using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class DeleteFileParameter : BaseParameter
    {
        public Guid FileInFolderId { get; set; }
    }
}
