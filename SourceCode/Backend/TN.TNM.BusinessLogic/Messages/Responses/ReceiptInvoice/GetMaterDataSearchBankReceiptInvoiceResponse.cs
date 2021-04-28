using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class GetMaterDataSearchBankReceiptInvoiceResponse : BaseResponse
    {
        public List<CategoryModel> ReasonOfPaymentList { get; set; }
        public List<CategoryModel> StatusOfPaymentList { get; set; }
        public List<EmployeeModel> EmployeeList { get; set; }
    }
}
