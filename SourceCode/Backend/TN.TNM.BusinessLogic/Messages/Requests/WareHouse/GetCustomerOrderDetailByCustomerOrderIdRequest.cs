using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class GetCustomerOrderDetailByCustomerOrderIdRequest : BaseRequest<GetCustomerOrderDetailByCustomerOrderIdParameter>
    {
        public int TypeWarehouseVocher { get; set; }

        public List<Guid> ListCustomerOrderId { get; set; }

        public override GetCustomerOrderDetailByCustomerOrderIdParameter ToParameter()
        {
            return new GetCustomerOrderDetailByCustomerOrderIdParameter
            {
                ListCustomerOrderId = this.ListCustomerOrderId,
                TypeWarehouseVocher=this.TypeWarehouseVocher
            };
        }
    }
}
