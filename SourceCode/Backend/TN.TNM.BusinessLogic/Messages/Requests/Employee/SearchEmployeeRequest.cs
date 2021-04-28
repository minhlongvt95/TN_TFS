using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SearchEmployeeRequest : BaseRequest<SearchEmployeeParameter>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string IdentityId { get; set; }
        public List<Guid> ListPosition { get; set; }
        public Guid? OrganizationId { get; set; }

        public override SearchEmployeeParameter ToParameter() => new SearchEmployeeParameter()
        {
            FirstName = FirstName,
            LastName = LastName,
            UserName = UserName,
            IdentityId = IdentityId,
            ListPosition = ListPosition,
            OrganizationId = OrganizationId,
            UserId = UserId
        };
    }
}
