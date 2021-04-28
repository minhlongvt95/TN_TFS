using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;
using TN.TNM.DataAccess.Messages.Results.CustomerCare;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICustomerCareDataAccess
    {
        CreateCustomerCareResult CreateCustomerCare(CreateCustomerCareParameter parameter);
        UpdateCustomerCareResult UpdateCustomerCare(UpdateCustomerCareParameter parameter);
        GetCustomerCareByIdResult GetCustomerCareById(GetCustomerCareByIdParameter parameter);
        CreateCustomerCareFeedBackResult CreateCustomerCareFeedBack(CreateCustomerCareFeedBackParameter parameter);
        UpdateCustomerCareFeedBackResult UpdateCustomerCareFeedBack(UpdateCustomerCareFeedBackParameter parameter);
        FilterCustomerResult FilterCustomer(FilterCustomerParameter parameter);
        SearchCustomerCareResult SearchCustomerCare(SearchCustomerCareParameter parameter);
        UpdateStatusCustomerCareCustomerByIdResult UpdateStatusCustomerCareCustomerById(UpdateStatusCustomerCareCustomerByIdParameter parameter);
        GetTimeLineCustomerCareByCustomerIdResult GetTimeLineCustomerCareByCustomerId(GetTimeLineCustomerCareByCustomerIdParameter parameter);
        GetCustomerCareFeedBackByCusIdAndCusCareIdResult GetCustomerCareFeedBackByCusIdAndCusCareId(GetCustomerCareFeedBackByCusIdAndCusCareIdParameter parameter);
        SendQuickEmailResult SendQuickEmail(SendQuickEmailParameter parameter);
        SendQuickSMSResult SendQuickSMS(SendQuickSMSParameter parameter);
        SendQuickGiftResult SendQuickGift(SendQuickGiftParameter parameter);
        UpdateStatusCustomerCareResult UpdateStatusCustomerCare(UpdateStatusCustomerCareParameter parameter);
        GetCustomerBirthDayResult GetCustomerBirthDay(GetCustomerBirthDayParameter parameter);
        GetTotalInteractiveResult GetTotalInteractive(GetTotalInteractiveParameter parameter);
        GetCustomerNewCSResult GetCustomerNewCS(GetCustomerNewCSParameter parameter);
        GetCustomerCareActiveResult GetCustomerCareActive(GetCustomerCareActiveParameter parameter);
        GetCharCustomerCSResult GetCharCustomerCS(GetCharCustomerCSParameter parameter);
        GetMasterDataCustomerCareListResult GetMasterDataCustomerCareList(GetMasterDataCustomerCareListParameter parameter);
        UpdateStatusCusCareResult UpdateStatusCusCare(UpdateStatusCusCareParameter parameter);
        UpdateCustomerMeettingResult UpdateCustomerMeeting(UpdateCustomerMettingParameter paramter);
        RemoveCustomerMeetingResult RemoveCustomerMeeting(RemoveCustomerMettingParameter parameter);
    }
}
