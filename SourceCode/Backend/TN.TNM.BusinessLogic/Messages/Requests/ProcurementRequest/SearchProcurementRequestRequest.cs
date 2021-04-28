using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class SearchProcurementRequestRequest : BaseRequest<SearchProcurementRequestParameter>
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
        public override SearchProcurementRequestParameter ToParameter()
        {
            return new SearchProcurementRequestParameter()
            {
                FromDate = FromDate,
                ProcurementRequestContent = ProcurementRequestContent,
                ListRequester = ListRequester,
                ListVendor = ListVendor,
                ListApproval = ListApproval,
                ListBudget = ListBudget,
                ListStatus = ListStatus,
                ListProduct = ListProduct,
                OrganizationId = OrganizationId,
                ProcurementRequestCode = ProcurementRequestCode,
                ToDate = ToDate,
                UserId = UserId
            };
        }
    }
}
