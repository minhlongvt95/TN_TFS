using TN.TNM.DataAccess.Messages.Parameters.RequestPayment;
using TN.TNM.DataAccess.Messages.Results.RequestPayment;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IRequestPaymentDataAccess
    {
        CreateRequestPaymentResult CreateRequestPayment(CreateRequestPaymentParameter parameter);
        EditRequestPaymentResult EditRequestPayment(EditRequestPaymentParameter parameter);
        GetRequestPaymentByIdResult GetRequestPaymentById(GetRequestPaymentByIdParameter parameter);
        FindRequestPaymentResult FindRequestPayment(FindRequestPaymentParameter parameter);
    }
}
