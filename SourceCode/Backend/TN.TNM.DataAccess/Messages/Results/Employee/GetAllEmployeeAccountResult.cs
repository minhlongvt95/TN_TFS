using System.Linq;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllEmployeeAccountResult : BaseResult
    {
        public IQueryable<EmployeeEntityModel> EmployeeAcounts { get; internal set; }
    }
}
