using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class SanPhamPhieuNhapKhoModel
    {
        public Guid? InventoryReceivingVoucherMappingId { get; set; }
        public Guid? InventoryReceivingVoucherId { get; set; }
        public Guid? ObjectId { get; set; }
        public Guid? ObjectDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal QuantityRequest { get; set; }
        public decimal QuantityReservation { get; set; } //Số lượng đặt trước
        public decimal QuantityActual { get; set; }
        public bool PriceAverage { get; set; }
        public decimal PriceProduct { get; set; }   //giá nhập
        public Guid? WarehouseId { get; set; }

        public string OrderCode { get; set; }   //Mã phiếu
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string UnitName { get; set; }    //Đơn vị tính
        public string WarehouseName { get; set; }   //vị trí (Kho)
        public string WarehouseCodeName { get; set; }
        public decimal Amount { get; set; } //Thành tiền = Giá nhập * Số lượng
        public int? Index { get; set; }

    }
}
