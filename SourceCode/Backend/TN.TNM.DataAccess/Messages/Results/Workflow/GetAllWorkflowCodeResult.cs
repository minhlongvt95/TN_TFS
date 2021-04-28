using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Workflow
{
    public class GetAllWorkflowCodeResult : BaseResult
    {
        public List<string> CodeList { get; set; }
    }
}
