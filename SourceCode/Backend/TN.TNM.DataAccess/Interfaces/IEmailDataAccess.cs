using TN.TNM.DataAccess.Messages.Parameters.Email;
using TN.TNM.DataAccess.Messages.Results.Email;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmailDataAccess
    {
        SendEmailResult SendEmail(SendEmailParameter parameter);
        SendEmailAfterEditPicResult SendEmailAfterEditPic(SendEmailAfterEditPicParameter parameter);
        SendEmailAfterCreatedLeadResult SendEmailAfterCreatedLead(SendEmailAfterCreatedLeadParameter parameter);
        SendEmailAfterCreateNoteResult SendEmailAfterCreateNote(SendEmailAfterCreateNoteParameter parameter);
        SendEmailEmployeePayslipResult SendEmailEmployeePayslip(SendEmailEmployeePayslipParameter parameter);
        SendEmailTeacherPayslipResult SendEmailTeacherPayslip(SendEmailTeacherPayslipParameter parameter);
        SendEmailAssistantPayslipResult SendEmailAssistantPayslip(SendEmailAssistantPayslipParameter parameter);
        SendEmailVendorOrderResult SendEmailVendorOrder(SendEmailVendorOrderParameter parameter);
        SendEmailForgotPassResult SendEmailForgotPass(SendEmailForgotPassParameter parameter);
        SendEmailCustomerOrderResult SendEmailCustomerOrder(SendEmailCustomerOrderParameter parameter);
        SendEmailPersonNotifyResult SendEmailPersonNotify(SendEmailPersonNotifyParameter parameter);
        SendEmailPersonCreateResult SendEmailPersonCreate(SendEmailPersonCreateParameter parameter);
        SendEmailPersonApproveResult SendEmailPersonApprove(SendEmailPersonApproveParameter parameter);
    }
}
