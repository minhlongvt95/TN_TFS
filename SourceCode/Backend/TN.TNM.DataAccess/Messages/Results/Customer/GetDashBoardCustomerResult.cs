using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetDashBoardCustomerResult : BaseResult
    {
        public List<ProductCategoryEntityModel> ListCusFollowProduct { get; set; }
        public List<CustomerEntityModel> ListTopPic { get; set; }
        public List<CustomerEntityModel> ListCusCreatedInThisYear { get; set; }
        //public List<CustomerEntityModel> ListCusIsNewest { get; set; }
        //public List<CustomerEntityModel> ListCusIsNewBought { get; set; }
        public List<CustomerEntityModel> ListCusTopRevenueInMonth { get; set; }
        public List<CustomerEntityModel> ListCusIdentification { get; set; }
        public List<CustomerEntityModel> ListCusFree { get; set; }
    }
}
