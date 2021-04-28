using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class PhieuNhapKhoModel
    {
        public Guid? InventoryReceivingVoucherId { get; set; }
        public string InventoryReceivingVoucherCode { get; set; }
        public Guid? StatusId { get; set; }
        public int? InventoryReceivingVoucherType { get; set; }
        public Guid? WarehouseId { get; set; }
        public string ShiperName { get; set; }
        public Guid? Storekeeper { get; set; }
        public DateTime? InventoryReceivingVoucherDate { get; set; }
        public TimeSpan? InventoryReceivingVoucherTime { get; set; }
        public int? LicenseNumber { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid? PartnersId { get; set; }

        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public string EmployeeCodeName { get; set; } //Người lập phiếu
        public decimal? TotalQuantityActual { get; set; }   //Tổng số lượng thực nhập
    }
}
