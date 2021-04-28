using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.BillSale;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.BillSale
{
    public class GetMasterDataBillSaleCreateEditResult:BaseResult
    {
        public BillSaleEntityModel BillSale { get; set; }
        public List<CategoryEntityModel> ListBanking { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public OrderBillEntityModel Order { get; set; }
        public List<CustomerOrderEntityModel> ListOrder { get; set; }
        public List<InventoryDeliveryVoucherEntityModel> ListInventoryDeliveryVoucher { get; set; }
        public List<CategoryEntityModel> ListMoney { get; set; }
    }
}
