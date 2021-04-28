using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Lead
{
    public class ImportLeadModel
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Identity { get; set; }
        public string Address { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public string Email { get; set; }
        public Guid? InterestedGroupId { get; set; }
        public Guid? PotentialId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public string CompanyName { get; set; }
    }
}
