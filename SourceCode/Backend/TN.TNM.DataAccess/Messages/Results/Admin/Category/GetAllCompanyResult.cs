using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Category
{
    public class GetAllCompanyResult : BaseResult
    {
        public List<Databases.Entities.Company> Company { get; set; }
    }
}
