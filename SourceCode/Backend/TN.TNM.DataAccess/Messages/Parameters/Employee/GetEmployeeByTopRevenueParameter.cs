using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeByTopRevenueParameter : BaseParameter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Guid?> ListOrganizationId { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}
