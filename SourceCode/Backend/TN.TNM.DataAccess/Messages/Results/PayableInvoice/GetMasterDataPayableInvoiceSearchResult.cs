using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class GetMasterDataPayableInvoiceSearchResult:BaseResult
    {
        public List<CategoryEntityModel> ReasonOfPaymentList { get; set; }
        public List<CategoryEntityModel> StatusOfPaymentList { get; set; }
        public List<Databases.Entities.Employee> lstUserEntityModel { get; set; }
    }
}
