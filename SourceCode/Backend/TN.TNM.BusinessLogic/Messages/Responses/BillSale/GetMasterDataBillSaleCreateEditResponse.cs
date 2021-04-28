using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.BusinessLogic.Models.BillSale;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.BillSale
{
    public class GetMasterDataBillSaleCreateEditResponse : BaseResponse
    {
        public BillSaleModel BillSale { get; set; }
        public List<CategoryModel> ListBanking { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<CustomerModel> ListCustomer { get; set; }
        public List<NoteModel> ListNote { get; set; }
        public OrderBillModel Order { get; set; }
        public List<CustomerOrderModel> ListOrder { get; set; }
        public List<InventoryDeliveryVoucherModel> ListInventoryDeliveryVoucher { get; set; }
        public List<CategoryModel> ListMoney { get; set; }
    }
}
