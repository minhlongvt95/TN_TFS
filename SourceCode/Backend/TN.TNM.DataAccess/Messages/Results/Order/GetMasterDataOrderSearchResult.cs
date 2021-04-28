using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataOrderSearchResult : BaseResult
    {
        public List<OrderStatus> ListOrderStatus { get; set; }
        public List<QuoteEntityModel> ListQuote { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<Databases.Entities.Contract> ListContract { get; set; }
        public List<Databases.Entities.Employee> ListEmployee { get; set; }
        public PDFOrderModel PDFOrder { get; set; }
    }
}
