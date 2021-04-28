using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Company
{
    public class ChangeSystemParameterResponse : BaseResponse
    {
        public List<SystemParameter> SystemParameterList { get; set; }
    }
}
