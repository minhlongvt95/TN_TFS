using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class CreatePromotionParameter : BaseParameter
    {
        public PromotionEntityModel Promotion { get; set; }
        public List<PromotionMappingEntityModel> ListPromotionMapping { get; set; }
    }
}
