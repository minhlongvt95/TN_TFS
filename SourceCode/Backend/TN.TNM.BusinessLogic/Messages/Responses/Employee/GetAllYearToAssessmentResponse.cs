using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetAllYearToAssessmentResponse : BaseResponse
    {
        public List<int?> ListYear { get; set; }
    }
}
