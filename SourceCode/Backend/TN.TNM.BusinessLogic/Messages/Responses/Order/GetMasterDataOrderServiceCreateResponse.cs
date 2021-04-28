using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.LocalAddress;
using TN.TNM.DataAccess.Models.LocalPoint;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetMasterDataOrderServiceCreateResponse : BaseResponse
    {
        public List<OrderStatus> ListOrderStatus { get; set; }
        public List<CategoryEntityModel> ListMoneyUnit { get; set; }
        public List<ProductCategoryEntityModel> ListProductCategory { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<LocalAddressEntityModel> ListLocalAddress { get; set; }
    }
}
