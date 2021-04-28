using TN.TNM.BusinessLogic.Messages.Requests.Customer;
using TN.TNM.BusinessLogic.Messages.Responses.Customer;

namespace TN.TNM.BusinessLogic.Interfaces.Customer
{
    public interface ICustomer
    {
        CreateCustomerResponse CreateCustomer(CreateCustomerRequest request);
        SearchCustomerResponse SearchCustomer(SearchCustomerRequest request);
        GetAllCustomerServiceLevelResponse GetAllCustomerServiceLevel(GetAllCustomerServiceLevelRequest request);
        GetCustomerByIdResponse GetCustomerById(GetCustomerByIdRequest request);
        EditCustomerByIdResponse EditCustomerById(EditCustomerByIdRequest request);
        GetAllCustomerResponse GetAllCustomer(GetAllCustomerRequest request);
        QuickCreateCustomerResponse QuickCreateCustomer(QuickCreateCustomerRequest request);
        GetAllCustomerCodeResponse GetAllCustomerCode(GetAllCustomerCodeRequest request);
        ImportCustomerResponse ImportCustomer(ImportCustomerRequest request);
        DownloadTemplateCustomerResponse DownloadTemplateCustomer(DownloadTemplateCustomerRequest request);
        UpdateCustomerDuplicateResponse UpdateCustomerDuplicate(UpdateCustomerDuplicateRequest request);
        GetStatisticCustomerForDashboardResponse GetStatisticCustomerForDashboard(GetStatisticCustomerForDashboardRequest request);
        GetListCustomeSaleToprForDashboardResponse GetListCustomeSaleToprForDashboard(GetListCustomeSaleToprForDashboardRequest request);
        CheckDuplicateCustomerResponse CheckDuplicateCustomer(CheckDuplicateCustomerRequest request);
        CheckDuplicateCustomerResponse CheckDuplicateCustomerPhoneOrEmail(CheckDuplicateCustomerLeadRequest request);
        CheckDuplicatePersonalCustomerByEmailOrPhoneResponse CheckDuplicatePersonalCustomerByEmailOrPhone(CheckDuplicatePersonalCustomerByEmailOrPhoneRequest request);
        CheckDuplicatePersonalCustomerResponse CheckDuplicatePersonalCustomer(CheckDuplicatePersonalCustomerRequest request);
        GetAllCustomerAdditionalByCustomerIdResponse GetAllCustomerAdditionalByCustomerId(GetAllCustomerAdditionalByCustomerIdRequest request);
        CreateCustomerAdditionalResponse CreateCustomerAdditional(CreateCustomerAdditionalRequest request);
        DeleteCustomerAdditionalResponse DeleteCustomerAdditional(DeleteCustomerAdditionalRequest request);
        EditCustomerAdditionalResponse EditCustomerAdditional(EditCustomerAdditionalRequest request);
        CreateListQuestionResponse CreateListQuestion(CreateListQuestionRequest request);
        GetListQuestionAnswerBySearchResponse GetListQuestionAnswerBySearch(GetListQuestionAnswerBySearchRequest request);
        GetAllHistoryProductByCustomerIdResponse GetAllHistoryProductByCustomerId(GetAllHistoryProductByCustomerIdRequest request);
        GetCustomerFromOrderCreateResponse GetCustomerFromOrderCreate(GetCustomerFromOrderCreateRequest request);
        CreateCustomerFromProtalResponse CreateCustomerFromProtal(CreateCustomerFromProtalRequest request);
        ChangeCustomerStatusToDeleteResponse ChangeCustomerStatusToDelete(ChangeCustomerStatusToDeleteRequest request);
        GetDashBoardCustomerResponse GetDashBoardCustomer(GetDashBoardCustomerRequest request);
        GetListCustomerResponse GetListCustomer(GetListCustomerRequest request);
        CreateCustomerMasterDataResponse CreateCustomerMasterData(CreateCustomerMasterDataRequest request);
        CheckDuplicateCustomerAllTypeResponse CheckDuplicateCustomerAllType(CheckDuplicateCustomerAllTypeRequest request);
        UpdateCustomerByIdResponse UpdateCustomerById(UpdateCustomerByIdRequest request);
        GetCustomerImportDetailResponse GetCustomerImportDetail(GetCustomerImportDetailRequest request);
        ImportListCustomerResponse ImportListCustomer(ImportListCustomerRequest request);
        DeleteListCustomerAdditionalResponse DeleteListCustomerAdditional(DeleteListCustomerAdditionalRequest request);
        GetHistoryCustomerCareResponse GetHistoryCustomerCare(GetHistoryCustomerCareRequest request);
        GetDataPreviewCustomerCareResponse GetDataPreviewCustomerCare(GetDataPreviewCustomerCareRequest request);
        GetDataCustomerCareFeedBackResponse GetDataCustomerCareFeedBack(GetDataCustomerCareFeedBackRequest request);
        SaveCustomerCareFeedBackResponse SaveCustomerCareFeedBack(SaveCustomerCareFeedBackRequest request);
        GetDataCustomerMeetingByIdResponse GetDataCustomerMeetingById(GetDataCustomerMeetingByIdRequest request);
        CreateCustomerMeetingResponse CreateCustomerMeeting(CreateCustomerMeetingRequest request);
        GetHistoryCustomerMeetingResponse GetHistoryCustomerMeeting(GetHistoryCustomerMeetingRequest request);
        SendApprovalResponse SendApproval(SendApprovalRequest request);
        SearchCustomerResponse GetListCustomerRequestApproval(SearchCustomerRequest request);
        SendApprovalResponse ApprovalOrRejectCustomer(ApprovalOrRejectCustomerRequest request);
        GetDataCreatePotentialCustomerResponse GetDataCreatePotentialCustomer(GetDataCreatePotentialCustomerRequest request);
        GetDataDetailPotentialCustomerResponse GetDataDetailPotentialCustomer(GetDataDetailPotentialCustomerRequest request);
        UpdatePotentialCustomerResponse UpdatePotentialCustomer(UpdatePotentialCustomerRequest request);
        GetDataSearchPotentialCustomerResponse GetDataSearchPotentialCustomer(GetDataSearchPotentialCustomerRequest request);
        SearchPotentialCustomerResponse SearchPotentialCustomer(SearchPotentialCustomerRequest request);
        GetDataDashboardPotentialCustomerResponse GetDataDashboardPotentialCustomer(GetDataDashboardPotentialCustomerRequest request);
        ConvertPotentialCustomerResponse ConvertPotentialCustomer(ConvertPotentialCustomerRequest request);
        DownloadTemplatePotentialCustomerResponse DownloadTemplatePotentialCustomer(DownloadTemplatePotentialCustomerRequest request);
        GetDataImportPotentialCustomerResponse GetDataImportPotentialCustomer(GetDataImportPotentialCustomerRequest request);
        DownloadTemplateImportCustomerResponse DownloadTemplateImportCustomer(DownloadTemplateImportCustomerRequest request);
        SearchContactCustomerResponse SearchContactCustomer(SearchContactCustomerRequest request);
        CheckDuplicateInforCustomerResponse CheckDuplicateInforCustomer(CheckDuplicateInforCustomerRequest request);
        ChangeStatusSupportResponse ChangeStatusSupport(ChangeStatusSupportRequest request);
    }
}
