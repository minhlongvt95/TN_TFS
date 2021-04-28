using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class SearchSaleBiddingRequest : BaseRequest<SearchSaleBiddingParameter>
    {
        public string SaleBiddingName { get; set; }
        public string CusName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<Guid> ListTypeCustomer { get; set; }// Loại khách hàng
        public List<Guid> ListContractId { get; set; } // Nhóm tiềm năng 
        public List<Guid> ListPersonalInChangeId { get; set; } // Người phụ trách
        public List<Guid> ListStatusId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public override SearchSaleBiddingParameter ToParameter()
        {
            return new SearchSaleBiddingParameter
            {
                UserId = UserId,
                SaleBiddingName = SaleBiddingName,
                CusName = CusName,
                Phone = Phone,
                Email = Email,
                ListContractId = ListContractId,
                ListPersonalInChangeId = ListPersonalInChangeId,
                ListStatusId = ListStatusId,
                ListTypeCustomer = ListTypeCustomer,
                FromDate = FromDate,
                ToDate = ToDate
            };
        }
    }
}
