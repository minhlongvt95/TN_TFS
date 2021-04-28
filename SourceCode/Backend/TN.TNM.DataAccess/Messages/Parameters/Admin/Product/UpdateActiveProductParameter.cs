using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class UpdateActiveProductParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
    }
}
