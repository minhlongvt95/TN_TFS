using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class SearchCustomerCareParameter:BaseParameter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string CustomerCareTitle { get; set; }
        public string CustomerCareCode { get; set; }
        public List<Guid> ListTypeCusCareId { get; set; }
        public List<Guid> PicName { get; set; }
        public List<Guid> Status { get; set; }
        public string CustomerCareContent { get; set; }
        public List<int> ProgramType { get; set; }
    }
}
