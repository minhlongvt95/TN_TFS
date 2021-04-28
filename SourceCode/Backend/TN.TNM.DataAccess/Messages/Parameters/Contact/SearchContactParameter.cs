using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class SearchContactParameter : BaseParameter
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Guid? PotentialId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? InterestedGroupId { get; set; }
        public Guid? PersonInChargeId { get; set; }
    }
}
