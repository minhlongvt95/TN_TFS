using System;
using System.Linq;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmployeeAssessmentDAO : BaseDAO, IEmployeeAssessmentDataAccess
    {
        public EmployeeAssessmentDAO(Databases.TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }
        public SearchEmployeeAssessmentResult SearchEmployeeAssessment(SearchEmployeeAssessmentParameter parameter)
        {
            var _empAssessment = context.EmployeeAssessment.Where(emp => emp.EmployeeId == parameter.EmployeeId && emp.Year == parameter.Year).ToList();
            return new SearchEmployeeAssessmentResult()
            {
                Message = "Success",
                Status = true,
                ListEmployeeAssessment = _empAssessment
            };
        }
        public GetAllYearToAssessmentResult GetAllYearToAssessment(GetAllYearToAssessmentParameter parameter)
        {
            var _list = context.EmployeeAssessment.Where(emp => emp.EmployeeId == parameter.EmployeeId).Select(y => y.Year).Distinct().ToList();
            return new GetAllYearToAssessmentResult()
            {
                Message = "Success",
                Status = true,
                ListYear = _list
            };
        }
        public EditEmployeeAssessmentResult EditEmployeeAssessment(EditEmployeeAssessmentParameter parameter)
        {
            var tmp = context.EmployeeAssessment.FirstOrDefault();
            parameter.ListEmployeeAssessment.ForEach(empAss =>
            {
                tmp = context.EmployeeAssessment.FirstOrDefault(_empAss => _empAss.EmployeeId == empAss.EmployeeId && _empAss.Year == empAss.Year && _empAss.Month == empAss.Month);
                if (tmp != null)
                {
                    tmp.Type = empAss.Type;
                    tmp.UpdateById = parameter.UserId;
                    tmp.UpdateDate = DateTime.Now;
                    context.EmployeeAssessment.Update(tmp);
                }
                else
                {
                    empAss.CreateById = parameter.UserId;
                    empAss.CreateDate = DateTime.Now;
                    context.EmployeeAssessment.Add(empAss);
                }
            });
            context.SaveChanges();
            return new EditEmployeeAssessmentResult()
            {
                Message = "Success",
                Status = true
            };
        }
    }
}
