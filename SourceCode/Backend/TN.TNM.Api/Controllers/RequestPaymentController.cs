using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.RequestPayment;
using TN.TNM.BusinessLogic.Messages.Requests.RequestPayment;
using TN.TNM.BusinessLogic.Messages.Responses.RequestPayment;

namespace TN.TNM.Api.Controllers
{
    public class RequestPaymentController : Controller
    {
        private readonly IRequestPayment _iRequestPayment;
        public RequestPaymentController(IRequestPayment iRequestPayment)
        {
            this._iRequestPayment = iRequestPayment;
        }
        /// <summary>
        /// Create Request Payment
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/requestPayment/createRequestPayment")]
        [Authorize(Policy = "Member")]
        public CreateRequestPaymentResponse CreateRequestPayment(CreateRequestPaymentRequest request)
        {
            return this._iRequestPayment.CreateRequestPayment(request);
        }

        /// <summary>
        /// Find Request Payment
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/requestPayment/findRequestPayment")]
        [Authorize(Policy = "Member")]
        public FindRequestPaymentResponse FindRequestPayment([FromBody]FindRequestPaymentRequest request)
        {
            return this._iRequestPayment.FindRequestPayment(request);
        }
        /// <summary>
        /// EditRequestPayment
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/requestPayment/editRequestPayment")]
        [Authorize(Policy = "Member")]
        public EditRequestPaymentResponse EditRequestPayment(EditRequestPaymentRequest request)
        {
            return this._iRequestPayment.EditRequestPayment(request);
        }
        /// <summary>
        /// GetRequestPaymentById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/requestPayment/getRequestPaymentById")]
        [Authorize(Policy = "Member")]
        public GetRequestPaymentByIdResponse GetRequestPaymentById([FromBody]GetRequestPaymentByIdRequest request)
        {
            return this._iRequestPayment.GetRequestPaymentById(request);
        }

    }
}