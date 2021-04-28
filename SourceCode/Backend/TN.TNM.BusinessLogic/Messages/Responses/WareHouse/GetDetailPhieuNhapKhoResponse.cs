using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Vendor;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetDetailPhieuNhapKhoResponse : BaseResponse
    {
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<WareHouseEntityModel> ListAllWarehouse { get; set; }
        public List<WareHouseEntityModel> ListWarehouse { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public PhieuNhapKhoModel PhieuNhapKho { get; set; }
        public List<SanPhamPhieuNhapKhoModel> ListItemDetail { get; set; }
        public List<VendorOrderEntityModel> ListVendorOrder { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<Guid?> ListSelectedVendorOrderId { get; set; }
        public List<FileInFolderEntityModel> ListFileUpload { get; set; }
        public List<NoteEntityModel> NoteHistory { get; set; }
    }
}
