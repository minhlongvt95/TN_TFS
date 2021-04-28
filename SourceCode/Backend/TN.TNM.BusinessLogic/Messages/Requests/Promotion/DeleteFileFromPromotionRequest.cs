using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class DeleteFileFromPromotionRequest : BaseRequest<DeleteFileFromPromotionParameter>
    {
        public Guid FileInFolderId { get; set; }

        public override DeleteFileFromPromotionParameter ToParameter()
        {
            return new DeleteFileFromPromotionParameter()
            {
                UserId = UserId,
                FileInFolderId = FileInFolderId
            };
        }
    }
}
