using TN.TNM.BusinessLogic.Messages.Requests.Order;
using TN.TNM.BusinessLogic.Messages.Responses.Order;

namespace TN.TNM.BusinessLogic.Interfaces.Order
{
    public interface ICustomerOrder
    {
        GetAllCustomerOrderResponse GetAllCustomerOrder(GetAllCustomerOrderRequest request);
        CreateCustomerOrderResponse CreateCustomerOrder(CreateCustomerOrderRequest request);
        UpdateCustomerOrderResponse UpdateCustomerOrder(UpdateCustomerOrderRequest request);
        GetCustomerOrderByIDResponse GetCustomerOrderByID(GetCustomerOrderByIDRequest request);
        ExportCustomerOrderPDFResponse ExportPdfCustomerOrder(ExportCustomerOrderPDFRequest request);
        GetCustomerOrderBySellerResponse GetCustomerOrderBySeller(GetCustomerOrderBySellerRequest request);
        GetEmployeeListByOrganizationIdResponse GetEmployeeListByOrganizationId(GetEmployeeListByOrganizationIdRequest request);
        GetMonthsListResponse GetMonthsList(GetMonthsListRequest request);
        GetProductCategoryGroupByLevelResponse GetProductCategoryGroupByLevel(GetProductCategoryGroupByLevelRequest request);
        GetProductCategoryGroupByManagerResponse GetProductCategoryGroupByManager(GetProductCategoryGroupByManagerRequest request);
        GetMasterDataOrderSearchResponse GetMasterDataOrderSearch(GetMasterDataOrderSearchRequest request);
        SearchOrderResponse SearchOrder(SearchOrderRequest request);
        GetMasterDataOrderCreateResponse GetMasterDataOrderCreate(GetMasterDataOrderCreateRequest request);

        GetMasterDataOrderDetailDialogResponse GetMasterDataOrderDetailDialog(
            GetMasterDataOrderDetailDialogRequest request);

        GetVendorByProductIdResponse GetVendorByProductId(GetVendorByProductIdRequest request);
        GetMasterDataOrderDetailResponse GetMasterDataOrderDetail(GetMasterDataOrderDetailRequest request);
        DeleteOrderResponse DeleteOrder(DeleteOrderRequest request);
        GetDataDashboardHomeResponse GetDataDashboardHome(GetDataDashboardHomeRequest request);
        CheckReceiptOrderHistoryResponse CheckReceiptOrderHistory(CheckReceiptOrderHistoryRequest request);
        CheckBeforCreateOrUpdateOrderResponse CheckBeforCreateOrUpdateOrder(CheckBeforCreateOrUpdateOrderRequest request);
        UpdateCustomerOrderResponse UpdateStatusOrder(UpdateStatusOrderRequest request);
        ProfitAccordingCustomersResponse SearchProfitAccordingCustomers(ProfitAccordingCustomersRequest request);

        GetMasterDataOrderServiceCreateResponse GetMasterDataOrderServiceCreate(
            GetMasterDataOrderServiceCreateRequest request);

        CreateOrderServiceResponse CreateOrderService(CreateOrderServiceRequest request);
        GetMasterDataPayOrderServiceResponse GetMasterDataPayOrderService(GetMasterDataPayOrderServiceRequest request);
        GetListOrderByLocalPointResponse GetListOrderByLocalPoint(GetListOrderByLocalPointRequest request);
        PayOrderByLocalPointResponse PayOrderByLocalPoint(PayOrderByLocalPointRequest request);
        CheckExistsCustomerByPhoneResponse CheckExistsCustomerByPhone(CheckExistsCustomerByPhoneRequest request);
        RefreshLocalPointResponse RefreshLocalPoint(RefreshLocalPointRequest request);
        GetLocalPointByLocalAddressResponse GetLocalPointByLocalAddress(GetLocalPointByLocalAddressRequest request);
        GetDataSearchTopReVenueResponse GetDataSearchTopReVenue(GetDataSearchTopReVenueRequest request);
        SearchTopReVenueResponse SearchTopReVenue(SearchTopReVenueRequest request);
        GetDataSearchRevenueProductResponse GetDataSearchRevenueProduct(GetDataSearchRevenueProductRequest request);
        SearchRevenueProductResponse SearchRevenueProduct(SearchRevenueProductRequest request);
        GetListOrderDetailByOrderResponse GetListOrderDetailByOrder(GetListOrderDetailByOrderRequest request);
        GetListProductWasOrderResponse GetListProductWasOrder(GetListProductWasOrderRequest request);
        UpdateCustomerServiceResponse UpdateCustomerService(UpdateCustomerServiceRequest request);
        GetDataProfitByCustomerResponse GetDataProfitByCustomer(GetDataProfitByCustomerRequest request);
        SearchProfitCustomerResponse SearchProfitCustomer(SearchProfitCustomerRequest request);
    }
}
