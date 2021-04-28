using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CheckDuplicateCustomerAllTypeParameter:BaseParameter
    {
        public short? CustomerType { get; set; }
        public Databases.Entities.Contact ContactModel { get; set; }
        public bool IsCheckOnSave { get; set; }
        public bool IsCheckedLead { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
