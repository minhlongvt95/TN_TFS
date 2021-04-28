using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class CreateQuoteParameter:BaseParameter
    {
        public Databases.Entities.Quote Quote { get; set; }
        public List<QuoteDetail> QuoteDetail { get; set; }
        public List<QuoteCostDetail> QuoteCostDetail { get; set; }
        public int TypeAccount { get; set; }
        public List<QuoteDocument> QuoteDocument { get; set; }
        public List<AdditionalInformation> ListAdditionalInformation { get; set; }
        public bool isClone { get; set; }
        public Guid QuoteIdClone { get; set; }
        public List<Guid> ListParticipant { get; set; }
        public List<PromotionObjectApplyEntityModel> ListPromotionObjectApply { get; set; }
    }
}
