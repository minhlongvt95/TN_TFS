using TN.TNM.BusinessLogic.Messages.Requests.Lead;
using TN.TNM.BusinessLogic.Messages.Responses.Leads;

namespace TN.TNM.BusinessLogic.Interfaces.Lead
{
    /// <summary>
    /// TODO: Thuc hien cac chung nang tim kiem lead
    /// 
    /// 1. GetAllLead: tim kiem lead theo cac tieu chi
    /// 2. GetLeadBy...: tim kiem lead theo...
    /// 3. ...
    /// 
    /// Author: thanhhh@tringhiatech.vn
    /// Date: 15/06/2018
    /// </summary>
    public interface ILeadSearch
    {
        /// <summary>
        /// TODO: tim kiem lead theo cac tieu chi...
        /// ....
        ///  
        /// </summary>
        /// <param name="request">Chua cac parameter truyen vao</param>        
        /// <returns>Danh sach lead thoa man dieu kien</returns>
        GetAllLeadResponse GetAllLead(GetAllLeadRequest request);
        /// <summary>
        /// CreateLead
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        CreateLeadResponse CreateLead(CreateLeadRequest request);
        /// <summary>
        /// GetLeadById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetLeadByIdResponse GetLeadById(GetLeadByIdRequest request);
        /// <summary>
        /// EditLeadById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        EditLeadByIdResponse EditLeadById(EditLeadByIdRequest request);
        /// <summary>
        /// GetNoteHistory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetNoteHistoryResponse GetNoteHistory(GetNoteHistoryRequest request);
        /// <summary>
        /// GetNoteHistory
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetLeadByStatusResponse GetLeadByStatus(GetLeadByStatusRequest request);
        GetLeadByNameResponse GetLeadByName(GetLeadByNameRequest request);
        GetEmployeeWithNotificationPermisisonResponse GetEmployeeWithNotificationPermisison(GetEmployeeWithNotificationPermisisonRequest request);
        ChangeLeadStatusToUnfollowResponse ChangeLeadStatusToUnfollow(ChangeLeadStatusToUnfollowRequest request);
        ApproveRejectUnfollowLeadResponse ApproveRejectUnfollowLead(ApproveRejectUnfollowLeadRequest request);
        GetEmployeeManagerResponse GetEmployeeManager(GetEmployeeManagerRequest request);
        SendEmailLeadResponse SendEmailLead(SendEmailLeadRequest request);
        SendSMSLeadResponse SendSMSLead(SendSMSLeadRequest request);
        ImportLeadResponse ImportLead(ImportLeadRequest request);
        UpdateLeadDuplicateResponse UpdateLeadDuplicate(UpdateLeadDuplicateRequest request);
        DownloadTemplateLeadResponse DownloadTemplateLead(DownloadTemplateLeadRequest request);
        ChangeLeadStatusToDeleteResponse ChangeLeadStatusToDelete(ChangeLeadStatusToDeleteRequest request);
        CheckPhoneLeadResponse CheckPhoneLead(CheckPhoneLeadRequest request);
        CheckEmailLeadResponse CheckEmailLead(CheckEmailLeadRequest request);
        GetPersonInChargeResponse GetPersonInCharge(GetPersonInChargeRequest request);
        EditPersonInChargeResponse EditPersonInCharge(EditPersonInChargeRequest request);
        EditLeadStatusByIdResponse EditLeadStatusById(EditLeadStatusByIdRequest request);
        GetDataCreateLeadResponse GetDataCreateLead(GetDataCreateLeadRequest request);
        GetDataEditLeadResponse GetDataEditLead(GetDataEditLeadRequest request);
        GetDataSearchLeadResponse GetDataSearchLead(GetDataSearchLeadRequest request);
        ApproveOrRejectLeadUnfollowResponse ApproveOrRejectLeadUnfollow(ApproveOrRejectLeadUnfollowRequest request);
        SetPersonalInChangeResponse SetPersonalInChange(SetPersonalInChangeRequest request);
        UnfollowListLeadResponse UnfollowListLead(UnfollowListLeadRequest request);
        ImportLeadDetailResponse ImportLeadDetail(ImportLeadDetailRequest request);
        ImportListLeadResponse ImportListLead(ImportListLeadRequest request);
        GetDataLeadProductDialogResponse GetDataLeadProductDialog(GetDataLeadProductDialogRequest request);
        SendEmailSupportLeadResponse SendEmailSupportLead(SendEmailSupportLeadRequest request);
        SendSMSSupportLeadResponse SendSMSSupportLead(SendSMSSupportLeadRequest request);
        SendGiftSupportLeadResponse SendGiftSupportLead(SendGiftSupportLeadRequest request);
        CreateLeadMeetingResponse CreateLeadMeeting(CreateLeadMeetingRequest request);
        ChangeLeadStatusResponse ChangeLeadStatus(ChangeLeadStatusRequest request);
        GetHistoryLeadCareResponse GetHistoryLeadCare(GetHistoryLeadCareRequest request);
        GetDataPreviewLeadCareResponse GetDataPreviewLeadCare(GetDataPreviewLeadCareRequest request);
        GetDataLeadCareFeedBackResponse GetDataLeadCareFeedBack(GetDataLeadCareFeedBackRequest request);
        SaveLeadCareFeedBackResponse SaveLeadCareFeedBack(SaveLeadCareFeedBackRequest request);
        GetHistoryLeadMeetingResponse GetHistoryLeadMeeting(GetHistoryLeadMeetingRequest request);
        GetDataLeadMeetingByIdResponse getDataLeadMeetingById(GetDataLeadMeetingByIdRequest request);
        CloneLeadResponse CloneLead(CloneLeadRequest request);
        GetEmployeeSellerResponse GetEmployeeSeller(GetEmployeeSellerRequest request);
        GetVendorByProductIdResponse GetVendorByProductId(GetVendorByProductIdRequest request);
        GetEmployeeByPersonInChargeResponse GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeRequest request);
        GetListCustomerByTypeResponse GetListCustomerByType(GetListCustomerByTypeRequest request);
        ReportLeadResponse ReportLead(ReportLeadRequest request);
        GetMasterDataReportLeadReponse GetMasterDataReportLead(GetMasterDataReportLeadRequest request);
        ChangeLeadStatusSupportResponse ChangeLeadStatusSupport(ChangeLeadStatusSupportRequest request);
    }
}
