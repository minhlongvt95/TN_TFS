using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetListCustomerResult: BaseResult
    {
       public List<AreaEntityModel> ListAreaEntityModel { get; set; }
       public List<CategoryEntityModel> ListSourceEntityModel { get; set; }
       public List<CategoryEntityModel> ListStatusCareEntityModel { get; set; }
       public List<CategoryEntityModel> ListCategoryEntityModel { get; set; }
       public List<CustomerServiceLevel> ListCustomerServiceLevel { get; set; }
    }
}
