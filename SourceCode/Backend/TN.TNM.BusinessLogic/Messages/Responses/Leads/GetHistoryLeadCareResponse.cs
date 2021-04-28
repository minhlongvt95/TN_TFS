using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetHistoryLeadCareResponse: BaseResponse
    {
        public List<DataAccess.Models.Lead.LeadCareInforModel> ListCustomerCareInfor { get; set; }
    }
}
