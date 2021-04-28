using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SearchEmployeeFromListRequest : BaseRequest<SearchEmployeeFromListParameter>
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
        public bool IsQuitWork{ get; set; }

        public override SearchEmployeeFromListParameter ToParameter() => new SearchEmployeeFromListParameter()
        {
            IsManager = IsManager,
            EmployeeId = EmployeeId,
            FirstName = FirstName,
            LastName = LastName,
            UserName = UserName,
            IdentityId = IdentityId,
            ListPosition = ListPosition,
            OrganizationId = OrganizationId,
            FromContractExpiryDate = FromContractExpiryDate,
            ToContractExpiryDate = ToContractExpiryDate,
            FromBirthDay = FromBirthDay,
            ToBirthDay = ToBirthDay,
            IsQuitWork = IsQuitWork,
            UserId = UserId
        };
    }
}
