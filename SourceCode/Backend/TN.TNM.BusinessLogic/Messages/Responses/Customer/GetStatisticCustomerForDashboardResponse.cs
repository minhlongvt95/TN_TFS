using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetStatisticCustomerForDashboardResponse : BaseResponse
    {
        public List<CustomerModel> ListCusIsNewest { get; set; }
        public List<CustomerModel> ListCusIsNewBought { get; set; }
        //public List<model.Customer.CustomerModel> ListCusSaleTop { get; set; }
        public List<ProductCategoryEntityModel> ListCusFollowProduct { get; set; }
        public List<CustomerModel> ListTopPic { get; set; }
        public List<CustomerModel> ListCusCreatedInThisYear { get; set; }
    }
}
