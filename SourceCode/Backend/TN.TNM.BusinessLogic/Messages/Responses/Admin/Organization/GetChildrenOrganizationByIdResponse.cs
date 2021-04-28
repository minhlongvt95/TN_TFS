using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetChildrenOrganizationByIdResponse : BaseResponse
    {
        public List<OrganizationModel> listOrganization { get; set; }
        public List<dynamic> organizationParent { get; set; }
        public bool isManager { get; set; }
    }
}
