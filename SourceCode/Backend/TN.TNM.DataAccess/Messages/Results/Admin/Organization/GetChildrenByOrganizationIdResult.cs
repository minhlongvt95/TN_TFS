using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetChildrenByOrganizationIdResult : BaseResult
    {
        public List<OrganizationEntityModel> OrganizationList { get; set; }
    }
}
