using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class InStockReportRequest : BaseRequest<InStockReportParameter>
    {
        public List<Guid> lstProduct { get; set; }
        public List<Guid> lstWarehouse { get; set; }
        public int? FromQuantity { get; set; }
        public int? ToQuantity { get; set; }
        public string SerialCode { get; set; }

        public override InStockReportParameter ToParameter()
        {
            return new InStockReportParameter
            {
                lstProduct = this.lstProduct,
                lstWarehouse = this.lstWarehouse,
                FromQuantity = this.FromQuantity,
                ToQuantity = this.ToQuantity,
                SerialCode = this.SerialCode
            };
        }
    }
}
