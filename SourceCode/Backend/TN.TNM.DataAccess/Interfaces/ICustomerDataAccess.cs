using TN.TNM.DataAccess.Messages.Parameters.Customer;
using TN.TNM.DataAccess.Messages.Results.Customer;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICustomerDataAccess
    {
        CreateCustomerResult CreateCustomer(CreateCustomerParameter parameter);
        SearchCustomerResult SearchCustomer(SearchCustomerParameter parameter);
        GetAllCustomerServiceLevelResult GetAllCustomerServiceLevel(GetAllCustomerServiceLevelParameter parameter);
        GetCustomerByIdResult GetCustomerById(GetCustomerByIdParameter parameter);
        EditCustomerByIdResult EditCustomerById(EditCustomerByIdParameter parameter);
        GetAllCustomerResult GetAllCustomer(GetAllCustomerParameter parameter);
        QuickCreateCustomerResult QuickCreateCustomer(QuickCreateCustomerParameter parameter);
        GetAllCustomerCodeResult GetAllCustomerCode(GetAllCustomerCodeParameter parameter);
        ImportCustomerResult ImportCustomer(ImportCustomerParameter parameter);
        DownloadTemplateCustomerResult DownloadTemplateCustomer(DownloadTemplateCustomerParameter parameter);
        UpdateCustomerDuplicateResult UpdateCustomerDuplicate(UpdateCustomerDuplicateParameter parameter);
        GetStatisticCustomerForDashboardResult GetStatisticCustomerForDashboard(GetStatisticCustomerForDashboardParameter parameter);
        GetListCustomeSaleToprForDashboardResult GetListCustomeSaleToprForDashboard(GetListCustomeSaleToprForDashboardParameter parameter);
        CheckDuplicateCustomerResult CheckDuplicateCustomer(CheckDuplicateCustomerParameter parameter);
        CheckDuplicateCustomerResult CheckDuplicateCustomerPhoneOrEmail(CheckDuplicateCustomerLeadParameter parameter);
        CheckDuplicatePersonalCustomerResult CheckDuplicatePersonalCustomer(CheckDuplicatePersonalCustomerParameter parameter);
        CheckDuplicatePersonalCustomerByEmailOrPhoneResult CheckDuplicatePersonalCustomerByEmailOrPhone(CheckDuplicatePersonalCustomerByEmailOrPhoneParameter parameter);
        GetAllCustomerAdditionalByCustomerIdResult GetAllCustomerAdditionalByCustomerId(GetAllCustomerAdditionalByCustomerIdParameter parameter);
        CreateCustomerAdditionalResult CreateCustomerAdditional(CreateCustomerAdditionalParameter parameter);
        DeleteCustomerAdditionalResult DeleteCustomerAdditional(DeleteCustomerAdditionalParameter parameter);
        EditCustomerAdditionalResult EditCustomerAdditional(EditCustomerAdditionalParameter parameter);
        CreateListQuestionResult CreateListQuestion(CreateListQuestionParameter parameter);
        GetListQuestionAnswerBySearchResult GetListQuestionAnswerBySearch(GetListQuestionAnswerBySearchParameter parameter);
        GetAllHistoryProductByCustomerIdResult GetAllHistoryProductByCustomerId(GetAllHistoryProductByCustomerIdParameter parameter);
        GetCustomerFromOrderCreateResult GetCustomerFromOrderCreate(GetCustomerFromOrderCreateParameter parameter);
        CreateCustomerFromProtalResult CreateCustomerFromProtal(CreateCustomerFromProtalParameter parameter);
        ChangeCustomerStatusToDeleteResult ChangeCustomerStatusToDelete(ChangeCustomerStatusToDeleteParameter parameter);
        GetDashBoardCustomerResult GetDashBoardCustomer(GetDashBoardCustomerParameter parameter);
        GetListCustomerResult GetListCustomer(GetListCustomerParameter parameter);
        CreateCustomerMasterDataResult CreateCustomerMasterData(CreateCustomerMasterDataParameter parameter);
        CheckDuplicateCustomerAllTypeResult CheckDuplicateCustomerAllType(CheckDuplicateCustomerAllTypeParameter request);
        UpdateCustomerByIdResult UpdateCustomerById(UpdateCustomerByIdParameter parameter);
        GetCustomerImportDetailResult GetCustomerImportDetail(GetCustomerImportDetailParameter parameter);
        ImportListCustomerResult ImportListCustomer(ImportListCustomerParameter parameter);

        DeleteListCustomerAdditionalResult
            DeleteListCustomerAdditional(DeleteListCustomerAdditionalParameter parameter);

        GetHistoryCustomerCareResult GetHistoryCustomerCare(GetHistoryCustomerCareParameter parameter);
        GetDataPreviewCustomerCareResult GetDataPreviewCustomerCare(GetDataPreviewCustomerCareParameter parameter);
        GetDataCustomerCareFeedBackResult GetDataCustomerCareFeedBack(GetDataCustomerCareFeedBackParameter parameter);
        SaveCustomerCareFeedBackResult SaveCustomerCareFeedBack(SaveCustomerCareFeedBackParameter parameter);
        GetDataCustomerMeetingByIdResult GetDataCustomerMeetingById(GetDataCustomerMeetingByIdParameter parameter);
        CreateCustomerMeetingResult CreateCustomerMeeting(CreateCustomerMeetingParameter parameter);
        GetHistoryCustomerMeetingResult GetHistoryCustomerMeeting(GetHistoryCustomerMeetingParameter parameter);
        SendApprovalResult SendApproval(SendApprovalParameter parameter);
        SearchCustomerResult GetListCustomerRequestApproval(SearchCustomerParameter parameter);
        SendApprovalResult ApprovalOrRejectCustomer(ApprovalOrRejectCustomerParameter parameter);
        GetDataCreatePotentialCustomerResponse GetDataCreatePotentialCustomer(GetDataCreatePotentialCustomerParameter parameter);
        GetDataDetailPotentialCustomerResult GetDataDetailPotentialCustomer(GetDataDetailPotentialCustomerParameter parameter);
        UpdatePotentialCustomerResult UpdatePotentialCustomer(UpdatePotentialCustomerParameter parameter);
        GetDataSearchPotentialCustomerResult GetDataSearchPotentialCustomer(GetDataSearchPotentialCustomerParameter parameter);
        SearchPotentialCustomerResult SearchPotentialCustomer(SearchPotentialCustomerParameter parameter);
        GetDataDashboardPotentialCustomerResult GetDataDashboardPotentialCustomer(GetDataDashboardPotentialCustomerParameter parameter);
        ConvertPotentialCustomerResult ConvertPotentialCustomer(ConvertPotentialCustomerParameter parameter);
        DownloadTemplatePotentialCustomerResult DownloadTemplatePotentialCustomer(DownloadTemplatePotentialCustomerParameter parameter);
        GetDataImportPotentialCustomerResult GetDataImportPotentialCustomer(GetDataImportPotentialCustomerParameter parameter);
        DownloadTemplateImportCustomerResult DownloadTemplateImportCustomer(DownloadTemplateImportCustomerParameter parameter);
        SearchContactCustomerResult SearchContactCustomer(SearchContactCustomerParameter parameter);
        CheckDuplicateInforCustomerResult CheckDuplicateInforCustomer(CheckDuplicateInforCustomerParameter parameter);
        ChangeStatusSupportResult ChangeStatusSupport(ChangeStatusSupportParameter parameter);
    }
}
