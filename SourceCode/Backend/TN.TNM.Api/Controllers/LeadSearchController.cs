using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Lead;
using TN.TNM.BusinessLogic.Messages.Requests.Lead;
using TN.TNM.BusinessLogic.Messages.Responses.Leads;

/// <summary>
/// A short description about this controller
/// 
/// 1/ A short description about 1st function
/// 2/ A short description about 2nd function
/// 3/ ...
/// 
/// Author: thanhhh@tringhiatech.vn
/// Date: 13/06/2016
/// </summary>
namespace TN.TNM.Api.Controllers
{
    public class LeadSearchController : Controller
    {
        private readonly ILeadSearch iLead;
        public LeadSearchController(ILeadSearch _iLead)
        {
            this.iLead = _iLead;
        }

        /// <summary>
        /// Search lead by parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getAllLead")]
        [Authorize(Policy = "Member")]
        public GetAllLeadResponse GetAllLead([FromBody]GetAllLeadRequest request)
        {
            return this.iLead.GetAllLead(request);
        }

        /// <summary>
        /// Create a new lead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lead/create")]
        [Authorize(Policy = "Member")]
        public CreateLeadResponse CreateLead([FromBody]CreateLeadRequest request)
        {
            return this.iLead.CreateLead(request);
        }

        /// <summary>
        /// GetLeadById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lead/getLeadById")]
        [Authorize(Policy = "Member")]
        public GetLeadByIdResponse GetLeadById([FromBody]GetLeadByIdRequest request)
        {
            return this.iLead.GetLeadById(request);
        }

        /// <summary>
        /// EditLeadById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lead/editLeadById")]
        [Authorize(Policy = "Member")]
        public EditLeadByIdResponse EditLeadById([FromBody]EditLeadByIdRequest request)
        {
            return this.iLead.EditLeadById(request);
        }
        /// <summary>
        /// GetNoteHistory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lead/getNoteHistory")]
        [Authorize(Policy = "Member")]
        public GetNoteHistoryResponse GetNoteHistory([FromBody] GetNoteHistoryRequest request)
        {
            return this.iLead.GetNoteHistory(request);
        }

        /// <summary>
        /// Search lead by parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getLeadByStatus")]
        [Authorize(Policy = "Member")]
        public GetLeadByStatusResponse GetLeadByStatus([FromBody]GetLeadByStatusRequest request)
        {
            return this.iLead.GetLeadByStatus(request);
        }

        /// <summary>
        /// Search lead by parameter
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getLeadByName")]
        [Authorize(Policy = "Member")]
        public GetLeadByNameResponse GetLeadByName([FromBody]GetLeadByNameRequest request)
        {
            return this.iLead.GetLeadByName(request);
        }

        /// <summary>
        /// Get all employee which in the same organization of current user
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getEmployeeWithNotificationPermisison")]
        [Authorize(Policy = "Member")]
        public GetEmployeeWithNotificationPermisisonResponse GetEmployeeWithNotificationPermisison([FromBody]GetEmployeeWithNotificationPermisisonRequest request)
        {
            return this.iLead.GetEmployeeWithNotificationPermisison(request);
        }

        /// <summary>
        /// Change status to Unfollow
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/changeLeadStatusToUnfollow")]
        [Authorize(Policy = "Member")]
        public ChangeLeadStatusToUnfollowResponse ChangeLeadStatusToUnfollow([FromBody]ChangeLeadStatusToUnfollowRequest request)
        {
            return this.iLead.ChangeLeadStatusToUnfollow(request);
        }

        /// <summary>
        /// Approve or Reject change status to Unfollow
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/approveRejectUnfollowLead")]
        [Authorize(Policy = "Member")]
        public ApproveRejectUnfollowLeadResponse ApproveRejectUnfollowLead([FromBody]ApproveRejectUnfollowLeadRequest request)
        {
            return this.iLead.ApproveRejectUnfollowLead(request);
        }

        /// <summary>
        /// Get all Employee's manager
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getEmployeeManager")]
        [Authorize(Policy = "Member")]
        public GetEmployeeManagerResponse GetEmployeeManager([FromBody]GetEmployeeManagerRequest request)
        {
            return this.iLead.GetEmployeeManager(request);
        }

        /// <summary>
        /// Send Email Lead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/sendEmailLead")]
        [Authorize(Policy = "Member")]
        public SendEmailLeadResponse SendEmailLead([FromBody]SendEmailLeadRequest request)
        {
            return this.iLead.SendEmailLead(request);
        }
        
        /// <summary>
        /// Send SMS Lead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/sendSMSLead")]
        [Authorize(Policy = "Member")]
        public SendSMSLeadResponse SendSMSLead([FromBody]SendSMSLeadRequest request)
        {
            return this.iLead.SendSMSLead(request);
        }
        /// <summary>
        /// Send SMS Lead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/importLead")]
        [Authorize(Policy = "Member")]
        public ImportLeadResponse ImportLead(ImportLeadRequest request)
        {
            return this.iLead.ImportLead(request);
        }
        /// <summary>
        /// UpdateLeadDuplicate
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/updateLeadDuplicate")]
        [Authorize(Policy = "Member")]
        public UpdateLeadDuplicateResponse UpdateLeadDuplicate([FromBody]UpdateLeadDuplicateRequest request)
        {
            return this.iLead.UpdateLeadDuplicate(request);
        }

        /// <summary>
        /// DownloadTemplateCustomer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/downloadTemplateLead")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateLeadResponse DownloadTemplateCustomer([FromBody]DownloadTemplateLeadRequest request)
        {
            return this.iLead.DownloadTemplateLead(request);
        }

        /// <summary>
        /// ChangeLeadStatusToDelete
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/changeLeadStatusToDelete")]
        [Authorize(Policy = "Member")]
        public ChangeLeadStatusToDeleteResponse ChangeLeadStatusToDelete([FromBody]ChangeLeadStatusToDeleteRequest request)
        {
            return this.iLead.ChangeLeadStatusToDelete(request);
        }

        /// <summary>
        /// CheckEmailLead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/checkEmailLead")]
        [Authorize(Policy = "Member")]
        public CheckEmailLeadResponse CheckEmailLead([FromBody]CheckEmailLeadRequest request)
        {
            return this.iLead.CheckEmailLead(request);
        }

        /// <summary>
        /// CheckPhoneLead
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/checkPhoneLead")]
        [Authorize(Policy = "Member")]
        public CheckPhoneLeadResponse CheckPhoneLead([FromBody]CheckPhoneLeadRequest request)
        {
            return this.iLead.CheckPhoneLead(request);
        }

        /// <summary>
        /// Get Person In Charge
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/getPersonInCharge")]
        [Authorize(Policy = "Member")]
        public GetPersonInChargeResponse GetPersonInCharge([FromBody]GetPersonInChargeRequest request)
        {
            return this.iLead.GetPersonInCharge(request);
        }

        /// <summary>
        /// Edit Person In Charge
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/editPersonInCharge")]
        [Authorize(Policy = "Member")]
        public EditPersonInChargeResponse EditPersonInCharge([FromBody]EditPersonInChargeRequest request)
        {
            return this.iLead.EditPersonInCharge(request);
        }

        /// <summary>
        /// Edit Lead Status By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Lead</returns>
        [HttpPost]
        [Route("api/lead/editLeadStatusById")]
        [Authorize(Policy = "Member")]
        public EditLeadStatusByIdResponse EditLeadStatusById([FromBody]EditLeadStatusByIdRequest request)
        {
            return this.iLead.EditLeadStatusById(request);
        }

        [HttpPost]
        [Route("api/lead/getDataCreateLead")]
        [Authorize(Policy = "Member")]
        public GetDataCreateLeadResponse GetDataCreateLead([FromBody]GetDataCreateLeadRequest request)
        {
            return this.iLead.GetDataCreateLead(request);
        }

        [HttpPost]
        [Route("api/lead/getListCustomerByType")]
        [Authorize(Policy = "Member")]
        public GetListCustomerByTypeResponse GetListCustomerByType([FromBody]GetListCustomerByTypeRequest request)
        {
            return this.iLead.GetListCustomerByType(request);
        }

        [HttpPost]
        [Route("api/lead/getDatEditLead")]
        [Authorize(Policy = "Member")]
        public GetDataEditLeadResponse GetDataEditLead([FromBody]GetDataEditLeadRequest request)
        {
            return this.iLead.GetDataEditLead(request);
        }

        [HttpPost]
        [Route("api/lead/getDataSearchLead")]
        [Authorize(Policy = "Member")]
        public GetDataSearchLeadResponse GetDataSearchLead([FromBody]GetDataSearchLeadRequest request)
        {
            return this.iLead.GetDataSearchLead(request);
        }

        [HttpPost]
        [Route("api/lead/approveOrRejectLeadUnfollow")]
        [Authorize(Policy = "Member")]
        public ApproveOrRejectLeadUnfollowResponse ApproveOrRejectLeadUnfollow([FromBody]ApproveOrRejectLeadUnfollowRequest request)
        {
            return this.iLead.ApproveOrRejectLeadUnfollow(request);
        }

        [HttpPost]
        [Route("api/lead/setPersonalInChange")]
        [Authorize(Policy = "Member")]
        public SetPersonalInChangeResponse SetPersonalInChange([FromBody]SetPersonalInChangeRequest request)
        {
            return this.iLead.SetPersonalInChange(request);
        }

        [HttpPost]
        [Route("api/lead/unfollowListLead")]
        [Authorize(Policy = "Member")]
        public UnfollowListLeadResponse UnfollowListLead([FromBody]UnfollowListLeadRequest request)
        {
            return this.iLead.UnfollowListLead(request);
        }

        [HttpPost]
        [Route("api/lead/importLeadDetail")]
        [Authorize(Policy = "Member")]
        public ImportLeadDetailResponse ImportLeadDetail([FromBody]ImportLeadDetailRequest request)
        {
            return this.iLead.ImportLeadDetail(request);
        }

        [HttpPost]
        [Route("api/lead/importListLead")]
        [Authorize(Policy = "Member")]
        public ImportListLeadResponse ImportListLead([FromBody]ImportListLeadRequest request)
        {
            return this.iLead.ImportListLead(request);
        }

        [HttpPost]
        [Route("api/lead/getDataLeadProductDialog")]
        [Authorize(Policy = "Member")]
        public GetDataLeadProductDialogResponse GetDataLeadProductDialog([FromBody]GetDataLeadProductDialogRequest request)
        {
            return this.iLead.GetDataLeadProductDialog(request);
        }

        [HttpPost]
        [Route("api/lead/sendEmailSupportLead")]
        [Authorize(Policy = "Member")]
        public SendEmailSupportLeadResponse SendEmailSupportLead([FromBody]SendEmailSupportLeadRequest request)
        {
            return this.iLead.SendEmailSupportLead(request);
        }

        [HttpPost]
        [Route("api/lead/sendSMSSupportLead")]
        [Authorize(Policy = "Member")]
        public SendSMSSupportLeadResponse SendSMSSupportLead([FromBody]SendSMSSupportLeadRequest request)
        {
            return this.iLead.SendSMSSupportLead(request);
        }

        [HttpPost]
        [Route("api/lead/sendGiftSupportLead")]
        [Authorize(Policy = "Member")]
        public SendGiftSupportLeadResponse SendGiftSupportLead([FromBody]SendGiftSupportLeadRequest request)
        {
            return this.iLead.SendGiftSupportLead(request);
        }

        [HttpPost]
        [Route("api/lead/createLeadMeeting")]
        [Authorize(Policy = "Member")]
        public CreateLeadMeetingResponse CreateLeadMeeting([FromBody]CreateLeadMeetingRequest request)
        {
            return this.iLead.CreateLeadMeeting(request);
        }

        [HttpPost]
        [Route("api/lead/changeLeadStatus")]
        [Authorize(Policy = "Member")]
        public ChangeLeadStatusResponse ChangeLeadStatus([FromBody]ChangeLeadStatusRequest request)
        {
            return this.iLead.ChangeLeadStatus(request);
        }

        [HttpPost]
        [Route("api/lead/getHistoryLeadCare")]
        [Authorize(Policy = "Member")]
        public GetHistoryLeadCareResponse GetHistoryLeadCare([FromBody]GetHistoryLeadCareRequest request)
        {
            return this.iLead.GetHistoryLeadCare(request);
        }

        [HttpPost]
        [Route("api/lead/getDataPreviewLeadCare")]
        [Authorize(Policy = "Member")]
        public GetDataPreviewLeadCareResponse GetDataPreviewLeadCare([FromBody]GetDataPreviewLeadCareRequest request)
        {
            return this.iLead.GetDataPreviewLeadCare(request);
        }

        [HttpPost]
        [Route("api/lead/getDataLeadCareFeedBack")]
        [Authorize(Policy = "Member")]
        public GetDataLeadCareFeedBackResponse GetDataLeadCareFeedBack([FromBody]GetDataLeadCareFeedBackRequest request)
        {
            return this.iLead.GetDataLeadCareFeedBack(request);
        }

        [HttpPost]
        [Route("api/lead/saveLeadCareFeedBack")]
        [Authorize(Policy = "Member")]
        public SaveLeadCareFeedBackResponse SaveLeadCareFeedBack([FromBody]SaveLeadCareFeedBackRequest request)
        {
            return this.iLead.SaveLeadCareFeedBack(request);
        }
       
        [HttpPost]
        [Route("api/lead/getHistoryLeadMeeting")]
        [Authorize(Policy = "Member")]
        public GetHistoryLeadMeetingResponse GetHistoryLeadMeeting([FromBody]GetHistoryLeadMeetingRequest request)
        {
            return this.iLead.GetHistoryLeadMeeting(request);
        }

        [HttpPost]
        [Route("api/lead/getDataLeadMeetintById")]
        [Authorize(Policy = "Member")]
        public GetDataLeadMeetingByIdResponse GetDataLeadMeetintById([FromBody]GetDataLeadMeetingByIdRequest request)
        {
            return this.iLead.getDataLeadMeetingById(request);
        }

        [HttpPost]
        [Route("api/lead/cloneLead")]
        [Authorize(Policy = "Member")]
        public CloneLeadResponse ReplyLead([FromBody]CloneLeadRequest request)
        {
            return this.iLead.CloneLead(request);
        }

        [HttpPost]
        [Route("api/lead/getEmployeeSeller")]
        [Authorize(Policy = "Member")]
        public GetEmployeeSellerResponse GetEmployeeSeller([FromBody]GetEmployeeSellerRequest request)
        {
            return this.iLead.GetEmployeeSeller(request);
        }
        
        [HttpPost]
        [Route("api/lead/getVendorByProductId")]
        [Authorize(Policy = "Member")]
        public GetVendorByProductIdResponse GetVendorByProductId([FromBody]GetVendorByProductIdRequest request)
        {
            return this.iLead.GetVendorByProductId(request);
        }

        //
        [HttpPost]
        [Route("api/lead/getEmployeeByPersonInCharge")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByPersonInChargeResponse GetEmployeeByPersonInCharge([FromBody]GetEmployeeByPersonInChargeRequest request)
        {
            return this.iLead.GetEmployeeByPersonInCharge(request);
        }

        [HttpPost]
        [Route("api/lead/reportLead")]
        [Authorize(Policy = "Member")]
        public ReportLeadResponse ReportLead([FromBody]ReportLeadRequest request)
        {
            return this.iLead.ReportLead(request);
        }

        [HttpPost]
        [Route("api/lead/getMasterDataReportLead")]
        [Authorize(Policy = "Member")]
        public GetMasterDataReportLeadReponse GetMasterDataReportLead([FromBody]GetMasterDataReportLeadRequest request)
        {
            return this.iLead.GetMasterDataReportLead(request);
        }

        //
        [HttpPost]
        [Route("api/lead/changeLeadStatusSupport")]
        [Authorize(Policy = "Member")]
        public ChangeLeadStatusSupportResponse ChangeLeadStatusSupport([FromBody]ChangeLeadStatusSupportRequest request)
        {
            return this.iLead.ChangeLeadStatusSupport(request);
        }
    }
}
