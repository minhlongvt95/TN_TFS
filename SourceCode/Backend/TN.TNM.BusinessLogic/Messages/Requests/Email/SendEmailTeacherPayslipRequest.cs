using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailTeacherPayslipRequest : BaseRequest<SendEmailTeacherPayslipParameter>
    {
        public List<Guid> lstEmpMonthySalary { get; set; }

        public override SendEmailTeacherPayslipParameter ToParameter()
        {
            return new SendEmailTeacherPayslipParameter()
            {
                lstEmpMonthySalary = this.lstEmpMonthySalary,
                UserId = this.UserId
            };
        }
    }
}
