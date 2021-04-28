using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class SearchInventoryDeliveryVoucherParameter:BaseParameter
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

    }
}
