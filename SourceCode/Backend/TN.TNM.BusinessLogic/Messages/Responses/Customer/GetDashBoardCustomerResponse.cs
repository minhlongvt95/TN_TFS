using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetDashBoardCustomerResponse : BaseResponse
    {
        public List<ProductCategoryEntityModel> ListCusFollowProduct { get; set; }
        public List<CustomerModel> ListTopPic { get; set; }
        public List<CustomerModel> ListCusCreatedInThisYear { get; set; }
        //public List<CustomerModel> ListCusIsNewest { get; set; }
        //public List<CustomerModel> ListCusIsNewBought { get; set; }
        public List<CustomerModel> ListCusTopRevenueInMonth { get; set; }
        public List<CustomerModel> ListCusIdentification { get; set; }
        public List<CustomerModel> ListCusFree { get; set; }
        
    }
}
