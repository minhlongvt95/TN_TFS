using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Category
{
    public class EditCategoryByIdParameter : BaseParameter
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
    }
}
