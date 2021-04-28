using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SearchEmployeeParameter : BaseParameter
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string IdentityId { get; set; }
        public List<Guid> ListPosition { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}
