using TN.TNM.BusinessLogic.Messages.Requests.CustomerCare;
using TN.TNM.BusinessLogic.Messages.Responses.CustomerCare;

namespace TN.TNM.BusinessLogic.Interfaces.CustomerCare
{
    public interface ICustomerCare
    {
        CreateCustomerCareResponse CreateCustomerCare(CreateCustomerCareRequest request);
        UpdateCustomerCareResponse UpdateCustomerCare(UpdateCustomerCareRequest request);
        GetCustomerCareByIdResponse GetCustomerCareById(GetCustomerCareByIdRequest request);
        CreateCustomerCareFeedBackResponse CreateCustomerCareFeedBack(CreateCustomerCareFeedBackRequest request);
        UpdateCustomerCareFeedBackResponse UpdateCustomerCareFeedBack(UpdateCustomerCareFeedBackRequest request);
        FilterCustomerResponse FilterCustomer(FilterCustomerRequest request);
        SearchCustomerCareResponse SearchCustomerCare(SearchCustomerCareRequest request);
        UpdateStatusCustomerCareCustomerByIdResponse UpdateStatusCustomerCareCustomerById(UpdateStatusCustomerCareCustomerByIdRequest request);
        GetTimeLineCustomerCareByCustomerIdResponse GetTimeLineCustomerCareByCustomerId(GetTimeLineCustomerCareByCustomerIdRequest request);
        GetCustomerCareFeedBackByCusIdAndCusCareIdResponse GetCustomerCareFeedBackByCusIdAndCusCareId(GetCustomerCareFeedBackByCusIdAndCusCareIdRequest request);
        SendQuickEmailResponse SendQuickEmail(SendQuickEmailRequest request);
        SendQuickSMSResponse SendQuickSMS(SendQuickSMSRequest request);
        SendQuickGiftResponse SendQuickGift(SendQuickGiftRequest request);
        UpdateStatusCustomerCareResponse UpdateStatusCustomerCare(UpdateStatusCustomerCareRequest request);
        GetCustomerBirthDayResponse GetCustomerBirthDay(GetCustomerBirthDayRequest request);
        GetTotalInteractiveResponse GetTotalInteractive(GetTotalInteractiveRequest request);
        GetCustomerNewCSResponse GetCustomerNewCS(GetCustomerNewCSRequest request);
        GetCustomerCareActiveResponse GetCustomerCareActive(GetCustomerCareActiveRequest request);
        GetCharCustomerCSResponse GetCharCustomerCS(GetCharCustomerCSRequest request);
        GetMasterDataCustomerCareListResponse GetMasterDataCustomerCareList(GetMasterDataCustomerCareListRequest request);
        UpdateStatusCusCareResponse UpdateStatusCusCare(UpdateStatusCusCareRequest request);
        UpdateCustomerMeetingResponse UpdateCustomerMeeting(UpdateCustomerMeetingRequest request);
        RemoveCustomerMeetingResponse RemoveCustomerMeeting(RemoveCustomerMeetingRequest request);
    }
}
