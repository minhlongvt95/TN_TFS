using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetStatisticCustomerForDashboardResult : BaseResult
    {
        public List<CustomerEntityModel> ListCusIsNewest { get; set; }
        public List<CustomerEntityModel> ListCusIsNewBought { get; set; }
        //public List<CustomerEntityModel> ListCusSaleTop { get; set; }
        public List<ProductCategoryEntityModel> ListCusFollowProduct { get; set; }
        public List<CustomerEntityModel> ListTopPic { get; set; }
        public List<CustomerEntityModel> ListCusCreatedInThisYear { get; set; }
    }
}
