using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetProductCategoryGroupByManagerParameter : BaseParameter
    {
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public Guid OrganizationId { get; set; }
        public int ProductCategoryLevel { get; set; }
    }
}
