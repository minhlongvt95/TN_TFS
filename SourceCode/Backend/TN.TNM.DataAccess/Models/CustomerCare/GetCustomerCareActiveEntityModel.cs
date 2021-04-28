using System;

namespace TN.TNM.DataAccess.Models.CustomerCare
{
    public class GetCustomerCareActiveEntityModel
    {
        public Guid CustomerCareId { get; set; }
        public string CustomerCareTitle { get; set; }
        public int CustomerTotal { get; set; }
        public string Status { get; set; }
        public string CategoryCare{ get; set; }
        public string DateCreate{ get; set; }

    }
}
