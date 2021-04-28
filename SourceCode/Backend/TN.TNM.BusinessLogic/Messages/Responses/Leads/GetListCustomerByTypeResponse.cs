using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetListCustomerByTypeResponse : BaseResponse
    {
        public List<DataAccess.Models.Lead.LeadReferenceCustomerModel> ListCustomerByType { get; set; }
    }
}
