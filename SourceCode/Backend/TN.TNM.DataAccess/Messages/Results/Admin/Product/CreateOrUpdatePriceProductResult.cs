using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class CreateOrUpdatePriceProductResult : BaseResult
    {
        public List<PriceProductEntityModel> ListPrice { get; set; }
    }
}
