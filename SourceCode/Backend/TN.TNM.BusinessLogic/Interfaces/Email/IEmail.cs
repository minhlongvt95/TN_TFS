using TN.TNM.BusinessLogic.Messages.Requests.Email;
using TN.TNM.BusinessLogic.Messages.Responses.Email;

namespace TN.TNM.BusinessLogic.Interfaces.Email
{
    public interface IEmail
    {
        SendEmailResponse SendEmail(SendEmailRequest request);
        SendEmailAfterEditPicResponse SendEmailAfterEditPic(SendEmailAfterEditPicRequest request);
        SendEmailAfterCreatedLeadResponse SendEmailAfterCreatedLead(SendEmailAfterCreatedLeadRequest request);
        SendEmailAfterCreateNoteResponse SendEmailAfterCreateNote(SendEmailAfterCreateNoteRequest request);
        SendEmailEmployeePayslipResponse SendEmailEmployeePayslip(SendEmailEmployeePayslipRequest request);
        SendEmailTeacherPayslipResponse SendEmailTeacherPayslip(SendEmailTeacherPayslipRequest request);
        SendEmailAssistantPayslipResponse SendEmailAssistantPayslip(SendEmailAssistantPayslipRequest request);
        SendEmailVendorOrderResponse SendEmailVendorOrder(SendEmailVendorOrderRequest request);
        SendEmailForgotPassResponse SendEmailForgotPass(SendEmailForgotPassRequest request);
        SendEmailCustomerOrderResponse SendEmailCustomerOrder(SendEmailCustomerOrderRequest request);
        SendEmailPersonNotifyResponse SendEmailPersonNotify(SendEmailPersonNotifyRequest request);
        SendEmailPersonCreateResponse SendEmailPersonCreate(SendEmailPersonCreateRequest request);
        SendEmailPersonApproveResponse SendEmailPersonApprove(SendEmailPersonApproveRequest request);
    }
}
