using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Category
{
    public class GetCategoryByIdParameter : BaseParameter
    {
        public Guid CategoryId { get; set; }
    }
}
