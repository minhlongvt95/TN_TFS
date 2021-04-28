using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Email;
using TN.TNM.BusinessLogic.Messages.Requests.Email;
using TN.TNM.BusinessLogic.Messages.Responses.Email;

namespace TN.TNM.Api.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmail iEmail;
        public EmailController(IEmail _iEmail)
        {
            iEmail = _iEmail;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmail")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailResponse SendEmail([FromBody]SendEmailRequest request)
        {
            return iEmail.SendEmail(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailAfterEditPic")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailAfterEditPicResponse SendEmailAfterEditPic([FromBody]SendEmailAfterEditPicRequest request)
        {
            return iEmail.SendEmailAfterEditPic(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailAfterCreatedLead")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailAfterCreatedLeadResponse SendEmailAfterCreatedLead([FromBody]SendEmailAfterCreatedLeadRequest request)
        {
            return iEmail.SendEmailAfterCreatedLead(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailAfterCreateNote")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailAfterCreateNoteResponse SendEmailAfterCreateNote([FromBody]SendEmailAfterCreateNoteRequest request)
        {
            return iEmail.SendEmailAfterCreateNote(request);
        }

        /// <summary>
        /// SendEmailEmployeePayslip
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailEmployeePayslip")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailEmployeePayslipResponse SendEmailEmployeePayslip([FromBody]SendEmailEmployeePayslipRequest request)
        {
            return iEmail.SendEmailEmployeePayslip(request);
        }

        /// <summary>
        /// SendEmailTeacherPayslip
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailTeacherPayslip")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailTeacherPayslipResponse SendEmailTeacherPayslip([FromBody]SendEmailTeacherPayslipRequest request)
        {
            return iEmail.SendEmailTeacherPayslip(request);
        }
        /// <summary>
        /// SendEmailAssistantPayslip
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailAssistantPayslip")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailAssistantPayslipResponse SendEmailAssistantPayslip([FromBody]SendEmailAssistantPayslipRequest request)
        {
            return iEmail.SendEmailAssistantPayslip(request);
        }

        [Route("api/email/sendEmailVendorOrder")]
        [HttpPost]
        [Authorize(Policy = "Member")]
        public SendEmailVendorOrderResponse SendEmailVendorOrder([FromBody]SendEmailVendorOrderRequest request)
        {
            return iEmail.SendEmailVendorOrder(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailForgotPass")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailForgotPassResponse sendEmailForgotPass([FromBody]SendEmailForgotPassRequest request)
        {
            return iEmail.SendEmailForgotPass(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailCustomerOrder")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailCustomerOrderResponse sendEmailCustomerOrder([FromBody]SendEmailCustomerOrderRequest request)
        {
            return iEmail.SendEmailCustomerOrder(request);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailPersonApprove")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailPersonApproveResponse SendEmailPersonApprove([FromBody]SendEmailPersonApproveRequest request)
        {
            return iEmail.SendEmailPersonApprove(request);
        }
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailPersonCreate")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailPersonCreateResponse SendEmailPersonCreate([FromBody]SendEmailPersonCreateRequest request)
        {
            return iEmail.SendEmailPersonCreate(request);
        }
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [Route("api/email/sendEmailPersonNotify")]
        [HttpPost]
        [AllowAnonymous]
        public SendEmailPersonNotifyResponse SendEmailPersonNotify([FromBody]SendEmailPersonNotifyRequest request)
        {
            return iEmail.SendEmailPersonNotify(request);
        }
    }
}