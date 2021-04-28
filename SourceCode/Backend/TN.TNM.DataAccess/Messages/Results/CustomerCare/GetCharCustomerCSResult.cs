using System.Collections.Generic;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Results.CustomerCare
{
    public class GetCharCustomerCSResult : BaseResult
    {
        public List<GetCharCustomerCSEntityModel> ListChar { get; set; }
    }
}
