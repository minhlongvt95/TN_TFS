using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class FilterProductRequest : BaseRequest<FilterProductParameter>
    {
        public List<Guid> ListProductCategory { get; set; }
        public List<Guid> ListProductId { get; set; }

        public override FilterProductParameter ToParameter()
        {
            return new FilterProductParameter
            {
                ListProductCategory = this.ListProductCategory,
                ListProductId = this.ListProductId
            };
        }
    }
}
