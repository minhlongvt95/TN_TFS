using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.SaleBidding
{
    public class SearchSaleBiddingParameter : BaseParameter
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
    }
}
