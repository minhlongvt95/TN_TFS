using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;
using TN.TNM.DataAccess.Models.Folder;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class CreateFileForPromotionRequest : BaseRequest<CreateFileForPromotionParameter>
    {
        public string FolderType { get; set; }
        public Guid? ObjectId { get; set; }
        public List<FileUploadEntityModel> ListFile { get; set; }

        public override CreateFileForPromotionParameter ToParameter()
        {
            return new CreateFileForPromotionParameter()
            {
                UserId = UserId,
                ObjectId = ObjectId,
                FolderType = FolderType,
                ListFile = ListFile
            };
        }
    }
}
