using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetProductNameAndProductCodeResponse:BaseResponse
    {
        public List<ProductEntityModel> ProductList { get; set; }

    }
}
