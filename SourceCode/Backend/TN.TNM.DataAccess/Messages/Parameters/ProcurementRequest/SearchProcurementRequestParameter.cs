using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest
{
    public class SearchProcurementRequestParameter : BaseParameter
    {
        public string ProcurementRequestCode { get; set; }
        public string ProcurementRequestContent { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid?> ListRequester { get; set; }
        public List<Guid?> ListStatus { get; set; }
        public List<Guid?> ListProduct { get; set; }
        public Guid? OrganizationId { get; set; }

        public List<Guid?> ListVendor { get; set; }
        public List<Guid?> ListApproval { get; set; }
        public List<Guid?> ListBudget { get; set; }
    }
}
