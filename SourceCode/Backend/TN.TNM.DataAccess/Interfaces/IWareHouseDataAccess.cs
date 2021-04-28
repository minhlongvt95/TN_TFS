using TN.TNM.DataAccess.Messages.Parameters.WareHouse;
using TN.TNM.DataAccess.Messages.Results.WareHouse;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IWareHouseDataAccess
    {
        CreateUpdateWareHouseResult CreateUpdateWareHouse(CreateUpdateWareHouseParameter parameter);
        SearchWareHouseResult SearchWareHouse(SearchWareHouseParameter parameter);
        GetWareHouseChaResult GetWareHouseCha(GetWareHouseChaParameter parameter);
        GetVendorOrderByVendorIdResult GetVendorOrderByVendorId(GetVendorOrderByVendorIdParameter parameter);
        GetVendorOrderDetailByVenderOrderIdResult GetVendorOrderDetailByVenderOrderId(GetVendorOrderDetailByVenderOrderIdParameter parameter);
        DownloadTemplateSerialResult DownloadTemplateSerial(DownloadTemplateSerialParameter parameter);
        CreateOrUpdateInventoryVoucherResult CreateOrUpdateInventoryVoucher(CreateOrUpdateInventoryVoucherParameter parameter);
        RemoveWareHouseResult RemoveWareHouse(RemoveWareHouseParameter parameter);
        GetInventoryReceivingVoucherByIdResult GetInventoryReceivingVoucherById(GetInventoryReceivingVoucherByIdParameter parameter);
        GetListInventoryReceivingVoucherResult GetListInventoryReceivingVoucher(GetListInventoryReceivingVoucherParameter parameter);
        GetListCustomerOrderByIdCustomerIdResult GetListCustomerOrderByIdCustomerId(GetListCustomerOrderByIdCustomerIdParameter parameter);
        GetCustomerOrderDetailByCustomerOrderIdResult GetCustomerOrderDetailByCustomerOrderId(GetCustomerOrderDetailByCustomerOrderIdParameter parameter);
        CheckQuantityActualReceivingVoucherResult CheckQuantityActualReceivingVoucher(CheckQuantityActualReceivingVoucherParameter parameter);
        FilterVendorResult FilterVendor(FilterVendorParameter parameter);
        FilterCustomerResult FilterCustomer(FilterCustomerParameter parameter);
        ChangeStatusInventoryReceivingVoucherResult ChangeStatusInventoryReceivingVoucher(ChangeStatusInventoryReceivingVoucherParameter parameter);
        DeleteInventoryReceivingVoucherResult DeleteInventoryReceivingVoucher(DeleteInventoryReceivingVoucherParameter parameter);
        InventoryDeliveryVoucherFilterCustomerOrderResult InventoryDeliveryVoucherFilterCustomerOrder(InventoryDeliveryVoucherFilterCustomerOrderParameter parameter);
        InventoryDeliveryVoucherFilterVendorOrderResult InventoryDeliveryVoucherFilterVendorOrder(InventoryDeliveryVoucherFilterVendorOrderParameter parameter);
        GetTop10WarehouseFromReceivingVoucherResult GetTop10WarehouseFromReceivingVoucher(GetTop10WarehouseFromReceivingVoucherParameter parameter);
        GetSerialResult GetSerial(GetSerialParameter parameter);
        CreateUpdateInventoryDeliveryVoucherResult CreateUpdateInventoryDeliveryVoucher(CreateUpdateInventoryDeliveryVoucherParameter parameter);
        GetInventoryDeliveryVoucherByIdResult GetInventoryDeliveryVoucherById(GetInventoryDeliveryVoucherByIdParameter parameter);
        DeleteInventoryDeliveryVoucherResult DeleteInventoryDeliveryVoucher(DeleteInventoryDeliveryVoucherParameter parameter);
        ChangeStatusInventoryDeliveryVoucherResult ChangeStatusInventoryDeliveryVoucher(ChangeStatusInventoryDeliveryVoucherParameter parameter);
        FilterCustomerInInventoryDeliveryVoucherResult FilterCustomerInInventoryDeliveryVoucher(FilterCustomerInInventoryDeliveryVoucherParameter parameter);
        SearchInventoryDeliveryVoucherResult SearchInventoryDeliveryVoucher(SearchInventoryDeliveryVoucherParameter parameter);
        FilterProductResult FilterProduct(FilterProductParameter parameter);
        GetProductNameAndProductCodeResult GetProductNameAndProductCode(GetProductNameAndProductCodeParameter parameter);
        GetVendorInvenoryReceivingResult GetVendorInvenoryReceiving(GetVendorInvenoryReceivingParameter parameter);
        GetCustomerDeliveryResult GetCustomerDelivery(GetCustomerDeliveryParameter parameter);
        InStockReportResult InStockReport(InStockReportParameter parameter);
        CreateUpdateWarehouseMasterdataResult CreateUpdateWarehouseMasterdata(CreateUpdateWarehouseMasterdataParameter parameter);

        GetMasterDataSearchInStockReportResult GetMasterDataSearchInStockReport(
            GetMasterDataSearchInStockReportParameter parameter);

        SearchInStockReportResult SearchInStockReport(SearchInStockReportParameter parameter);
        GetMasterDataPhieuNhapKhoResult GetMasterDataPhieuNhapKho(GetMasterDataPhieuNhapKhoParameter parameter);
        GetDanhSachSanPhamCuaPhieuResult GetDanhSachSanPhamCuaPhieu(GetDanhSachSanPhamCuaPhieuParameter parameter);
        GetDanhSachKhoConResult GetDanhSachKhoCon(GetDanhSachKhoConParameter parameter);
        CreateItemInventoryReportResult CreateItemInventoryReport(CreateItemInventoryReportParameter parameter);
        UpdateItemInventoryReportResult UpdateItemInventoryReport(UpdateItemInventoryReportParameter parameter);
        CreateUpdateSerialResult CreateUpdateSerial(CreateUpdateSerialParameter parameter);
        DeleteItemInventoryReportResult DeleteItemInventoryReport(DeleteItemInventoryReportParameter parameter);
        GetSoGTCuaSanPhamTheoKhoResult GetSoGTCuaSanPhamTheoKho(GetSoGTCuaSanPhamTheoKhoParameter parameter);
        CreatePhieuNhapKhoResult CreatePhieuNhapKho(CreatePhieuNhapKhoParameter parameter);
        GetDetailPhieuNhapKhoResult GetDetailPhieuNhapKho(GetDetailPhieuNhapKhoParameter parameter);
        SuaPhieuNhapKhoResult SuaPhieuNhapKho(SuaPhieuNhapKhoParameter parameter);
        KiemTraKhaDungPhieuNhapKhoResult KiemTraKhaDungPhieuNhapKho(KiemTraKhaDungPhieuNhapKhoParameter parameter);
        DanhDauCanLamPhieuNhapKhoResult DanhDauCanLamPhieuNhapKho(DanhDauCanLamPhieuNhapKhoParameter parameter);
        NhanBanPhieuNhapKhoResult NhanBanPhieuNhapKho(NhanBanPhieuNhapKhoParameter parameter);
        XoaPhieuNhapKhoResult XoaPhieuNhapKho(XoaPhieuNhapKhoParameter parameter);
        HuyPhieuNhapKhoResult HuyPhieuNhapKho(HuyPhieuNhapKhoParameter parameter);
        KhongGiuPhanPhieuNhapKhoResult KhongGiuPhanPhieuNhapKho(KhongGiuPhanPhieuNhapKhoParameter parameter);
        KiemTraNhapKhoResult KiemTraNhapKho(KiemTraNhapKhoParameter parameter);
        DatVeNhapPhieuNhapKhoResult DatVeNhapPhieuNhapKho(DatVeNhapPhieuNhapKhoParameter parameter);
        GetListProductPhieuNhapKhoResult GetListProductPhieuNhapKho(GetListProductPhieuNhapKhoParameter parameter);

        GetMasterDataListPhieuNhapKhoResult GetMasterDataListPhieuNhapKho(
            GetMasterDataListPhieuNhapKhoParameter parameter);

        SearchListPhieuNhapKhoResult SearchListPhieuNhapKho(SearchListPhieuNhapKhoParameter parameter);
    }
}
