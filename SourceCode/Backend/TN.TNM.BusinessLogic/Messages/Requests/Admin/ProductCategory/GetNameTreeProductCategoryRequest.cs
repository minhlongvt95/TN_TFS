using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.ProductCategory
{
    public class GetNameTreeProductCategoryRequest : BaseRequest<GetNameTreeProductCategoryParameter>
    {

        public Guid? ProductCategoryID { get; set; }
        public override GetNameTreeProductCategoryParameter ToParameter() => new GetNameTreeProductCategoryParameter()
        {
            ProductCategoryID=this.ProductCategoryID
        };
    }
}
