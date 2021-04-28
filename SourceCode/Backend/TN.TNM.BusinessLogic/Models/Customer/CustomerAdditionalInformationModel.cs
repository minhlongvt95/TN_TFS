using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Models.Customer
{
    public class CustomerAdditionalInformationModel
    {
        public Guid CustomerAdditionalInformationId { get; set; }
        public Guid CustomerId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
