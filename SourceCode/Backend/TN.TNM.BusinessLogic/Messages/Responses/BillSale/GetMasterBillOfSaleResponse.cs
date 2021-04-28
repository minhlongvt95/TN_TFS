using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.BillSale;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.BillSale
{
    public class GetMasterBillOfSaleResponse : BaseResponse
    {
        public List<ProductModel> ListProduct { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
    }
}
