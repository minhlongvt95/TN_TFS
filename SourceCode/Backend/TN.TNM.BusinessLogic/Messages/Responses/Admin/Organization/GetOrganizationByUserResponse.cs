using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetOrganizationByUserResponse: BaseResponse
    {
        public List<DataAccess.Models.OrganizationEntityModel> ListOrganization { get; set; }
        public List<Guid?> ListValidSelectionOrganization { get; set; }
    }
}
