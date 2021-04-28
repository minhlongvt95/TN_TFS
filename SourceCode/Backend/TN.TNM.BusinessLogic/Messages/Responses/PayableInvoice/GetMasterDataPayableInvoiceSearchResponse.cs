using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Customer;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class GetMasterDataPayableInvoiceSearchResponse:BaseResponse
    {
        public List<CategoryModel> ReasonOfPaymentList { get; set; }
        public List<CategoryModel> StatusOfPaymentList { get; set; }
        public List<EmployeeModel> lstUserEntityModel { get; set; }
    }
}
