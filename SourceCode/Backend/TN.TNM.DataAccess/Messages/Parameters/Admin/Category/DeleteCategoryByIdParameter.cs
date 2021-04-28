using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Category
{
    public class DeleteCategoryByIdParameter : BaseParameter
    {
        public Guid CategoryId { get; set; }
    }
}
