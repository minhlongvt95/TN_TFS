using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory
{
    public class GetNameTreeProductCategoryParameter : BaseParameter
    {
        public Guid? ProductCategoryID { get; set; }
    }
}
