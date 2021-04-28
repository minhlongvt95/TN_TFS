using System;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmployeeInsuranceDAO : BaseDAO, IEmployeeInsuranceDataAccess
    {
        public EmployeeInsuranceDAO(TNTN8Context _context, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _context;
            this.iAuditTrace = _iAuditTrace;
        }
        public CreateEmployeeInsuranceResult CreateEmployeeInsurance (CreateEmployeeInsuranceParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.ADD, ObjectName.EMPLOYEE, "Create employee insurance", parameter.UserId);
            parameter.EmployeeInsurance.EffectiveDate = DateTime.Now;
            parameter.EmployeeInsurance.CreateById = parameter.UserId;
            parameter.EmployeeInsurance.CreateDate = DateTime.Now;
            // Luu vao DB
            context.EmployeeInsurance.Add(parameter.EmployeeInsurance);
            context.SaveChanges();

            return new CreateEmployeeInsuranceResult()
            {
                Message = "Success",
                Status = true
            };
        }

        public EditEmployeeInsuranceResult EditEmployeeInsurance (EditEmployeeInsuranceParameter parameter)
        {
            if (parameter.EmployeeInsurance?.HealthInsurancePercent < 0 || parameter.EmployeeInsurance?.HealthInsuranceSupportPercent < 0 || parameter.EmployeeInsurance?.SocialInsurancePercent < 0 || parameter.EmployeeInsurance?.SocialInsuranceSalary < 0 || parameter.EmployeeInsurance?.SocialInsuranceSupportPercent < 0 || parameter.EmployeeInsurance?.UnemploymentinsurancePercent < 0 || parameter.EmployeeInsurance?.UnemploymentinsuranceSupportPercent < 0)
            {
                return new EditEmployeeInsuranceResult()
                {
                    Message = "Failed",
                    Status = false
                };
            }
         
            this.iAuditTrace.Trace(ActionName.ADD, ObjectName.EMPLOYEE, "Edit employee insurance", parameter.UserId);
            parameter.EmployeeInsurance.EffectiveDate = DateTime.Now;
            var _empisr = context.EmployeeInsurance.FirstOrDefault(emp => emp.EmployeeId == parameter.EmployeeInsurance.EmployeeId && emp.EffectiveDate.Value.Date == parameter.EmployeeInsurance.EffectiveDate.Value.Date);
            if (_empisr != null)
            {
                _empisr.EffectiveDate = parameter.EmployeeInsurance.EffectiveDate;
                _empisr.HealthInsurancePercent = parameter.EmployeeInsurance.HealthInsurancePercent;
                _empisr.HealthInsuranceSupportPercent = parameter.EmployeeInsurance.HealthInsuranceSupportPercent;
                _empisr.SocialInsurancePercent = parameter.EmployeeInsurance.SocialInsurancePercent;
                _empisr.SocialInsuranceSalary = parameter.EmployeeInsurance.SocialInsuranceSalary;
                _empisr.SocialInsuranceSupportPercent = parameter.EmployeeInsurance.SocialInsuranceSupportPercent;
                _empisr.UnemploymentinsurancePercent = parameter.EmployeeInsurance.UnemploymentinsurancePercent;
                _empisr.UnemploymentinsuranceSupportPercent = parameter.EmployeeInsurance.UnemploymentinsuranceSupportPercent;
                _empisr.UpdateById = parameter.UserId;
                _empisr.UpdateDate = DateTime.Now;
                context.EmployeeInsurance.Update(_empisr);
            }
            else
            {
                parameter.EmployeeInsurance.CreateById = parameter.UserId;
                parameter.EmployeeInsurance.CreateDate = DateTime.Now;
                context.EmployeeInsurance.Add(parameter.EmployeeInsurance);
            }
            context.SaveChanges();
            return new EditEmployeeInsuranceResult()
            {
                Message = "Success",
                Status = true
            };
        }

        public SearchEmployeeInsuranceResult SearchEmployeeInsurance (SearchEmployeeInsuranceParameter parameter)
        {
            this.iAuditTrace.Trace(ActionName.ADD, ObjectName.EMPLOYEE, "Search employee insurance", parameter.UserId);
            // Search all Employee Insurance with parameter
            var _ListEmployeeInsurance = context.EmployeeInsurance.Where(empisr => (parameter.EmployeeId == empisr.EmployeeId || parameter.EmployeeId == null) &&
                                                                                  (parameter.EmployeeInsuranceId == empisr.EmployeeInsuranceId || parameter.EmployeeInsuranceId == null)).OrderByDescending(t => t.EffectiveDate.Value.Date).ToList();

            return new SearchEmployeeInsuranceResult()
            {
                Message = "Success",
                ListEmployeeInsurance = _ListEmployeeInsurance,
                Status = true
            };
        }
        
    }
}
