using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CheckDuplicatePersonalCustomerByEmailOrPhoneParameter: BaseParameter
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool CheckByPhone { get; set; }
        public bool CheckByWorkPhone { get; set; }
        public bool CheckByOtherPhone { get; set; }
        public bool CheckByEmail { get; set; }
        public bool CheckByWorkEmail { get; set; }
        public bool CheckByOtherEmail { get; set; }
        public Guid? LeadId { get; set; }
    }
}
