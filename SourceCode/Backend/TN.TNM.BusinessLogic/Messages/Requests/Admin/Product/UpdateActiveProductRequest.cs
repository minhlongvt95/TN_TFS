using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class UpdateActiveProductRequest : BaseRequest<UpdateActiveProductParameter>
    {
        public Guid ProductId { get; set; }


        public override UpdateActiveProductParameter ToParameter()
        {
            return new UpdateActiveProductParameter
            {
                ProductId = ProductId,
                UserId = UserId
            };
        }
    }
}
