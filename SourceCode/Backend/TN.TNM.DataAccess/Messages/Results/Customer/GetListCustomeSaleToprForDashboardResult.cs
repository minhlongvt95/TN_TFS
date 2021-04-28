using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetListCustomeSaleToprForDashboardResult: BaseResult
    {
        public List<CustomerEntityModel> ListCusSaleTop { get; set; }
    }
}
