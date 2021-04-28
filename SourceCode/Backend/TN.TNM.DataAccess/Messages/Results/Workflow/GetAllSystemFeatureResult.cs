using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Workflow
{
    public class GetAllSystemFeatureResult : BaseResult
    {
        public List<SystemFeature> SystemFeatureList { get; set; }
    }
}
