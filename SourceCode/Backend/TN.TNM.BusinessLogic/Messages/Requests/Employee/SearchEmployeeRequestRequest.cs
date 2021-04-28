using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SearchEmployeeRequestRequest : BaseRequest<SearchEmployeeRequestParameter>
    {
        public string EmployeeRequestCode { get; set; }
        public string OfferEmployeeCode { get; set; }
        public string OfferEmployeeName { get; set; }
        public Guid? OfferOrganizationId { get; set; }
        public List<Guid?> ListTypeRequestId { get; set; }
        public List<Guid?> ListStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        

        public override SearchEmployeeRequestParameter ToParameter() => new SearchEmployeeRequestParameter()
        {
            EmployeeRequestCode = EmployeeRequestCode,
            OfferEmployeeCode = OfferEmployeeCode,
            OfferEmployeeName = OfferEmployeeName,
            OfferOrganizationId = OfferOrganizationId,
            ListTypeRequestId = ListTypeRequestId,
            ListStatusId = ListStatusId,
            StartDate = StartDate,
            EndDate = EndDate,
            UserId = UserId
        };
    }
}
