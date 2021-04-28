using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class GetOrganizationByUserResult: BaseResult
    {
        public List<DataAccess.Models.OrganizationEntityModel> ListOrganization { get; set; }
        public List<Guid?> ListValidSelectionOrganization { get; set; }
    }
}
