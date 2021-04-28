using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class EmployeeSalaryHandmadeRequest:BaseRequest<EmployeeSalaryHandmadeParameter>
    {
        public List<IFormFile> FileList { get; set; }

        public override EmployeeSalaryHandmadeParameter ToParameter()
        {
            return new EmployeeSalaryHandmadeParameter()
            {
                UserId = UserId,
                FileList = this.FileList
            };
        }

    }
}
