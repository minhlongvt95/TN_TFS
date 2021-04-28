using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.LinkOfDocument;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class CreateLinkForPromotionParameter : BaseParameter
    {
        public LinkOfDocumentEntityModel LinkOfDocument { get; set; }
    }
}
