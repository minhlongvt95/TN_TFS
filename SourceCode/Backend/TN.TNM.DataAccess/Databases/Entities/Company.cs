using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Company
    {
        public Company()
        {
            Lead = new HashSet<Lead>();
        }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public ICollection<Lead> Lead { get; set; }
    }
}
