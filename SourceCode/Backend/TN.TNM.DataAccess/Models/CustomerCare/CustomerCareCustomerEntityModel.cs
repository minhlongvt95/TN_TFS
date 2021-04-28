using System;

namespace TN.TNM.DataAccess.Models.CustomerCare
{
    public class CustomerCareCustomerEntityModel
    {
        public Guid CustomerCareCustomerId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CustomerCareId { get; set; }
        public Guid? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateById { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateById { get; set; }

    }
}
