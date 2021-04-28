using System;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetProductCategoryGroupByLevelRequest : BaseRequest<GetProductCategoryGroupByLevelParameter>
    {
        public Guid Seller { get; set; }
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public int ProductCategoryLevel { get; set; }
        public override GetProductCategoryGroupByLevelParameter ToParameter() => new GetProductCategoryGroupByLevelParameter()
        {
            Seller = Seller,
            OrderDateStart = OrderDateStart,
            OrderDateEnd = OrderDateEnd,
            ProductCategoryLevel = ProductCategoryLevel
        };
    }
}
