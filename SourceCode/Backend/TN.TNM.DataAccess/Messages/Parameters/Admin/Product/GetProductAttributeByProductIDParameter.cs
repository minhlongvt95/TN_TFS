using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class GetProductAttributeByProductIDParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
    }
}
