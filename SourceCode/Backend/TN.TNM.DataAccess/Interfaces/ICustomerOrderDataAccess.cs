using TN.TNM.DataAccess.Messages.Parameters.Order;
using TN.TNM.DataAccess.Messages.Results.Order;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICustomerOrderDataAccess
    {
        GetAllCustomerOrderResult GetAllCustomerOrder(GetAllCustomerOrderParameter parameter);
        CreateCustomerOrderResult CreateCustomerOrder(CreateCustomerOrderParameter parameter);
        UpdateCustomerOrderResult UpdateCustomerOrder(UpdateCustomerOrderParameter parameter);
        GetCustomerOrderByIDResult GetCustomerOrderByID(GetCustomerOrderByIDParameter parameter);
        ExportCustomerOrderPDFResult ExportPdfCustomerOrder(ExportCustomerOrderPDFParameter parameter);
        GetCustomerOrderBySellerResult GetCustomerOrderBySeller(GetCustomerOrderBySellerParameter parameter);
        GetEmployeeListByOrganizationIdResult GetEmployeeListByOrganizationId(GetEmployeeListByOrganizationIdParameter parameter);
        GetMonthsListResult GetMonthsList(GetMonthsListParameter parameter);
        GetProductCategoryGroupByLevelResult GetProductCategoryGroupByLevel(GetProductCategoryGroupByLevelParameter parameter);
        GetProductCategoryGroupByManagerResult GetProductCategoryGroupByManager(GetProductCategoryGroupByManagerParameter parameter);
        GetMasterDataOrderSearchResult GetMasterDataOrderSearch(GetMasterDataOrderSearchParameter parameter);
        SearchOrderResult SearchOrder(SearchOrderParameter parameter);
        GetMasterDataOrderCreateResult GetMasterDataOrderCreate(GetMasterDataOrderCreateParameter parameter);

        GetMasterDataOrderDetailDialogResult GetMasterDataOrderDetailDialog(
            GetMasterDataOrderDetailDialogParameter parameter);

        GetVendorByProductIdResult GetVendorByProductId(GetVendorByProductIdParameter parameter);

        GetMasterDataOrderDetailResult GetMasterDataOrderDetail(GetMasterDataOrderDetailParameter parameter);
        DeleteOrderResult DeleteOrder(DeleteOrderParameter parameter);
        GetDataDashboardHomeResult GetDataDashboardHome(GetDataDashboardHomeParameter parameter);
        CheckReceiptOrderHistoryResult CheckReceiptOrderHistory(CheckReceiptOrderHistoryParameter parameter);
        CheckBeforCreateOrUpdateOrderResult CheckBeforCreateOrUpdateOrder(CheckBeforCreateOrUpdateOrderParameter parameter);
        UpdateCustomerOrderResult UpdateStatusOrder(UpdateStatusOrderParameter parameter);
        ProfitAccordingCustomersResult SearchProfitAccordingCustomers(ProfitAccordingCustomersParameter parameter);

        GetMasterDataOrderServiceCreateResult GetMasterDataOrderServiceCreate(
            GetMasterDataOrderServiceCreateParameter parameter);

        CreateOrderServiceResult CreateOrderService(CreateOrderServiceParameter parameter);

        GetMasterDataPayOrderServiceResult
            GetMasterDataPayOrderService(GetMasterDataPayOrderServiceParameter parameter);

        GetListOrderByLocalPointResult GetListOrderByLocalPoint(GetListOrderByLocalPointParameter parameter);
        PayOrderByLocalPointResult PayOrderByLocalPoint(PayOrderByLocalPointParameter parameter);
        CheckExistsCustomerByPhoneResult CheckExistsCustomerByPhone(CheckExistsCustomerByPhoneParameter parameter);
        RefreshLocalPointResult RefreshLocalPoint(RefreshLocalPointParameter parameter);
        GetLocalPointByLocalAddressResult GetLocalPointByLocalAddress(GetLocalPointByLocalAddressParameter parameter);
        GetDataSearchTopReVenueResult GetDataSearchTopReVenue(GetDataSearchTopReVenueParameter parameter);
        SearchTopReVenueResult SearchTopReVenue(SearchTopReVenueParameter parameter);
        GetDataSearchRevenueProductResult GetDataSearchRevenueProduct(GetDataSearchRevenueProductParameter parameter);
        SearchRevenueProductResult SearchRevenueProduct(SearchRevenueProductParameter parameter);
        GetListOrderDetailByOrderResult GetListOrderDetailByOrder(GetListOrderDetailByOrderParameter parameter);
        GetListProductWasOrderResult GetListProductWasOrder(GetListProductWasOrderParameter parameter);
        UpdateCustomerServiceResult UpdateCustomerService(UpdateCustomerServiceParameter parameter);
        GetDataProfitByCustomerResult GetDataProfitByCustomer(GetDataProfitByCustomerParameter parameter);
        SearchProfitCustomerResult SearchProfitCustomer(SearchProfitCustomerParameter parameter);
    }
}
