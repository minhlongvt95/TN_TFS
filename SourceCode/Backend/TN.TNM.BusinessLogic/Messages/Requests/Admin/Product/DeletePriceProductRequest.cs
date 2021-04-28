using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class DeletePriceProductRequest : BaseRequest<DeletePriceProductParameter>
    {
        public Guid PriceProductId { get; set; }
        public override DeletePriceProductParameter ToParameter()
        {
            return new DeletePriceProductParameter
            {
                PriceProductId = PriceProductId,
                UserId = UserId
            };
        }
    }
}
