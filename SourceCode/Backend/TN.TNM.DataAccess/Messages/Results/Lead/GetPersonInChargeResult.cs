using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetPersonInChargeResult : BaseResult
    {
        public List<EmployeeEntityModel> ListPersonInCharge { get; set; }
    }
}
