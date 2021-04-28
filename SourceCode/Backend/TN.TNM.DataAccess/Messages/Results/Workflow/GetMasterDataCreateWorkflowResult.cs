using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Workflow
{
    public class GetMasterDataCreateWorkflowResult : BaseResult
    {
        public List<Position> ListPosition { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<SystemFeature> ListSystemFeature { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
