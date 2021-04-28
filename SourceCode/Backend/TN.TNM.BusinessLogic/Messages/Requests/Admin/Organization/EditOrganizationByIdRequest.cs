using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class EditOrganizationByIdRequest : BaseRequest<EditOrganizationByIdParameter>
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public bool IsFinancialIndependence { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public override EditOrganizationByIdParameter ToParameter()
        {
            return new EditOrganizationByIdParameter()
            {
                OrganizationId = OrganizationId,
                OrganizationName = OrganizationName,
                OrganizationCode = OrganizationCode,
                Address = Address,
                Phone = Phone,
                UserId = UserId,
                IsFinancialIndependence = IsFinancialIndependence
            };
        }
    }
}
