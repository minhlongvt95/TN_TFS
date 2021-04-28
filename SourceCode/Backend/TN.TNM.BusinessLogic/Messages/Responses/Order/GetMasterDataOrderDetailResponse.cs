using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.BillSale;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Contract;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.OrderStatus;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetMasterDataOrderDetailResponse : BaseResponse
    {
        public List<OrderStatusModel> ListOrderStatus { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<CustomerModel> ListCustomer { get; set; }
        public List<BankAccountModel> ListCustomerBankAccount { get; set; }
        public List<CategoryModel> ListPaymentMethod { get; set; }
        public List<CategoryModel> ListCustomerGroup { get; set; }
        public List<string> ListCustomerCode { get; set; }
        public CustomerOrderModel CustomerOrderObject { get; set; }
        public List<CustomerOrderDetailModel> ListCustomerOrderDetail { get; set; }
        public List<NoteModel> ListNote { get; set; }
        public List<QuoteModel> ListQuote { get; set; }
        public List<WareHouseModel> ListWare { get; set; }
        public List<ProductModel> ListProduct { get; set; }
        public List<ContractModel> ListContract { get; set; }
        public List<BillSaleModel> ListBillSale { get; set; }
        public List<OrderCostDetailModel> ListCustomerOrderCostDetail { get; set; }
        public List<InventoryDeliveryVoucherModel> ListInventoryDeliveryVoucher { get; set; }
        public bool IsManager { get; set; }
    }
}
