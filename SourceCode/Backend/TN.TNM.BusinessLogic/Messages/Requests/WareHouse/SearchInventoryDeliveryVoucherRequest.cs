using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class SearchInventoryDeliveryVoucherRequest : BaseRequest<SearchInventoryDeliveryVoucherParameter>
    {
        public string VoucherCode { get; set; }
        public List<Guid> listStatusSelectedId { get; set; }
        public List<Guid> listWarehouseSelectedId { get; set; }
        public List<Guid> listCreateVoucherSelectedId { get; set; }
        public List<Guid> listStorekeeperSelectedId { get; set; }
        public List<Guid> listCustomerSelectedId { get; set; }
        public List<Guid> listProductSelectedId { get; set; }
        public List<DateTime> listCreateDate { get; set; }
        public List<DateTime> listInventoryReceivingDate { get; set; }
        public string serial { get; set; }

        public override SearchInventoryDeliveryVoucherParameter ToParameter()
        {
            return new SearchInventoryDeliveryVoucherParameter
            {
                VoucherCode = this.VoucherCode,
                listStatusSelectedId = this.listStatusSelectedId,
                listWarehouseSelectedId = this.listWarehouseSelectedId,
                listCreateVoucherSelectedId = this.listCreateVoucherSelectedId,
                listStorekeeperSelectedId = this.listStorekeeperSelectedId,
                listCustomerSelectedId = this.listCustomerSelectedId,
                listProductSelectedId = this.listProductSelectedId,
                listCreateDate = this.listCreateDate,
                listInventoryReceivingDate = this.listInventoryReceivingDate,
                serial = this.serial,
                UserId=this.UserId
            };
        }
    }
}
