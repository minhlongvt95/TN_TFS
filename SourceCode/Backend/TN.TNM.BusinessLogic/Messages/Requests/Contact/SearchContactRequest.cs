using System;
using TN.TNM.DataAccess.Messages.Parameters.Contact;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contact
{
    public class SearchContactRequest : BaseRequest<SearchContactParameter>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid? PotentialId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? InterestedGroupId { get; set; }
        public Guid? PersonInChargeId { get; set; }

        public override SearchContactParameter ToParameter() => new SearchContactParameter()
        {
            FirstName = FirstName,
            LastName = LastName,
            Phone = Phone,
            Email = Email,
            PotentialId = PotentialId,
            StatusId = StatusId,
            InterestedGroupId = InterestedGroupId,
            PersonInChargeId = PersonInChargeId
        };
    }
}
