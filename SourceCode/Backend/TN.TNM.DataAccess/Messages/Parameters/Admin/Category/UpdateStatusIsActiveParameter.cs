using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Category
{
    public class UpdateStatusIsActiveParameter : BaseParameter
    {
        public Guid CategoryId { get; set; }
        public bool? Active { get; set; }
    }
}
