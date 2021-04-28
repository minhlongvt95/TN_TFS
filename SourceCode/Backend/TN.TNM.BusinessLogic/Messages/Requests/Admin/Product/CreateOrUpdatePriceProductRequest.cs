using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class CreateOrUpdatePriceProductRequest : BaseRequest<CreateOrUpdatePriceProductParameter>
    {
        public DataAccess.Models.Product.PriceProductEntityModel PriceProduct { get; set; }
        public override CreateOrUpdatePriceProductParameter ToParameter()
        {
            return new CreateOrUpdatePriceProductParameter
            {
                UserId = UserId,
                PriceProduct = PriceProduct
            };
        }
    }
}
