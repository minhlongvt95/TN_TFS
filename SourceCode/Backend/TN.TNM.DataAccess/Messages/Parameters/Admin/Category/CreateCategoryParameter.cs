using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Category
{
    public class CreateCategoryParameter : BaseParameter
    {
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public Guid CategoryTypeId { get; set; }
    }
}
