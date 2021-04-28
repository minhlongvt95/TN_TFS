using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetOrganizationByEmployeeIdResponse : BaseResponse
    {
        public OrganizationModel Organization { get; set; }
    }
}
