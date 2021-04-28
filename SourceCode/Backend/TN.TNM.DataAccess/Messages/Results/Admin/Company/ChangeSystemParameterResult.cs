using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Company
{
    public class ChangeSystemParameterResult : BaseResult
    {
        public List<SystemParameter> SystemParameterList { get; set; }
    }
}
