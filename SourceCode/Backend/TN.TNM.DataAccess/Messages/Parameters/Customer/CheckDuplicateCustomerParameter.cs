using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CheckDuplicateCustomerParameter:BaseParameter
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Taxcode { get; set; }
        public  int CustomerType { get; set; }
        public bool CheckByEmail { get; set; }
        public bool CheckByPhone { get; set; }
        public bool CheckCreateForm { get; set; }
        public Guid? LeadId { get; set; }
    }
}
