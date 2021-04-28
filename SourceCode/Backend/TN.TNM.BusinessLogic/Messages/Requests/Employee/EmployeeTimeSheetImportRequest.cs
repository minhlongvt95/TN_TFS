using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EmployeeTimeSheetImportRequest : BaseRequest<EmployeeTimeSheetImportParameter>
    {
        public List<IFormFile> FileList { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public override EmployeeTimeSheetImportParameter ToParameter()
        {
            return new EmployeeTimeSheetImportParameter()
            {
                UserId = UserId,
                Month=Month,
                Year=Year,
                FileList=this.FileList
            };
        }
    }
}
