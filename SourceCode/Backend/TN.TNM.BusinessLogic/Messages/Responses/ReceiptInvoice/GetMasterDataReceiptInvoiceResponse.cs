using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class GetMasterDataReceiptInvoiceResponse : BaseResponse
    {
        public List<CategoryModel> ReasonOfReceiptList { get; set; }
        public List<CategoryModel> TypesOfReceiptList { get; set; }
        public List<CategoryModel> StatusOfReceiptList { get; set; }
        public List<CategoryModel> UnitMoneyList { get; set; }
        public List<OrganizationModel> OrganizationList { get; set; }
        public List<CustomerModel> CustomerList { get; set; }
    }
}
