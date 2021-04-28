using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class AssistantSalaryHandmadeParameter:BaseParameter
    {
        public List<IFormFile> FileList { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
