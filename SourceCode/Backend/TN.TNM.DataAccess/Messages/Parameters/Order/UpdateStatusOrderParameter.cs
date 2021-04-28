using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class UpdateStatusOrderParameter : BaseParameter
    {
        public Guid CustomerOrderId { get; set; }
        public string ObjectType { get; set; }
        public string Description { get; set; }
    }
}
