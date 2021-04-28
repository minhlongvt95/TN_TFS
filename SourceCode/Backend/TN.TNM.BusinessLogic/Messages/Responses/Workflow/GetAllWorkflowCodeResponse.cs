using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Workflow
{
    public class GetAllWorkflowCodeResponse : BaseResponse
    {
        public List<string> CodeList { get; set; }
    }
}
