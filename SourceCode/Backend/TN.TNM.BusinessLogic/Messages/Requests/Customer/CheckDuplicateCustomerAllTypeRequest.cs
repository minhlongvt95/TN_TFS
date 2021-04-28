using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CheckDuplicateCustomerAllTypeRequest:BaseRequest<CheckDuplicateCustomerAllTypeParameter>
    {
        public short? CustomerType { get; set; }
        public DataAccess.Databases.Entities.Contact ContactModel { get; set; }
        public bool IsCheckOnSave { get; set; }
        public bool IsCheckedLead { get; set; }
        public Guid EmployeeId { get; set; }

        public override CheckDuplicateCustomerAllTypeParameter ToParameter()
        {
            return new CheckDuplicateCustomerAllTypeParameter
            {
                ContactModel = ContactModel,
                CustomerType = CustomerType,
                IsCheckOnSave = IsCheckOnSave,
                IsCheckedLead = IsCheckedLead,
                EmployeeId = EmployeeId
            };
        }
    }
}
