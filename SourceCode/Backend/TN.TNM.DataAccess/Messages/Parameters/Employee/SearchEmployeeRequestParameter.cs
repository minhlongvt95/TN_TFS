using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SearchEmployeeRequestParameter : BaseParameter
    {
        public string EmployeeRequestCode { get; set; }
        public string OfferEmployeeCode { get; set; }
        public string OfferEmployeeName { get; set; }
        public Guid? OfferOrganizationId { get; set; }
        public List<Guid?> ListTypeRequestId { get; set; }
        public List<Guid?> ListStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
    }
}
