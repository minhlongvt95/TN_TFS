using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class DeleteLinkFromPromotionRequest : BaseRequest<DeleteLinkFromPromotionParameter>
    {
        public Guid LinkOfDocumentId { get; set; }

        public override DeleteLinkFromPromotionParameter ToParameter()
        {
            return new DeleteLinkFromPromotionParameter()
            {
                UserId = UserId,
                LinkOfDocumentId = LinkOfDocumentId
            };
        }
    }
}
