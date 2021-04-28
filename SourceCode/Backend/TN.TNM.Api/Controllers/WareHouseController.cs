using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.WareHouse;
using TN.TNM.BusinessLogic.Messages.Requests.WareHouse;
using TN.TNM.BusinessLogic.Messages.Responses.WareHouse;

namespace TN.TNM.Api.Controllers
{
    public class WareHouseController : Controller
    {
        private readonly IWareHouse iWareHouse;
        public WareHouseController(IWareHouse _iWareHouse)
        {
            this.iWareHouse = _iWareHouse;
        }

        /// <summary>
        /// Create/Update WareHouse
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/createUpdateWareHouse")]
        [Authorize(Policy = "Member")]
        public CreateUpdateWareHouseResponse CreateUpdateWareHouse([FromBody]CreateUpdateWareHouseRequest request)
        {
            return iWareHouse.CreateUpdateWareHouse(request);
        }

        /// <summary>
        /// Search WareHouse
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/searchWareHouse")]
        [Authorize(Policy = "Member")]
        public SearchWareHouseResponse SearchWareHouse([FromBody]SearchWareHouseRequest request)
        {
            return iWareHouse.SearchWareHouse(request);
        }


        /// <summary>
        /// GetWareHouseCha
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getWareHouseCha")]
        [Authorize(Policy = "Member")]
        public GetWareHouseChaResponse GetWareHouseCha([FromBody]GetWareHouseChaRequest request)
        {
            return iWareHouse.GetWareHouseCha(request);
        }

        /// <summary>
        /// GetVendorOrderByVendorId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getVendorOrderByVendorId")]
        [Authorize(Policy = "Member")]
        public GetVendorOrderByVendorIdResponse GetVendorOrderByVendorId([FromBody]GetVendorOrderByVendorIdRequest request)
        {
            return iWareHouse.GetVendorOrderByVendorId(request);
        }

        /// <summary>
        /// GetVendorOrderDetailByVenderOrderId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getVendorOrderDetailByVenderOrderId")]
        [Authorize(Policy = "Member")]
        public GetVendorOrderDetailByVenderOrderIdResponse GetVendorOrderDetailByVenderOrderId([FromBody]GetVendorOrderDetailByVenderOrderIdRequest request)
        {
            return iWareHouse.GetVendorOrderDetailByVenderOrderId(request);
        }

        /// <summary>
        /// DownloadTemplateSerial
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/downloadTemplateSerial")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateSerialResponse DownloadTemplateSerial([FromBody]DownloadTemplateSerialRequest request)
        {
            return iWareHouse.DownloadTemplateSerial(request);
        }

        /// <summary>
        /// CreateOrUpdateInventoryVoucher
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/createOrUpdateInventoryVoucher")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateInventoryVoucherResponse CreateOrUpdateInventoryVoucher(CreateOrUpdateInventoryVoucherRequest request)
        {
                return iWareHouse.CreateOrUpdateInventoryVoucher(request);
        }

        /// <summary>
        /// Remove WareHouse
        /// </summary>
        /// <param name="request">Remove WareHouse</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/removeWareHouse")]
        [Authorize(Policy = "Member")]
        public RemoveWareHouseResponse RemoveWareHouse([FromBody]RemoveWareHouseRequest request)
        {
            return iWareHouse.RemoveWareHouse(request);
        }
        /// <summary>
        /// GetInventoryReceivingVoucherByIdResponse
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getInventoryReceivingVoucherById")]
        [Authorize(Policy = "Member")]
        public GetInventoryReceivingVoucherByIdResponse GetInventoryReceivingVoucherById([FromBody]GetInventoryReceivingVoucherByIdRequest request)
        {
            return iWareHouse.GetInventoryReceivingVoucherById(request);
        }
        /// <summary>
        /// GetInventoryReceivingVoucherByIdResponse
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getListInventoryReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public GetListInventoryReceivingVoucherResponse GetListInventoryReceivingVoucher([FromBody]GetListInventoryReceivingVoucherRequest request)
        {
            return iWareHouse.GetListInventoryReceivingVoucher(request);
        }
        /// <summary>
        /// GetListCustomerOrderByIdCustomerId
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getListCustomerOrderByIdCustomerId")]
        [Authorize(Policy = "Member")]
        public GetListCustomerOrderByIdCustomerIdResponse GetListCustomerOrderByIdCustomerId([FromBody]GetListCustomerOrderByIdCustomerIdRequest request)
        {
            return iWareHouse.GetListCustomerOrderByIdCustomerId(request);
        }

        /// <summary>
        /// GetCustomerOrderDetailByCustomerOrderId
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getCustomerOrderDetailByCustomerOrderId")]
        [Authorize(Policy = "Member")]
        public GetCustomerOrderDetailByCustomerOrderIdResponse GetCustomerOrderDetailByCustomerOrderId([FromBody]GetCustomerOrderDetailByCustomerOrderIdRequest request)
        {
            return iWareHouse.GetCustomerOrderDetailByCustomerOrderId(request);
        }
        /// <summary>
        /// CheckQuantityActualReceivingVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/checkQuantityActualReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public CheckQuantityActualReceivingVoucherResponse CheckQuantityActualReceivingVoucher([FromBody]CheckQuantityActualReceivingVoucherRequest request)
        {
            return iWareHouse.CheckQuantityActualReceivingVoucher(request);
        }
        /// <summary>
        /// FilterVendor
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/filterVendor")]
        [Authorize(Policy = "Member")]
        public FilterVendorResponse FilterVendor([FromBody]FilterVendorRequest request)
        {
            return iWareHouse.FilterVendor(request);
        }
        /// <summary>
        /// FilterCustomer
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/filterCustomer")]
        [Authorize(Policy = "Member")]
        public FilterCustomerResponse FilterCustomer([FromBody]FilterCustomerRequest request)
        {
            return iWareHouse.FilterCustomer(request);
        }

        /// <summary>
        /// ChangeStatusInventoryReceivingVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/changeStatusInventoryReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public ChangeStatusInventoryReceivingVoucherResponse ChangeStatusInventoryReceivingVoucher([FromBody]ChangeStatusInventoryReceivingVoucherRequest request)
        {
            return iWareHouse.ChangeStatusInventoryReceivingVoucher(request);
        }

        /// <summary>
        /// DeleteInventoryReceivingVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/deleteInventoryReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public DeleteInventoryReceivingVoucherResponse DeleteInventoryReceivingVoucher([FromBody]DeleteInventoryReceivingVoucherRequest request)
        {
            return iWareHouse.DeleteInventoryReceivingVoucher(request);
        }
        /// <summary>
        /// InventoryDeliveryVoucherFilterVendorOrder
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/inventoryDeliveryVoucherFilterVendorOrder")]
        [Authorize(Policy = "Member")]
        public InventoryDeliveryVoucherFilterVendorOrderResponse InventoryDeliveryVoucherFilterVendorOrder([FromBody]InventoryDeliveryVoucherFilterVendorOrderRequest request)
        {
            return iWareHouse.InventoryDeliveryVoucherFilterVendorOrder(request);
        }
        /// <summary>
        /// InventoryDeliveryVoucherFilterCustomerOrder
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/inventoryDeliveryVoucherFilterCustomerOrder")]
        [Authorize(Policy = "Member")]
        public InventoryDeliveryVoucherFilterCustomerOrderResponse InventoryDeliveryVoucherFilterCustomerOrder([FromBody]InventoryDeliveryVoucherFilterCustomerOrderRequest request)
        {
            return iWareHouse.InventoryDeliveryVoucherFilterCustomerOrder(request);
        }
        /// <summary>
        /// GetTop10WarehouseFromReceivingVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getTop10WarehouseFromReceivingVoucher")]
        [Authorize(Policy = "Member")]
        public GetTop10WarehouseFromReceivingVoucherResponse GetTop10WarehouseFromReceivingVoucher([FromBody]GetTop10WarehouseFromReceivingVoucherRequest request)
        {
            return iWareHouse.GetTop10WarehouseFromReceivingVoucher(request);
        }
        /// <summary>
        /// GetSerial
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getSerial")]
        [Authorize(Policy = "Member")]
        public GetSerialResponse GetSerial([FromBody]GetSerialRequest request)
        {
            return iWareHouse.GetSerial(request);
        }
        /// <summary>
        /// CreateUpdateInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/createUpdateInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public CreateUpdateInventoryDeliveryVoucherResponse CreateUpdateInventoryDeliveryVoucher(CreateUpdateInventoryDeliveryVoucherRequest request)
        {
            return iWareHouse.CreateUpdateInventoryDeliveryVoucher(request);
        }

        /// <summary>
        /// GetInventoryDeliveryVoucherById
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getInventoryDeliveryVoucherById")]
        [Authorize(Policy = "Member")]
        public GetInventoryDeliveryVoucherByIdResponse GetInventoryDeliveryVoucherById([FromBody]GetInventoryDeliveryVoucherByIdRequest request)
        {
            return iWareHouse.GetInventoryDeliveryVoucherById(request);
        }
        /// <summary>
        /// DeleteInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/deleteInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public DeleteInventoryDeliveryVoucherResponse DeleteInventoryDeliveryVoucher([FromBody]DeleteInventoryDeliveryVoucherRequest request)
        {
            return iWareHouse.DeleteInventoryDeliveryVoucher(request);
        }

        /// <summary>
        /// ChangeStatusInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/changeStatusInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public ChangeStatusInventoryDeliveryVoucherResponse ChangeStatusInventoryDeliveryVoucherRequest([FromBody]ChangeStatusInventoryDeliveryVoucherRequest request)
        {
            return iWareHouse.ChangeStatusInventoryDeliveryVoucher(request);
        }
        /// <summary>
        /// FilterCustomerInInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/filterCustomerInInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public FilterCustomerInInventoryDeliveryVoucherResponse FilterCustomerInInventoryDeliveryVoucher([FromBody]FilterCustomerInInventoryDeliveryVoucherRequest request)
        {
            return iWareHouse.FilterCustomerInInventoryDeliveryVoucher(request);
        }
        /// <summary>
        /// searchInventoryDeliveryVoucher
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/searchInventoryDeliveryVoucher")]
        [Authorize(Policy = "Member")]
        public SearchInventoryDeliveryVoucherResponse SearchInventoryDeliveryVoucher([FromBody]SearchInventoryDeliveryVoucherRequest request)
        {
            return iWareHouse.SearchInventoryDeliveryVoucher(request);
        }
        /// <summary>
        /// FilterProduct
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/filterProduct")]
        [Authorize(Policy = "Member")]
        public FilterProductResponse FilterProduct([FromBody]FilterProductRequest request)
        {
            return iWareHouse.FilterProduct(request);
        }
        /// <summary>
        /// GetProductNameAndProductCode
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getProductNameAndProductCode")]
        [Authorize(Policy = "Member")]
        public GetProductNameAndProductCodeResponse GetProductNameAndProductCode([FromBody]GetProductNameAndProductCodeRequest request)
        {
            return iWareHouse.GetProductNameAndProductCode(request);
        }
        /// <summary>
        /// GetVendorInvenoryReceiving
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getVendorInvenoryReceiving")]
        [Authorize(Policy = "Member")]
        public GetVendorInvenoryReceivingResponse GetVendorInvenoryReceiving([FromBody]GetVendorInvenoryReceivingRequest request)
        {
            return iWareHouse.GetVendorInvenoryReceiving(request);
        }

        /// <summary>
        /// GetCustomerDelivery
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/getCustomerDelivery")]
        [Authorize(Policy = "Member")]
        public GetCustomerDeliveryResponse GetCustomerDelivery([FromBody]GetCustomerDeliveryRequest request)
        {
            return iWareHouse.GetCustomerDelivery(request);
        }
        /// <summary>
        /// InStockReport
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/inStockReport")]
        [Authorize(Policy = "Member")]
        public InStockReportResponse InStockReport([FromBody]InStockReportRequest request)
        {
            return iWareHouse.InStockReport(request);
        }

        /// <summary>
        /// Create Update Warehouse Masterdata
        /// </summary>
        /// <param name="request">Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/warehouse/createUpdateWarehouseMasterdata")]
        [Authorize(Policy = "Member")]
        public CreateUpdateWarehouseMasterdataResponse CreateUpdateWarehouseMasterdata([FromBody]CreateUpdateWarehouseMasterdataRequest request)
        {
            return iWareHouse.CreateUpdateWarehouseMasterdata(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getMasterDataSearchInStockReport")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchInStockReportResponse GetMasterDataSearchInStockReport([FromBody]GetMasterDataSearchInStockReportRequest request)
        {
            return iWareHouse.GetMasterDataSearchInStockReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/searchInStockReport")]
        [Authorize(Policy = "Member")]
        public SearchInStockReportResponse SearchInStockReport([FromBody]SearchInStockReportRequest request)
        {
            return iWareHouse.SearchInStockReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getMasterDataPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPhieuNhapKhoResponse GetMasterDataPhieuNhapKho([FromBody]GetMasterDataPhieuNhapKhoRequest request)
        {
            return iWareHouse.GetMasterDataPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getDanhSachSanPhamCuaPhieu")]
        [Authorize(Policy = "Member")]
        public GetDanhSachSanPhamCuaPhieuResponse GetDanhSachSanPhamCuaPhieu([FromBody]GetDanhSachSanPhamCuaPhieuRequest request)
        {
            return iWareHouse.GetDanhSachSanPhamCuaPhieu(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getDanhSachKhoCon")]
        [Authorize(Policy = "Member")]
        public GetDanhSachKhoConResponse GetDanhSachKhoCon([FromBody]GetDanhSachKhoConRequest request)
        {
            return iWareHouse.GetDanhSachKhoCon(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/createItemInventoryReport")]
        [Authorize(Policy = "Member")]
        public CreateItemInventoryReportResponse CreateItemInventoryReport([FromBody]CreateItemInventoryReportRequest request)
        {
            return iWareHouse.CreateItemInventoryReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/updateItemInventoryReport")]
        [Authorize(Policy = "Member")]
        public UpdateItemInventoryReportResponse UpdateItemInventoryReport([FromBody]UpdateItemInventoryReportRequest request)
        {
            return iWareHouse.UpdateItemInventoryReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/createUpdateSerial")]
        [Authorize(Policy = "Member")]
        public CreateUpdateSerialResponse CreateUpdateSerial([FromBody]CreateUpdateSerialRequest request)
        {
            return iWareHouse.CreateUpdateSerial(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/deleteItemInventoryReport")]
        [Authorize(Policy = "Member")]
        public DeleteItemInventoryReportResponse DeleteItemInventoryReport([FromBody]DeleteItemInventoryReportRequest request)
        {
            return iWareHouse.DeleteItemInventoryReport(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getSoGTCuaSanPhamTheoKho")]
        [Authorize(Policy = "Member")]
        public GetSoGTCuaSanPhamTheoKhoResponse GetSoGTCuaSanPhamTheoKho([FromBody]GetSoGTCuaSanPhamTheoKhoRequest request)
        {
            return iWareHouse.GetSoGTCuaSanPhamTheoKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/createPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public CreatePhieuNhapKhoResponse CreatePhieuNhapKho(CreatePhieuNhapKhoRequest request)
        {
            return iWareHouse.CreatePhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getDetailPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public GetDetailPhieuNhapKhoResponse GetDetailPhieuNhapKho([FromBody]GetDetailPhieuNhapKhoRequest request)
        {
            return iWareHouse.GetDetailPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/suaPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public SuaPhieuNhapKhoResponse SuaPhieuNhapKho([FromBody]SuaPhieuNhapKhoRequest request)
        {
            return iWareHouse.SuaPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/kiemTraKhaDungPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public KiemTraKhaDungPhieuNhapKhoResponse SuaPhieuNhapKho([FromBody]KiemTraKhaDungPhieuNhapKhoRequest request)
        {
            return iWareHouse.KiemTraKhaDungPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/danhDauCanLamPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public DanhDauCanLamPhieuNhapKhoResponse DanhDauCanLamPhieuNhapKho([FromBody]DanhDauCanLamPhieuNhapKhoRequest request)
        {
            return iWareHouse.DanhDauCanLamPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/nhanBanPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public NhanBanPhieuNhapKhoResponse NhanBanPhieuNhapKho([FromBody]NhanBanPhieuNhapKhoRequest request)
        {
            return iWareHouse.NhanBanPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/xoaPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public XoaPhieuNhapKhoResponse XoaPhieuNhapKho([FromBody]XoaPhieuNhapKhoRequest request)
        {
            return iWareHouse.XoaPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/huyPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public HuyPhieuNhapKhoResponse HuyPhieuNhapKho([FromBody]HuyPhieuNhapKhoRequest request)
        {
            return iWareHouse.HuyPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/khongGiuPhanPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public KhongGiuPhanPhieuNhapKhoResponse KhongGiuPhanPhieuNhapKho([FromBody]KhongGiuPhanPhieuNhapKhoRequest request)
        {
            return iWareHouse.KhongGiuPhanPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/kiemTraNhapKho")]
        [Authorize(Policy = "Member")]
        public KiemTraNhapKhoResponse KiemTraNhapKho([FromBody]KiemTraNhapKhoRequest request)
        {
            return iWareHouse.KiemTraNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/datVeNhapPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public DatVeNhapPhieuNhapKhoResponse DatVeNhapPhieuNhapKho([FromBody]DatVeNhapPhieuNhapKhoRequest request)
        {
            return iWareHouse.DatVeNhapPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getListProductPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public GetListProductPhieuNhapKhoResponse GetListProductPhieuNhapKho([FromBody]GetListProductPhieuNhapKhoRequest request)
        {
            return iWareHouse.GetListProductPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/getMasterDataListPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public GetMasterDataListPhieuNhapKhoResponse GetMasterDataListPhieuNhapKho([FromBody]GetMasterDataListPhieuNhapKhoRequest request)
        {
            return iWareHouse.GetMasterDataListPhieuNhapKho(request);
        }

        //
        [HttpPost]
        [Route("api/warehouse/searchListPhieuNhapKho")]
        [Authorize(Policy = "Member")]
        public SearchListPhieuNhapKhoResponse SearchListPhieuNhapKho([FromBody]SearchListPhieuNhapKhoRequest request)
        {
            return iWareHouse.SearchListPhieuNhapKho(request);
        }
    }
}
