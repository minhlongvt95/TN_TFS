using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetEmployeeByTopRevenueResponse : BaseResponse
    {
        public List<dynamic> listEmployee { get; set; }
    }
}
