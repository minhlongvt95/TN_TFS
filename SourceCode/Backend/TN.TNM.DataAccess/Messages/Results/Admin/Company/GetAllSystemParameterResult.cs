using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Company
{
    public class GetAllSystemParameterResult : BaseResult
    {
        public List<SystemParameter> systemParameterList { get; set; }
    }
}
