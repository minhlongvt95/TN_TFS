using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class DeleteLinkFromPromotionParameter : BaseParameter
    {
        public Guid LinkOfDocumentId { get; set; }
    }
}
