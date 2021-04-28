using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class AssistantSalaryHandmadeRequest : BaseRequest<AssistantSalaryHandmadeParameter>
    {
        public List<IFormFile> FileList { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public override AssistantSalaryHandmadeParameter ToParameter()
        {
            return new AssistantSalaryHandmadeParameter()
            {
                FileList = this.FileList,
                Month = this.Month,
                Year = this.Year,
                UserId = this.UserId
            };
        }
    }
}
