using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetChildrenOrganizationByIdResult : BaseResult
    {
        public List<OrganizationEntityModel> listOrganization { get; set; }
        public List<dynamic> organizationParent { get; set; }
        public bool isManager { get; set; }
    }
}
