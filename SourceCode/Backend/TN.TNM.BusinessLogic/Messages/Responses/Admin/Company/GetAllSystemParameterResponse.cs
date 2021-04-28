using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Company;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Company
{
    public class GetAllSystemParameterResponse : BaseResponse
    {
        public List<SystemParameterModel> systemParameterList { get; set; }
    }
}
