using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Workflow;

namespace TN.TNM.BusinessLogic.Messages.Responses.Workflow
{
    public class GetAllSystemFeatureResponse : BaseResponse
    {
        public List<SystemFeatureModel> SystemFeatureList { get; set; }
    }
}
