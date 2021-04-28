using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Email
{
    public class SearchEmailConfigMasterdataResult: BaseResult
    {
        public List<DataAccess.Databases.Entities.Category> ListEmailType { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListEmailStatus { get; set; }
    }
}
