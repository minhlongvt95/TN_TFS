using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class CreateOrUpdatePriceProductResponse : BaseResponse
    {
        public List<PriceProductModel> ListPrice { get; set; }
    }
}
