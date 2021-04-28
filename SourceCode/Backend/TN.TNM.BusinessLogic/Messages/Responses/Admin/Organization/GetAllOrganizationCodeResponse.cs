using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetAllOrganizationCodeResponse : BaseResponse
    {
        public List<string> OrgCodeList { get; set; }
    }
}
