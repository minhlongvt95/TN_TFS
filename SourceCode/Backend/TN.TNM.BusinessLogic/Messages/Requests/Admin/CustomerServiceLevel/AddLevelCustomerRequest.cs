using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.CustomerServiceLevel
{
    public class AddLevelCustomerRequest : BaseRequest<AddLevelCustomerParameter>
    {
        public List<CustomerServiceLevelModel> CustomerServiceLevel { get; set; }
        public override AddLevelCustomerParameter ToParameter()
        {
            List<DataAccess.Databases.Entities.CustomerServiceLevel> listCustomerServiceLevels = new List<DataAccess.Databases.Entities.CustomerServiceLevel>();

            CustomerServiceLevel.ForEach(x =>
            {
                var customerObject = new DataAccess.Databases.Entities.CustomerServiceLevel();
                customerObject = x.ToEntity();
                listCustomerServiceLevels.Add(customerObject);
            });
            return new AddLevelCustomerParameter()
            {
                CustomerServiceLevel = listCustomerServiceLevels
            };
        }
    }
}
