using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetAllOrganizationCodeResult : BaseResult
    {
        public List<string> OrgCodeList { get; set; }
    }
}
