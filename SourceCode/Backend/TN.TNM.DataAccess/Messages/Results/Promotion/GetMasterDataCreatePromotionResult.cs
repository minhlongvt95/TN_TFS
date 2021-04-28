using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Promotion
{
    public class GetMasterDataCreatePromotionResult : BaseResult
    {
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<CategoryEntityModel> ListCustomerGroup { get; set; }
    }
}
