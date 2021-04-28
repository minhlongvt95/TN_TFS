using TN.TNM.BusinessLogic.Messages.Requests.RequestPayment;
using TN.TNM.BusinessLogic.Messages.Responses.RequestPayment;

namespace TN.TNM.BusinessLogic.Interfaces.RequestPayment
{
    public interface IRequestPayment
    {
        CreateRequestPaymentResponse CreateRequestPayment(CreateRequestPaymentRequest request);
        EditRequestPaymentResponse EditRequestPayment(EditRequestPaymentRequest request);
        GetRequestPaymentByIdResponse GetRequestPaymentById(GetRequestPaymentByIdRequest request);
        FindRequestPaymentResponse FindRequestPayment(FindRequestPaymentRequest request);
    }
}
