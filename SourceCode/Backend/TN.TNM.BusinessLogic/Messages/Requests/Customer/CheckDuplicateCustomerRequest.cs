using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CheckDuplicateCustomerRequest : BaseRequest<CheckDuplicateCustomerParameter>
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Taxcode { get; set; }
        public int CustomerType { get; set; }
        public bool CheckByEmail { get; set; }
        public bool CheckByPhone { get; set; }
        public bool CheckCreateForm { get; set; }
        public Guid? LeadId { get; set; }

        public override CheckDuplicateCustomerParameter ToParameter()
        {
            return new CheckDuplicateCustomerParameter
            {
                CustomerType = this.CustomerType,
                Email = this.Email,
                Phone = this.Phone,
                Taxcode = this.Taxcode,
                UserId = this.UserId,
                CheckByEmail = this.CheckByEmail,
                CheckByPhone = this.CheckByPhone,
                CheckCreateForm = this.CheckCreateForm,
                LeadId = this.LeadId
            };
        }
    }
}
