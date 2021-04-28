using TN.TNM.BusinessLogic.Messages.Requests.WareHouse;
using TN.TNM.BusinessLogic.Messages.Responses.WareHouse;

namespace TN.TNM.BusinessLogic.Interfaces.WareHouse
{
    public interface IWareHouse
    {
        CreateUpdateWareHouseResponse CreateUpdateWareHouse(CreateUpdateWareHouseRequest request);
        SearchWareHouseResponse SearchWareHouse(SearchWareHouseRequest request);
        GetWareHouseChaResponse GetWareHouseCha(GetWareHouseChaRequest request);
        GetVendorOrderByVendorIdResponse GetVendorOrderByVendorId(GetVendorOrderByVendorIdRequest request);
        GetVendorOrderDetailByVenderOrderIdResponse GetVendorOrderDetailByVenderOrderId(GetVendorOrderDetailByVenderOrderIdRequest request);
        DownloadTemplateSerialResponse DownloadTemplateSerial(DownloadTemplateSerialRequest request);
        CreateOrUpdateInventoryVoucherResponse CreateOrUpdateInventoryVoucher(CreateOrUpdateInventoryVoucherRequest request);
        RemoveWareHouseResponse RemoveWareHouse(RemoveWareHouseRequest request);
        GetInventoryReceivingVoucherByIdResponse GetInventoryReceivingVoucherById(GetInventoryReceivingVoucherByIdRequest request);
        GetListInventoryReceivingVoucherResponse GetListInventoryReceivingVoucher(GetListInventoryReceivingVoucherRequest request);
        GetListCustomerOrderByIdCustomerIdResponse GetListCustomerOrderByIdCustomerId(GetListCustomerOrderByIdCustomerIdRequest request);
        GetCustomerOrderDetailByCustomerOrderIdResponse GetCustomerOrderDetailByCustomerOrderId(GetCustomerOrderDetailByCustomerOrderIdRequest request);
        CheckQuantityActualReceivingVoucherResponse CheckQuantityActualReceivingVoucher(CheckQuantityActualReceivingVoucherRequest request);
        FilterVendorResponse FilterVendor(FilterVendorRequest request);
        FilterCustomerResponse FilterCustomer(FilterCustomerRequest request);
        ChangeStatusInventoryReceivingVoucherResponse ChangeStatusInventoryReceivingVoucher(ChangeStatusInventoryReceivingVoucherRequest request);
        DeleteInventoryReceivingVoucherResponse DeleteInventoryReceivingVoucher(DeleteInventoryReceivingVoucherRequest request);
        InventoryDeliveryVoucherFilterVendorOrderResponse InventoryDeliveryVoucherFilterVendorOrder(InventoryDeliveryVoucherFilterVendorOrderRequest request);
        InventoryDeliveryVoucherFilterCustomerOrderResponse InventoryDeliveryVoucherFilterCustomerOrder(InventoryDeliveryVoucherFilterCustomerOrderRequest request);
        GetTop10WarehouseFromReceivingVoucherResponse GetTop10WarehouseFromReceivingVoucher(GetTop10WarehouseFromReceivingVoucherRequest request);
        GetSerialResponse GetSerial(GetSerialRequest request);
        CreateUpdateInventoryDeliveryVoucherResponse CreateUpdateInventoryDeliveryVoucher(CreateUpdateInventoryDeliveryVoucherRequest request);
        GetInventoryDeliveryVoucherByIdResponse GetInventoryDeliveryVoucherById(GetInventoryDeliveryVoucherByIdRequest request);
        DeleteInventoryDeliveryVoucherResponse DeleteInventoryDeliveryVoucher(DeleteInventoryDeliveryVoucherRequest request);
        ChangeStatusInventoryDeliveryVoucherResponse ChangeStatusInventoryDeliveryVoucher(ChangeStatusInventoryDeliveryVoucherRequest request);
        FilterCustomerInInventoryDeliveryVoucherResponse FilterCustomerInInventoryDeliveryVoucher(FilterCustomerInInventoryDeliveryVoucherRequest request);
        SearchInventoryDeliveryVoucherResponse SearchInventoryDeliveryVoucher(SearchInventoryDeliveryVoucherRequest request);
        FilterProductResponse FilterProduct(FilterProductRequest request);
        GetProductNameAndProductCodeResponse GetProductNameAndProductCode(GetProductNameAndProductCodeRequest request);
        GetVendorInvenoryReceivingResponse GetVendorInvenoryReceiving(GetVendorInvenoryReceivingRequest request);
        GetCustomerDeliveryResponse GetCustomerDelivery(GetCustomerDeliveryRequest request);
        InStockReportResponse InStockReport(InStockReportRequest request);
        CreateUpdateWarehouseMasterdataResponse CreateUpdateWarehouseMasterdata(CreateUpdateWarehouseMasterdataRequest request);

        GetMasterDataSearchInStockReportResponse GetMasterDataSearchInStockReport(
            GetMasterDataSearchInStockReportRequest request);

        SearchInStockReportResponse SearchInStockReport(SearchInStockReportRequest request);
        GetMasterDataPhieuNhapKhoResponse GetMasterDataPhieuNhapKho(GetMasterDataPhieuNhapKhoRequest request);
        GetDanhSachSanPhamCuaPhieuResponse GetDanhSachSanPhamCuaPhieu(GetDanhSachSanPhamCuaPhieuRequest request);
        GetDanhSachKhoConResponse GetDanhSachKhoCon(GetDanhSachKhoConRequest request);
        CreateItemInventoryReportResponse CreateItemInventoryReport(CreateItemInventoryReportRequest request);
        UpdateItemInventoryReportResponse UpdateItemInventoryReport(UpdateItemInventoryReportRequest request);
        CreateUpdateSerialResponse CreateUpdateSerial(CreateUpdateSerialRequest request);
        DeleteItemInventoryReportResponse DeleteItemInventoryReport(DeleteItemInventoryReportRequest request);
        GetSoGTCuaSanPhamTheoKhoResponse GetSoGTCuaSanPhamTheoKho(GetSoGTCuaSanPhamTheoKhoRequest request);
        CreatePhieuNhapKhoResponse CreatePhieuNhapKho(CreatePhieuNhapKhoRequest request);
        GetDetailPhieuNhapKhoResponse GetDetailPhieuNhapKho(GetDetailPhieuNhapKhoRequest request);
        SuaPhieuNhapKhoResponse SuaPhieuNhapKho(SuaPhieuNhapKhoRequest request);
        KiemTraKhaDungPhieuNhapKhoResponse KiemTraKhaDungPhieuNhapKho(KiemTraKhaDungPhieuNhapKhoRequest request);
        DanhDauCanLamPhieuNhapKhoResponse DanhDauCanLamPhieuNhapKho(DanhDauCanLamPhieuNhapKhoRequest request);
        NhanBanPhieuNhapKhoResponse NhanBanPhieuNhapKho(NhanBanPhieuNhapKhoRequest request);
        XoaPhieuNhapKhoResponse XoaPhieuNhapKho(XoaPhieuNhapKhoRequest request);
        HuyPhieuNhapKhoResponse HuyPhieuNhapKho(HuyPhieuNhapKhoRequest request);
        KhongGiuPhanPhieuNhapKhoResponse KhongGiuPhanPhieuNhapKho(KhongGiuPhanPhieuNhapKhoRequest request);
        KiemTraNhapKhoResponse KiemTraNhapKho(KiemTraNhapKhoRequest request);
        DatVeNhapPhieuNhapKhoResponse DatVeNhapPhieuNhapKho(DatVeNhapPhieuNhapKhoRequest request);
        GetListProductPhieuNhapKhoResponse GetListProductPhieuNhapKho(GetListProductPhieuNhapKhoRequest request);

        GetMasterDataListPhieuNhapKhoResponse GetMasterDataListPhieuNhapKho(
            GetMasterDataListPhieuNhapKhoRequest request);

        SearchListPhieuNhapKhoResponse SearchListPhieuNhapKho(SearchListPhieuNhapKhoRequest request);
    }
}
