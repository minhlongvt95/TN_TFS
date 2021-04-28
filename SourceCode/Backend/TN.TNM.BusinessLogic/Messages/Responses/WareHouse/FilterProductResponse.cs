using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class FilterProductResponse:BaseResponse
    {
        public List<ProductEntityModel> ProductList { get; set; }

    }
}
