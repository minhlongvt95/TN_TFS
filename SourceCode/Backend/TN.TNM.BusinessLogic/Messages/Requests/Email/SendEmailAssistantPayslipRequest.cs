using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Email;

namespace TN.TNM.BusinessLogic.Messages.Requests.Email
{
    public class SendEmailAssistantPayslipRequest : BaseRequest<SendEmailAssistantPayslipParameter>
    {
        public List<Guid> lstEmpMonthySalary { get; set; }

        public override SendEmailAssistantPayslipParameter ToParameter()
        {
            return new SendEmailAssistantPayslipParameter()
            {
                lstEmpMonthySalary = lstEmpMonthySalary,
                UserId = this.UserId
            };
        }
    }
}
