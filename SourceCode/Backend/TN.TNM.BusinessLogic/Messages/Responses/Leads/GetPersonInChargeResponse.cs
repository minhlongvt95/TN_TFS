using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetPersonInChargeResponse : BaseResponse
    {
        public List<EmployeeEntityModel> ListPersonInCharge { get; set; }
    }
}
