using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class SearchCustomerRequest : BaseRequest<SearchCustomerParameter>
    {
        public bool NoPic { get; set; }

        public bool IsBusinessCus { get; set; }

        public bool IsPersonalCus { get; set; }

        public Guid StatusCareId { get; set; }

        public List<Guid?> CustomerGroupIdList { get; set; }

        public List<Guid?> PersonInChargeIdList { get; set; }

        //public List<Guid?> SourceId { get; set; }

        public Guid AreaId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }


        public List<Guid?> CustomerServiceLevelIdList { get; set; }
        public bool IsHKDCus { get; set; }
        public string CustomerCode { get; set; }
        public string TaxCode { get; set; }
        public bool IsIdentificationCus { get; set; }
        public bool IsFreeCus { get; set; }

        public override SearchCustomerParameter ToParameter()
        {
            return new SearchCustomerParameter() {
                NoPic = NoPic,
                IsBusinessCus = IsBusinessCus,
                IsPersonalCus = IsPersonalCus,
                StatusCareId = StatusCareId,
                CustomerGroupIdList = CustomerGroupIdList,
                PersonInChargeIdList = PersonInChargeIdList,
                //SourceId = SourceId,
                AreaId = AreaId,
                FromDate = FromDate,
                ToDate = ToDate,
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone,
                Email = Email,
                Address = Address,
                UserId = UserId,
                IsHKDCus = IsHKDCus,
                CustomerServiceLevelIdList = CustomerServiceLevelIdList,
                CustomerCode = CustomerCode,
                TaxCode = TaxCode,
                IsIdentificationCus = IsIdentificationCus,
                IsFreeCus = IsFreeCus
            };
        }
    }
}
