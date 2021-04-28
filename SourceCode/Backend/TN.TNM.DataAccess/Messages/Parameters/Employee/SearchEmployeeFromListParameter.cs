using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SearchEmployeeFromListParameter : BaseParameter
    {
        public bool IsManager { get; set; }
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string IdentityId { get; set; }
        public List<Guid> ListPosition { get; set; }
        public Guid? OrganizationId { get; set; }
        public DateTime? FromContractExpiryDate { get; set; }
        public DateTime? ToContractExpiryDate { get; set; }
        public DateTime? FromBirthDay { get; set; }
        public DateTime? ToBirthDay { get; set; }
        public bool IsQuitWork { get; set; }
    }
}
