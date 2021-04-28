using System;
using System.Linq;
using TN.TNM.Common;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.CustomerServiceLevel;
using TN.TNM.DataAccess.Messages.Results.Admin.CustomerServiceLevel;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class CustomerServiceLevelDAO : BaseDAO, ICustomerServiceLevelDataAccess
    {
        public CustomerServiceLevelDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public GetConfigCustomerServiceLevelResult GetConfigCustomerServiceLevel(GetConfigCustomerServiceLevelParameter parameter)
        {
            var listCustomerConfig = context.CustomerServiceLevel.OrderBy(x => x.MinimumSaleValue).ToList();
            return new GetConfigCustomerServiceLevelResult
            {
                Status = true,
                CustomerServiceLevel = listCustomerConfig
            };
        }

        public AddLevelCustomerResult AddLevelCustomer(AddLevelCustomerParameter parameter)
        {
            parameter.CustomerServiceLevel.ForEach(x =>
            {
                x.Active = true;
                x.CustomerServiceLevelId = Guid.NewGuid();
                x.CreatedById = parameter.UserId;
                x.CreatedDate = DateTime.Now;
            });
            context.CustomerServiceLevel.AddRange(parameter.CustomerServiceLevel);
            context.SaveChanges();

            return new AddLevelCustomerResult
            {
                Status = true,
                Message = CommonMessage.CustomerServiceLevel.ADD_SUCCESS,
            };
        }

        public UpdateConfigCustomerResults UpdateConfigCustomer(UpdateConfigCustomerParameter parameter)
        {
            var customerConfig =
                context.CustomerServiceLevel.FirstOrDefault(c => c.CustomerServiceLevelId == parameter.CustomerLevelId);
            if (customerConfig != null)
            {
                context.CustomerServiceLevel.Remove(customerConfig);
                context.SaveChanges();
            }
            return new UpdateConfigCustomerResults
            {
                Status = true
            };
        }
    }
}
