using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Organization
{
    public class EditOrganizationByIdParameter : BaseParameter
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsFinancialIndependence { get; set; }
    }
}
