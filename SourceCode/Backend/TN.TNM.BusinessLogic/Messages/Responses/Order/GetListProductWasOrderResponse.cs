using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetListProductWasOrderResponse : BaseResponse
    {
        public Guid OrderId { get; set; }
        public List<ProductEntityModel> ListProductWasOrder { get; set; }
    }
}
