using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetOrganizationByIdResponse : BaseResponse
    {
        public OrganizationModel Organization { get; set; }
    }
}
