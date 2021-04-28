using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;
using TN.TNM.DataAccess.Models.LinkOfDocument;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class CreateLinkForPromotionRequest : BaseRequest<CreateLinkForPromotionParameter>
    {
        public LinkOfDocumentEntityModel LinkOfDocument { get; set; }

        public override CreateLinkForPromotionParameter ToParameter()
        {
            return new CreateLinkForPromotionParameter()
            {
                UserId = UserId,
                LinkOfDocument = LinkOfDocument
            };
        }
    }
}
