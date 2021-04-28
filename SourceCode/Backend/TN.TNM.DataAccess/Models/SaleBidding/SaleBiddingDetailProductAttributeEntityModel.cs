using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.SaleBidding
{
    public class SaleBiddingDetailProductAttributeEntityModel
    {
        public Guid SaleBiddingDetailProductAttributeId { get; set; }
        public Guid? SaleBiddingDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductAttributeCategoryId { get; set; }
        public Guid? ProductAttributeCategoryValueId { get; set; }
    }
}
