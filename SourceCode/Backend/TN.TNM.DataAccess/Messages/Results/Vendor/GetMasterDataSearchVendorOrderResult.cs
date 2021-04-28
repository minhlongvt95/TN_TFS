using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetMasterDataSearchVendorOrderResult : BaseResult
    {
        public List<Databases.Entities.Vendor> Vendors { get; set; }
        public List<PurchaseOrderStatus> OrderStatuses { get; set; }
        public List<Databases.Entities.Employee> Employees { get; set; }
        public List<ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public DataAccess.Models.CompanyConfigEntityModel CompanyConfig { get; set; }
    }
}
