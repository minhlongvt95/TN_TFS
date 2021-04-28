using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Requests.WareHouse
{
    public class SearchInStockReportRequest : BaseRequest<SearchInStockReportParameter>
    {
        public DateTime FromDate { get; set; }
        public string ProductNameCode { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public Guid? WarehouseId { get; set; }

        public override SearchInStockReportParameter ToParameter()
        {
            return new SearchInStockReportParameter()
            {
                UserId = UserId,
                FromDate = FromDate,
                ProductNameCode = ProductNameCode,
                ProductCategoryId = ProductCategoryId,
                WarehouseId = WarehouseId
            };
        }
    }
}
