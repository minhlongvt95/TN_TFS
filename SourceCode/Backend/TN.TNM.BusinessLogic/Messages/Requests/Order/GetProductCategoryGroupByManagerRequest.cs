using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetProductCategoryGroupByManagerRequest : BaseRequest<GetProductCategoryGroupByManagerParameter>
    {
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public Guid OrganizationId { get; set; }
        public int ProductCategoryLevel { get; set; }
        public override GetProductCategoryGroupByManagerParameter ToParameter() => new GetProductCategoryGroupByManagerParameter()
        {
            OrderDateStart = OrderDateStart,
            OrderDateEnd = OrderDateEnd,
            OrganizationId = OrganizationId,
            ProductCategoryLevel = ProductCategoryLevel
        };
    }
}
