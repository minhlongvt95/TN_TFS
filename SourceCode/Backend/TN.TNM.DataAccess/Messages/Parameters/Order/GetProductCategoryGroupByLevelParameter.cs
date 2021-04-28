using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetProductCategoryGroupByLevelParameter : BaseParameter
    {
        public Guid Seller { get; set; }
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public int ProductCategoryLevel { get; set; }
    }
}
