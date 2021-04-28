using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CheckDuplicatePersonalCustomerByEmailOrPhoneRequest: BaseRequest<CheckDuplicatePersonalCustomerByEmailOrPhoneParameter>
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
        public override CheckDuplicatePersonalCustomerByEmailOrPhoneParameter ToParameter()
        {
            return new CheckDuplicatePersonalCustomerByEmailOrPhoneParameter()
            {
                UserId = UserId,
                Email = Email,
                Phone = Phone,
                CheckByPhone = CheckByPhone,
                CheckByWorkPhone = CheckByWorkPhone,
                CheckByOtherPhone = CheckByOtherPhone,
                CheckByEmail = CheckByEmail,
                CheckByWorkEmail = CheckByWorkEmail,
                CheckByOtherEmail = CheckByOtherEmail,
                LeadId = LeadId                
            };
        }
    }
}
