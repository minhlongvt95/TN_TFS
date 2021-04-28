using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class FindEmployeeMonthySalaryRequest : BaseRequest<FindEmployeeMonthySalaryParameter>
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeUnit { get; set; }
        public string EmployeeBranch { get; set; }
        public Guid? EmployeePostionId { get; set; }
        public List<Guid?> lstEmployeeUnitId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public override FindEmployeeMonthySalaryParameter ToParameter()
        {
            return new FindEmployeeMonthySalaryParameter()
            {
                EmployeeName = EmployeeName,
                EmployeeCode= EmployeeCode,
                EmployeeUnit= EmployeeUnit,
                EmployeeBranch= EmployeeBranch,
                EmployeePostionId= EmployeePostionId,
                lstEmployeeUnitId=lstEmployeeUnitId,
                Month= Month,
                Year= Year,
                UserId = UserId,
            };
        }


    }
}
