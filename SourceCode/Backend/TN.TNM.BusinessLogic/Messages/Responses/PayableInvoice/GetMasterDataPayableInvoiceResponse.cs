using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;

namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class GetMasterDataPayableInvoiceResponse:BaseResponse
    {
        public List<CategoryModel> ReasonOfPaymentList { get; set; }
        public List<CategoryModel> TypesOfPaymentList { get; set; }
        public List<CategoryModel> StatusOfPaymentList { get; set; }
        public List<CategoryModel> UnitMoneyList { get; set; }
        public List<OrganizationModel> OrganizationList { get; set; }
        public List<CustomerModel> CustomerList { get; set; }
    }
}
