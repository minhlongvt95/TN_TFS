using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetFinancialindependenceOrgResult : BaseResult
    {
        public List<Databases.Entities.Organization> ListOrg { get; set; }
    }
}
