using System;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CheckDuplicateCustomerLeadRequest : BaseRequest<CheckDuplicateCustomerLeadParameter>
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid LeadId { get; set; }
        public bool IsUpdateLead { get; set; }
        public override CheckDuplicateCustomerLeadParameter ToParameter()
        {
            return new CheckDuplicateCustomerLeadParameter
            {
                Email = this.Email,
                Phone = this.Phone,
                LeadId = this.LeadId,
                UserId = this.UserId,
                IsUpdateLead = this.IsUpdateLead,
            };
        }
    }
}
