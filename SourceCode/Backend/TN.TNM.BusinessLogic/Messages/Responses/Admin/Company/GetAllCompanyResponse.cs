using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Company;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Company
{
    public class GetAllCompanyResponse : BaseResponse
    {
        public List<CompanyModel> Company { get; set; }
    }
}
