using System;

namespace TN.TNM.DataAccess.Models.CustomerServiceLevel
{
    public class CustomerServiceLevelEntityModel
    {
        public Guid? CustomerServiceLevelId { get; set; }
        public string CustomerServiceLevelName { get; set; }
        public string CustomerServiceLevelCode { get; set; }
        public decimal? MinimumSaleValue { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
    }
}
