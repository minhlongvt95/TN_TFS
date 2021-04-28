using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class GetMasterDataBankSearchPayableInvoiceResponse : BaseResponse
    {
        public List<CategoryModel> ReasonOfPaymentList { get; set; }
        public List<CategoryModel> StatusOfPaymentList { get; set; }
        public List<EmployeeModel> EmployeeList { get; set; }
    }
}
