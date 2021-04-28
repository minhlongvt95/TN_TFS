using TN.TNM.DataAccess.Messages.Parameters.Lead;
using TN.TNM.DataAccess.Messages.Results.Lead;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ILeadSearchDataAccess
    {
        /// <summary>
        /// GetLeadById
        /// </summary>
        /// <param name="paramater"></param>
        /// <returns></returns>
        GetLeadByIdResult GetLeadById(GetLeadByIdParamater paramater);
        /// <summary>
        /// GetAllLead
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetAllLeadResult GetAllLead(GetAllLeadParameter parameter);
        /// <summary>
        /// CreateLead
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        CreateLeadResult CreateLead(CreateLeadParameter parameter);
        /// <summary>
        /// DeleteLead
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        DeleteLeadResult DeleteLead(DeleteLeadParameter parameter);
        /// <summary>
        /// EditLeadById
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        EditLeadByIdResult EditLeadById(EditLeadByIdParameter parameter);
        /// <summary>
        /// GetNoteHistory
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetNoteHistoryResult GetNoteHistory(GetNoteHistoryParameter parameter);

        GetLeadByStatusResult GetLeadByStatus(GetLeadByStatusParameter parameter);
        GetLeadByNameResult GetLeadByName(GetLeadByNameParameter parameter);
        GetEmployeeWithNotificationPermisisonResult GetEmployeeWithNotificationPermisison(GetEmployeeWithNotificationPermisisonParameter parameter);
        ChangeLeadStatusToUnfollowResult ChangeLeadStatusToUnfollow(ChangeLeadStatusToUnfollowParameter parameter);
        ApproveRejectUnfollowLeadResult ApproveRejectUnfollowLead(ApproveRejectUnfollowLeadParameter parameter);
        GetEmployeeManagerResult GetEmployeeManager(GetEmployeeManagerParameter parameter);
        SendEmailLeadResult SendEmailLead(SendEmailLeadParameter parameter);
        SendSMSLeadResult SendSMSLead(SendSMSLeadParameter parameter);
        ImportLeadResult ImportLead(ImportLeadParameter parameter);
        UpdateLeadDuplicateResult UpdateLeadDuplicate(UpdateLeadDuplicateParameter parameter);
        DownloadTemplateLeadResult DownloadTemplateLead(DownloadTemplateLeadParameter parameter);
        ChangeLeadStatusToDeleteResult ChangeLeadStatusToDelete(ChangeLeadStatusToDeleteParameter parameter);
        CheckPhoneLeadResult CheckPhoneLead(CheckPhoneLeadParameter parameter);
        CheckEmailLeadResult CheckEmailLead(CheckEmailLeadParameter parameter);
        GetPersonInChargeResult GetPersonInCharge(GetPersonInChargeParameter parameter);
        EditPersonInChargeResult EditPersonInCharge(EditPersonInChargeParameter parameter);
        EditLeadStatusByIdResult EditLeadStatusById(EditLeadStatusByIdParameter parameter);
        GetDataCreateLeadResult GetDataCreateLead(GetDataCreateLeadParameter parameter);
        GetDataEditLeadResult GetDataEditLead(GetDataEditLeadParameter parameter);
        GetDataSearchLeadResult GetDataSearchLead(GetDataSearchLeadParameter parameter);
        ApproveOrRejectLeadUnfollowResult ApproveOrRejectLeadUnfollow(ApproveOrRejectLeadUnfollowParameter parameter);
        SetPersonalInChangeResult SetPersonalInChange(SetPersonalInChangeParameter parameter);
        UnfollowListLeadResult UnfollowListLead(UnfollowListLeadParamerter paramerter);
        ImportLeadDetailResult ImportLeadDetail(ImportLeadDetailParameter parameter);
        ImportListLeadResult ImportListLead(ImportListLeadParameter parameter);
        GetDataLeadProductDialogResult GetDataLeadProductDialog(GetDataLeadProductDialogParameter parameter);
        SendEmailSupportLeadResult SendEmailSupportLead(SendEmailSupportLeadParameter parameter);
        SendSMSSupportLeadResult SendSMSSupportLead(SendSMSSupportLeadParameter parameter);
        SendGiftSupportLeadResult SendGiftSupportLead(SendGiftSupportLeadParameter parameter);
        CreateLeadMeetingResult CreateLeadMeeting(CreateLeadMeetingParameter parameter);
        ChangeLeadStatusResult ChangeLeadStatus(ChangeLeadStatusParameter parameter);
        GetHistoryLeadCareResult GetHistoryLeadCare(GetHistoryLeadCareParameter parameter);
        GetDataPreviewLeadCareResult GetDataPreviewLeadCare(GetDataPreviewLeadCareParameter parameter);
        GetDataLeadCareFeedBackResult GetDataLeadCareFeedBack(GetDataLeadCareFeedBackParameter parameter);
        SaveLeadCareFeedBackResult SaveLeadCareFeedBack(SaveLeadCareFeedBackParameter parameter);
        GetHistoryLeadMeetingResult GetHistoryLeadMeeting(GetHistoryLeadMeetingParameter parameter);
        GetDataLeadMeetingByIdResult GetDataLeadMeetingById(GetDataLeadMeetingByIdParameter parameter);
        CloneLeadResult CloneLead(CloneLeadParameter parameter);
        GetEmployeeSellerResult GetEmployeeSeller(GetEmployeeSellerParameter parameter);
        GetVendorByProductIdResult GetVendorByProductId(GetVendorByProductIdParameter parameter);
        GetEmployeeByPersonInChargeResult GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeParameter parameter);
        GetListCustomerByTypeResult GetListCustomerByType(GetListCustomerByTypeParameter parameter);
        ReportLeadResult ReportLead(ReportLeadParameter parameter);
        GetMasterDataReportLeadResult GetMasterDataReportLead(GetMasterDataReportLeadParameter parameter);
        ChangeLeadStatusSupportResult ChangeLeadStatusSupport(ChangeLeadStatusSupportParameter parameter);
    }
}
