using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class SearchPotentialCustomerParameter: BaseParameter
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
        public List<Guid?> InvestmentFundId { get; set; }
        public List<Guid?> PersonInChargeId { get; set; }
        public List<Guid?> ListCareStateId { get; set; }
        public List<Guid?> ListCusTypeId { get; set; }
        public List<Guid?> ListCusGroupId { get; set; }
        public List<Guid?> ListAreaId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool? IsConverted { get; set; }
    }
}
