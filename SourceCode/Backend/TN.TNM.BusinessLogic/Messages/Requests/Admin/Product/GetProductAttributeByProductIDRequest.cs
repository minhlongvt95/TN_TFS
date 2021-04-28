using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class GetProductAttributeByProductIDRequest : BaseRequest<GetProductAttributeByProductIDParameter>
    {

        public Guid ProductId { get; set; }


        public override GetProductAttributeByProductIDParameter ToParameter()
        {

            return new GetProductAttributeByProductIDParameter
            {
                ProductId=ProductId,
                UserId=this.UserId
            };
        }
    }
}
