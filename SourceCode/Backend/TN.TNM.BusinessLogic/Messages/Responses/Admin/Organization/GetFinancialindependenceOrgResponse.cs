using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetFinancialindependenceOrgResponse : BaseResponse
    {
        public List<OrganizationModel> ListOrg { get; set; }
    }
}
