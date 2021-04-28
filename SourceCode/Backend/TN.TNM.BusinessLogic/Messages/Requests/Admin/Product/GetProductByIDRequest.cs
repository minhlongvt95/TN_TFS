using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class GetProductByIDRequest : BaseRequest<GetProductByIDParameter>
    {

        public Guid ProductId { get; set; }


        public override GetProductByIDParameter ToParameter()
        {

            return new GetProductByIDParameter
            {
                ProductId=ProductId,
                UserId=this.UserId
            };
        }
    }
}
