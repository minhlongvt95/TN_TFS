using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.OrderStatus;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetMasterDataOrderSearchResponse : BaseResponse
    {
        public List<OrderStatusModel> ListOrderStatus { get; set; }
        public List<QuoteModel> ListQuote { get; set; }
        public List<ProductModel> ListProduct { get; set; }
        public List<ContractModel> ListContract { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public PDFOrderModel PDFOrder { get; set; }
    }
}
