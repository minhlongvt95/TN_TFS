using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class GetMasterDataReceiptInvoiceResult : BaseResult
    {
        public List<CategoryEntityModel> ReasonOfReceiptList { get; set; }
        public List<CategoryEntityModel> TypesOfReceiptList { get; set; }
        public List<CategoryEntityModel> StatusOfReceiptList { get; set; }
        public List<CategoryEntityModel> UnitMoneyList { get; set; }
        public List<Organization> OrganizationList { get; set; }
        public List<Databases.Entities.Customer> CustomerList
        {
            get; set;
        }
    }
}
