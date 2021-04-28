using TN.TNM.BusinessLogic.Messages.Requests.Admin.CustomerServiceLevel;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.CustomerServiceLevel;

namespace TN.TNM.BusinessLogic.Interfaces.Admin.CustomerServiceLevel
{
    public interface ICustomerServiceLevel
    {
        AddLevelCustomerResponse AddLevelCustomer(AddLevelCustomerRequest request);
        GetConfigCustomerServiceLevelResponse GetConfigCustomerServiceLevel(GetConfigCustomerServiceLevelRequest request);
        UpdateConfigCustomerResponse UpdateConfigCustomer(UpdateConfigCustomerRequest request);
    }
}
