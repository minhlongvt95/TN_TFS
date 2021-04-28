using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Folder
{
    public class DeleteFileRequest : BaseRequest<DeleteFileParameter>
    {
        public Guid FileInFolderId { get; set; }

        public override DeleteFileParameter ToParameter()
        {
            return new DeleteFileParameter()
            {
                UserId = UserId,
                FileInFolderId = FileInFolderId
            };
        }
    }
}
