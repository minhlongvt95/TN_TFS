using System;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Email;
using TN.TNM.BusinessLogic.Messages.Requests.Email;
using TN.TNM.BusinessLogic.Messages.Responses.Email;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.Email
{
    public class EmailFactory : BaseFactory, IEmail
    {
        private IEmailDataAccess iEmailDataAccess;

        public EmailFactory(IEmailDataAccess _iEmailDataAccess, ILogger<EmailFactory> _logger)
        {
            this.iEmailDataAccess = _iEmailDataAccess;
            this.logger = _logger;
        }

        public SendEmailResponse SendEmail(SendEmailRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email after Create Employee");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmail(parameter);
                var response = new SendEmailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailAfterEditPicResponse SendEmailAfterEditPic(SendEmailAfterEditPicRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email after Edit Person In Charge");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailAfterEditPic(parameter);
                var response = new SendEmailAfterEditPicResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailAfterEditPicResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailAfterCreatedLeadResponse SendEmailAfterCreatedLead(SendEmailAfterCreatedLeadRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email after create a new Lead");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailAfterCreatedLead(parameter);
                var response = new SendEmailAfterCreatedLeadResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailAfterCreatedLeadResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailAfterCreateNoteResponse SendEmailAfterCreateNote(SendEmailAfterCreateNoteRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email after create a note");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailAfterCreateNote(parameter);
                var response = new SendEmailAfterCreateNoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailAfterCreateNoteResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailEmployeePayslipResponse SendEmailEmployeePayslip(SendEmailEmployeePayslipRequest request)
        {
            try
            {
                this.logger.LogInformation("SendEmailEmployeePayslip");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailEmployeePayslip(parameter);
                var response = new SendEmailEmployeePayslipResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailEmployeePayslipResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailTeacherPayslipResponse SendEmailTeacherPayslip(SendEmailTeacherPayslipRequest request)
        {
            try
            {
                this.logger.LogInformation("SendEmailTeacherPayslip");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailTeacherPayslip(parameter);
                var response = new SendEmailTeacherPayslipResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailTeacherPayslipResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailAssistantPayslipResponse SendEmailAssistantPayslip(SendEmailAssistantPayslipRequest request)
        {
            try
            {
                this.logger.LogInformation("SendEmailAssistantPayslip");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailAssistantPayslip(parameter);
                var response = new SendEmailAssistantPayslipResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailAssistantPayslipResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailVendorOrderResponse SendEmailVendorOrder(SendEmailVendorOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("SendEmailVendorOrder");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailVendorOrder(parameter);
                var response = new SendEmailVendorOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailVendorOrderResponse()
                {
                    MessageCode = "Không gửi được email cho nhà cung cấp",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
            throw new NotImplementedException();
        }

        public SendEmailForgotPassResponse SendEmailForgotPass(SendEmailForgotPassRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email Guide Forgot Password");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailForgotPass(parameter);
                var response = new SendEmailForgotPassResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailForgotPassResponse()
                {
                    MessageCode = "common.messages.exception",
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailCustomerOrderResponse SendEmailCustomerOrder(SendEmailCustomerOrderRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email Customer Order");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailCustomerOrder(parameter);
                var response = new SendEmailCustomerOrderResponse()
                {
                    StatusCode = result.Status == true ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Found,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailCustomerOrderResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailPersonNotifyResponse SendEmailPersonNotify(SendEmailPersonNotifyRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email Person Notify");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailPersonNotify(parameter);
                var response = new SendEmailPersonNotifyResponse()
                {
                    StatusCode = result.Status == true ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Found,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailPersonNotifyResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailPersonCreateResponse SendEmailPersonCreate(SendEmailPersonCreateRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email Person Create");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailPersonCreate(parameter);
                var response = new SendEmailPersonCreateResponse()
                {
                    StatusCode = result.Status == true ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Found,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailPersonCreateResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }

        public SendEmailPersonApproveResponse SendEmailPersonApprove(SendEmailPersonApproveRequest request)
        {
            try
            {
                this.logger.LogInformation("Send Email Customer Order");
                var parameter = request.ToParameter();
                var result = iEmailDataAccess.SendEmailPersonApprove(parameter);
                var response = new SendEmailPersonApproveResponse()
                {
                    StatusCode = result.Status == true ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Found,
                    MessageCode = result.Message
                };
                return response;
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
                return new SendEmailPersonApproveResponse()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.Forbidden
                };
            }
        }
    }
}
