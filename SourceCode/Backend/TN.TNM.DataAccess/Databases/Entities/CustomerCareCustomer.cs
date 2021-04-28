using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerCareCustomer
    {
        public Guid CustomerCareCustomerId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? TenantId { get; set; }

        public Customer Customer { get; set; }
        public CustomerCare CustomerCare { get; set; }
    }
}
