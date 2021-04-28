using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class ExportEmployeeRevenueRequest : BaseRequest<ExportEmployeeRevenueParameter>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Guid?> ListOrganizationId { get; set; }
        public Guid? EmployeeId { get; set; }
        public override ExportEmployeeRevenueParameter ToParameter()
        {
            return new ExportEmployeeRevenueParameter()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                ListOrganizationId = ListOrganizationId,
                EmployeeId = EmployeeId
            };
        }
    }
}
