using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataOrderDetailResult : BaseResult
    {
        public List<OrderStatus> ListOrderStatus { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<BankAccountEntityModel> ListCustomerBankAccount { get; set; }
        public List<CategoryEntityModel> ListPaymentMethod { get; set; }
        public List<CategoryEntityModel> ListCustomerGroup { get; set; }
        public List<string> ListCustomerCode { get; set; }
        public CustomerOrderEntityModel CustomerOrderObject { get; set; }
        public List<CustomerOrderDetailEntityModel> ListCustomerOrderDetail { get; set; }
        public List<OrderCostDetailEntityModel> ListCustomerOrderCostDetail { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<QuoteEntityModel> ListQuote { get; set; }
        public List<WareHouseEntityModel> ListWare { get; set; }
        public List<BillSaleEntityModel> ListBillSale { get; set; }
        public List<Databases.Entities.Product> ListProduct { get; set; }
        public List<Databases.Entities.Contract> ListContract { get; set; }
        public List<InventoryDeliveryVoucherEntityModel> ListInventoryDeliveryVoucher { get; set; }
        public bool IsManager { get; set; }

    }
}
