using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeByTopRevenueRequest : BaseRequest<GetEmployeeByTopRevenueParameter>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Guid?> ListOrganizationId { get; set; }
        public Guid? EmployeeId { get; set; }

        public override GetEmployeeByTopRevenueParameter ToParameter()
        {
            return new GetEmployeeByTopRevenueParameter()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                ListOrganizationId = ListOrganizationId,
                EmployeeId = EmployeeId
            };
        }
    }
}
