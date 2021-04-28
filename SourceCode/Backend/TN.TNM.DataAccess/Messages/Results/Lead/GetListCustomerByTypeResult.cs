using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetListCustomerByTypeResult : BaseResult
    {
        public List<Models.Lead.LeadReferenceCustomerModel> ListCustomerByType { get; set; }
    }
}
