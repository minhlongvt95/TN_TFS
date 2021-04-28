using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetChildrenByOrganizationIdResponse : BaseResponse
    {
        public List<OrganizationModel> OrganizationList { get; set; }
    }
}
