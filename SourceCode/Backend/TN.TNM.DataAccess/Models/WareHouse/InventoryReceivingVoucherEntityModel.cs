using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class InventoryReceivingVoucherEntityModel
    {
        public Guid? InventoryReceivingVoucherId { get; set; }
        public string InventoryReceivingVoucherCode { get; set; }
        public Guid? StatusId { get; set; }
        public int? InventoryReceivingVoucherType { get; set; }
        public Guid? WarehouseId { get; set; }
        public string ShiperName { get; set; }
        public Guid? Storekeeper { get; set; }
        public DateTime? InventoryReceivingVoucherDate { get; set; }    //Ngày nhập kho
        public TimeSpan? InventoryReceivingVoucherTime { get; set; }
        public int? LicenseNumber { get; set; }
        public DateTime? ExpectedDate { get; set; } //Ngày gửi dự kiến
        public string Description { get; set; }
        public string Note { get; set; }
        public Guid? PartnersId { get; set; } //Id đối tác
        public DateTime? CreatedDate { get; set; } //Ngày lập phiếu (Tạo phiếu)
        public Guid? CreatedById { get; set; } //UserId người lập phiếu (Tạo phiếu)

        public string PartnersName { get; set; } //Tên Đối tác
        public string CreatedName { get; set; } //Tên người lập phiếu
        public string InventoryReceivingVoucherTypeName { get; set; } //Tên loại phiếu
        public string StatusName { get; set; } //Tên trạng thái
    }
}
