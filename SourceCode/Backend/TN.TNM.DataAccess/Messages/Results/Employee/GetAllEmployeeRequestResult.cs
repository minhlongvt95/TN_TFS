using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllEmployeeRequestResult : BaseResult
    {
        public List<EmployeeRequestEntityModel> EmployeeRequestList { get; set; }
        public List<OrganizationDetail> OrganizationList { get; set; }
    }

    public class OrganizationDetail
    {
        public Guid? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        


    }
}
