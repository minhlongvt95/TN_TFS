using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailEmployeePayslipRequest : BaseRequest<SendEmailEmployeePayslipParameter>
    {
        public List<Guid> lstEmpMonthySalary { get; set; }

        public override SendEmailEmployeePayslipParameter ToParameter()
        {
            return new SendEmailEmployeePayslipParameter()
            {
                lstEmpMonthySalary = this.lstEmpMonthySalary,
                UserId = this.UserId
            };
        }
    }
}
