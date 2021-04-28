using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.CashBook;

namespace TN.TNM.BusinessLogic.Messages.Requests.CashBook
{
    public class GetSurplusCashBookPerMonthRequest : BaseRequest<GetSurplusCashBookPerMonthParameter>
    {
        public DateTime? ToDate { get; set; }
        public DateTime? FromDate { get; set; }
        public List<Guid?> OrganizationList { get; set; }
        public override GetSurplusCashBookPerMonthParameter ToParameter()
        {
            return new GetSurplusCashBookPerMonthParameter
            {
                ToDate = ToDate,
                FromDate = FromDate,
                OrganizationList=OrganizationList,
                UserId = UserId
            };
        }
    }
}
