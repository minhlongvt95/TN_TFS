using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Category
{
    public class UpdateStatusIsDefaultParameter : BaseParameter
    {
        public Guid CategoryId { get; set; }
        public Guid CategoryTypeId { get; set; }
    }
}
