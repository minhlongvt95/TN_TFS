using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeByOrganizationIdResult : BaseResult
    {
        public List<dynamic> listEmployee { get; set; }
    }
}
