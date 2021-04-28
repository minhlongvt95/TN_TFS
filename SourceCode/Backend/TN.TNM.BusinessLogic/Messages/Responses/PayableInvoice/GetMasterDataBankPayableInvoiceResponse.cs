using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class GetMasterDataBankPayableInvoiceResponse : BaseResponse
    {
        public List<CategoryModel> ReasonOfPaymentList { get; set; }
        public List<TN.TNM.BusinessLogic.Models.BankAccount.BankAccountModel> TypesOfPaymentList { get; set; }
        public List<CategoryModel> StatusOfPaymentList { get; set; }
        public List<CategoryModel> UnitMoneyList { get; set; }
        public List<OrganizationModel> OrganizationList { get; set; }
        public List<VendorModel> VendorList { get; set; }
    }
}
