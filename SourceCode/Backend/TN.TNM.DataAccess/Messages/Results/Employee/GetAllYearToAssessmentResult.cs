using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllYearToAssessmentResult : BaseResult
    {
        public List<int?> ListYear { get; set; }
    }
}
