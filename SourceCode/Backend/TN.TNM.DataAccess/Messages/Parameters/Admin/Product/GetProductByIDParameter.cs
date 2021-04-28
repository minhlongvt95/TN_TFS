using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class GetProductByIDParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
    }
}
