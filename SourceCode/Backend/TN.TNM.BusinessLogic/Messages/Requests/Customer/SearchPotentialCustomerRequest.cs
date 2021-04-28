using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;


namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class SearchPotentialCustomerRequest : BaseRequest<SearchPotentialCustomerParameter>
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

        public override SearchPotentialCustomerParameter ToParameter()
        {
            return new SearchPotentialCustomerParameter()
            {
               FullName = FullName,
               Phone = Phone,
               Email = Email,
               Adress = Adress,
               InvestmentFundId = InvestmentFundId,
               PersonInChargeId = PersonInChargeId,
               ListCareStateId = ListCareStateId,
               IsConverted = IsConverted,
               ListCusTypeId = ListCusTypeId,
               ListCusGroupId = ListCusGroupId,
               ListAreaId = ListAreaId,
               StartDate = StartDate,
               EndDate = EndDate,
               UserId = UserId
            };
        }
    }
}
