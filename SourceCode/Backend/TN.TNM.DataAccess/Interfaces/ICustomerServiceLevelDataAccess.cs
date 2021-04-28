using TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel;
using TN.TNM.DataAccess.Messages.Results.Admin.CustomerServiceLevel;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICustomerServiceLevelDataAccess
    {
        GetConfigCustomerServiceLevelResult GetConfigCustomerServiceLevel(GetConfigCustomerServiceLevelParameter parameter);
        AddLevelCustomerResult AddLevelCustomer(AddLevelCustomerParameter parameter);
        UpdateConfigCustomerResults UpdateConfigCustomer(UpdateConfigCustomerParameter parameter);
    }
}
