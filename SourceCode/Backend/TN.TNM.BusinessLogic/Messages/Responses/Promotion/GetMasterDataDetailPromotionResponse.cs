using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Promotion
{
    public class GetMasterDataDetailPromotionResponse : BaseResponse
    {
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<CategoryEntityModel> ListCustomerGroup { get; set; }
    }
}
