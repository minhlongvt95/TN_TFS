using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetListProductWasOrderResult : BaseResult
    {
        public Guid OrderId { get; set; }
        public List<ProductEntityModel> ListProductWasOrder { get; set; }
    }
}
