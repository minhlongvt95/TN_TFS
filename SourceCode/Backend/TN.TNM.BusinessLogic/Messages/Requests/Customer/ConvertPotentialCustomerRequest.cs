using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class ConvertPotentialCustomerRequest: BaseRequest<ConvertPotentialCustomerParameter>
    {
        public Guid? CustomerId { get; set; }
        public bool? IsCreateCustomer { get; set; }
        public bool? IsCreateLead { get; set; }
        public Guid? PersonalInChargeId { get; set; } //nếu tạo khách hàng 
        //public DataAccess.Models.Lead.LeadEntityModel LeadModel { get; set; }
        //public DataAccess.Models.ContactEntityModel ContactModel { get; set; }
        //public List<Guid?> ListInterestedGroupId { get; set; }
        public string LeadName { get; set; }

        public override ConvertPotentialCustomerParameter ToParameter()
        {
            return new ConvertPotentialCustomerParameter()
            {
                CustomerId = CustomerId,
                IsCreateCustomer = IsCreateCustomer,
                IsCreateLead = IsCreateLead,
                PersonalInChargeId = PersonalInChargeId,
                //LeadModel = LeadModel,
                //ContactModel = ContactModel,
                //ListInterestedGroupId = ListInterestedGroupId ?? new List<Guid?>(),
                LeadName = LeadName,
                UserId = UserId
            };
        }
    }
}
