using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class GetMasterDataSearchReceiptInvoiceResponse : BaseResponse
    {
        public List<CategoryModel> ListReason { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
    }
}
