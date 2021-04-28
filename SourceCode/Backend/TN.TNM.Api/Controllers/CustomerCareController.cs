using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.CustomerCare;
using TN.TNM.BusinessLogic.Messages.Requests.CustomerCare;
using TN.TNM.BusinessLogic.Messages.Responses.CustomerCare;

namespace TN.TNM.Api.Controllers
{
    public class CustomerCareController : Controller
    {
        private readonly ICustomerCare _iCustomerCare;
        public CustomerCareController(ICustomerCare iCustomerCare)
        {
            this._iCustomerCare = iCustomerCare;
        }

        /// <summary>
        /// Create CustomerCare
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/createCustomerCare")]
        [Authorize(Policy = "Member")]
        public CreateCustomerCareResponse CreateCustomerCare([FromBody] CreateCustomerCareRequest request)
        {
            return this._iCustomerCare.CreateCustomerCare(request);
        }
        /// <summary>
        /// Update CustomerCare
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateCustomerCare")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerCareResponse UpdateCustomerCare([FromBody] UpdateCustomerCareRequest request)
        {
            return this._iCustomerCare.UpdateCustomerCare(request);
        }
        /// <summary>
        /// Get CustomerCare ById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerCareById")]
        [Authorize(Policy = "Member")]
        public GetCustomerCareByIdResponse GetCustomerCareById([FromBody] GetCustomerCareByIdRequest request)
        {
            return this._iCustomerCare.GetCustomerCareById(request);
        }
        /// <summary>
        /// Create CustomerCare FeedBack
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/createCustomerCareFeedBack")]
        [Authorize(Policy = "Member")]
        public CreateCustomerCareFeedBackResponse CreateCustomerCareFeedBack([FromBody] CreateCustomerCareFeedBackRequest request)
        {
            return this._iCustomerCare.CreateCustomerCareFeedBack(request);
        }
        /// <summary>
        /// Update CustomerCareFeedBack
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateCustomerCareFeedBack")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerCareFeedBackResponse UpdateCustomerCareFeedBack([FromBody] UpdateCustomerCareFeedBackRequest request)
        {
            return this._iCustomerCare.UpdateCustomerCareFeedBack(request);
        }

        /// <summary>
        /// FilterCustomer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/filterCustomer")]
        [Authorize(Policy = "Member")]
        public FilterCustomerResponse FilterCustomer([FromBody] FilterCustomerRequest request)
        {
            return this._iCustomerCare.FilterCustomer(request);
        }

        /// <summary>
        /// SearchCustomerCare
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/searchCustomerCare")]
        [Authorize(Policy = "Member")]
        public SearchCustomerCareResponse SearchCustomerCare([FromBody] SearchCustomerCareRequest request)
        {
            return this._iCustomerCare.SearchCustomerCare(request);
        }

        /// <summary>
        /// Update Status Customer Care Customer By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateStatusCustomerCareCustomerById")]
        [Authorize(Policy = "Member")]
        public UpdateStatusCustomerCareCustomerByIdResponse UpdateStatusCustomerCareCustomerById([FromBody] UpdateStatusCustomerCareCustomerByIdRequest request)
        {
            return this._iCustomerCare.UpdateStatusCustomerCareCustomerById(request);
        }

        /// <summary>
        /// Get TimeLine Customer Care By CustomerId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getTimeLineCustomerCareByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetTimeLineCustomerCareByCustomerIdResponse GetTimeLineCustomerCareByCustomerId([FromBody] GetTimeLineCustomerCareByCustomerIdRequest request)
        {
            return this._iCustomerCare.GetTimeLineCustomerCareByCustomerId(request);
        }

        /// <summary>
        /// Get CustomerCareFeedBack By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerCareFeedBackByCusIdAndCusCareId")]
        [Authorize(Policy = "Member")]
        public GetCustomerCareFeedBackByCusIdAndCusCareIdResponse GetCustomerCareFeedBackByCusIdAndCusCareId([FromBody] GetCustomerCareFeedBackByCusIdAndCusCareIdRequest request)
        {
            return this._iCustomerCare.GetCustomerCareFeedBackByCusIdAndCusCareId(request);
        }

        /// <summary>
        /// Send Quick Email
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/sendQuickEmail")]
        [Authorize(Policy = "Member")]
        public SendQuickEmailResponse SendQuickEmail([FromBody] SendQuickEmailRequest request)
        {
            return this._iCustomerCare.SendQuickEmail(request);
        }

        /// <summary>
        /// Send Quick SMS
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/sendQuickSMS")]
        [Authorize(Policy = "Member")]
        public SendQuickSMSResponse SendQuickSMS([FromBody] SendQuickSMSRequest request)
        {
            return this._iCustomerCare.SendQuickSMS(request);
        }

        /// <summary>
        /// Send Quick Gift
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/SendQuickGift")]
        [Authorize(Policy = "Member")]
        public SendQuickGiftResponse SendQuickGift([FromBody] SendQuickGiftRequest request)
        {
            return this._iCustomerCare.SendQuickGift(request);
        }

        /// <summary>
        /// Update Status Customer Care
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateStatusCustomerCare")]
        [Authorize(Policy = "Member")]
        public UpdateStatusCustomerCareResponse UpdateStatusCustomerCare([FromBody] UpdateStatusCustomerCareRequest request)
        {
            return this._iCustomerCare.UpdateStatusCustomerCare(request);
        }

        /// <summary>
        /// GetTotalInteractive
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getTotalInteractive")]
        [Authorize(Policy = "Member")]
        public GetTotalInteractiveResponse GetTotalInteractive([FromBody] GetTotalInteractiveRequest request)
        {
            return this._iCustomerCare.GetTotalInteractive(request);
        }

        /// <summary>
        /// Update Status Customer Care
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerBirthDay")]
        [Authorize(Policy = "Member")]
        public GetCustomerBirthDayResponse GetCustomerBirthDay([FromBody] GetCustomerBirthDayRequest request)
        {
            return this._iCustomerCare.GetCustomerBirthDay(request);
        }

        /// <summary>
        /// GetCustomerNewCS
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerNewCS")]
        [Authorize(Policy = "Member")]
        public GetCustomerNewCSResponse GetCustomerNewCS([FromBody] GetCustomerNewCSRequest request)
        {
            return this._iCustomerCare.GetCustomerNewCS(request);
        }

        /// <summary>
        /// GetCustomerCareActive
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCustomerCareActive")]
        [Authorize(Policy = "Member")]
        public GetCustomerCareActiveResponse GetCustomerCareActive([FromBody] GetCustomerCareActiveRequest request)
        {
            return this._iCustomerCare.GetCustomerCareActive(request);
        }

        /// <summary>
        /// GetCustomerCareActive
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getCharCustomerCS")]
        [Authorize(Policy = "Member")]
        public GetCharCustomerCSResponse GetCharCustomerCS([FromBody] GetCharCustomerCSRequest request)
        {
            return this._iCustomerCare.GetCharCustomerCS(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/getMasterDataCustomerCareList")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCustomerCareListResponse GetMasterDataCustomerCareList([FromBody] GetMasterDataCustomerCareListRequest request)
        {
            return this._iCustomerCare.GetMasterDataCustomerCareList(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateStatusCusCare")]
        [Authorize(Policy = "Member")]
        public UpdateStatusCusCareResponse UpdateStatusCusCare([FromBody] UpdateStatusCusCareRequest request)
        {
            return this._iCustomerCare.UpdateStatusCusCare(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/updateCustomerMeeting")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerMeetingResponse UpdateCustomerMeeting([FromBody] UpdateCustomerMeetingRequest request)
        {
            return this._iCustomerCare.UpdateCustomerMeeting(request);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customerCare/removeCustomerMeeting")]
        [Authorize(Policy = "Member")]
        public RemoveCustomerMeetingResponse RemoveCustomerMeeting([FromBody] RemoveCustomerMeetingRequest request)
        {
            return this._iCustomerCare.RemoveCustomerMeeting(request);
        }
    }
}