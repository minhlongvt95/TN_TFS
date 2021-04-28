using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Order;
using TN.TNM.BusinessLogic.Messages.Requests.Order;
using TN.TNM.BusinessLogic.Messages.Responses.Order;

namespace TN.TNM.Api.Controllers
{
    public class CustomerOrderController : Controller
    {
        private readonly ICustomerOrder _iCustomerOrder;
        public CustomerOrderController (ICustomerOrder iCustomerOrder)
        {
            this._iCustomerOrder = iCustomerOrder;
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getAllOrder")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerOrderResponse GetAllCustomerOrder([FromBody]GetAllCustomerOrderRequest request)
        {
            return this._iCustomerOrder.GetAllCustomerOrder(request);
        }
        /// <summary>
        /// Create Customer Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/createCustomerOrder")]
        [Authorize(Policy = "Member")]
        public CreateCustomerOrderResponse CreateCustomerOrder([FromBody]CreateCustomerOrderRequest request)
        {
            return this._iCustomerOrder.CreateCustomerOrder(request);
        }
        /// <summary>
        /// Update Customer Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/updateCustomerOrder")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerOrderResponse UpdateCustomerOrder([FromBody]UpdateCustomerOrderRequest request)
        {
            return this._iCustomerOrder.UpdateCustomerOrder(request);
        }
        /// <summary>
        /// Get Customer Order By ID
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getCustomerOrderByID")]
        [Authorize(Policy = "Member")]
        public GetCustomerOrderByIDResponse GetCustomerOrderByID([FromBody]GetCustomerOrderByIDRequest request)
        {
            return this._iCustomerOrder.GetCustomerOrderByID(request);
        }
        /// <summary>
        /// Export Pdf Customer Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/exportPdfCustomerOrder")]
        [Authorize(Policy = "Member")]
        public ExportCustomerOrderPDFResponse ExportPdfCustomerOrder([FromBody]ExportCustomerOrderPDFRequest request)
        {
            return this._iCustomerOrder.ExportPdfCustomerOrder(request);
        }

        /// <summary>
        /// Get Customer Order By Seller
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getCustomerOrderBySeller")]
        [Authorize(Policy = "Member")]
        public GetCustomerOrderBySellerResponse GetCustomerOrderBySeller([FromBody]GetCustomerOrderBySellerRequest request)
        {
            return this._iCustomerOrder.GetCustomerOrderBySeller(request);
        }

        /// <summary>
        /// Get Employee List By OrganizationId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getEmployeeListByOrganizationId")]
        [Authorize(Policy = "Member")]
        public GetEmployeeListByOrganizationIdResponse GetEmployeeListByOrganizationId([FromBody]GetEmployeeListByOrganizationIdRequest request)
        {
            return this._iCustomerOrder.GetEmployeeListByOrganizationId(request);
        }

        /// <summary>
        /// Get Months List of revenue between months
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getMonthsList")]
        [Authorize(Policy = "Member")]
        public GetMonthsListResponse GetMonthsList([FromBody]GetMonthsListRequest request)
        {
            return this._iCustomerOrder.GetMonthsList(request);
        }

        /// <summary>
        /// Get Product Category Group By Level
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getProductCategoryGroupByLevel")]
        [Authorize(Policy = "Member")]
        public GetProductCategoryGroupByLevelResponse GetProductCategoryGroupByLevel([FromBody]GetProductCategoryGroupByLevelRequest request)
        {
            return this._iCustomerOrder.GetProductCategoryGroupByLevel(request);
        }

        /// <summary>
        /// Get Product Category Group By Manager
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/getProductCategoryGroupByManager")]
        [Authorize(Policy = "Member")]
        public GetProductCategoryGroupByManagerResponse GetProductCategoryGroupByManager([FromBody]GetProductCategoryGroupByManagerRequest request)
        {
            return this._iCustomerOrder.GetProductCategoryGroupByManager(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderSearch")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderSearchResponse GetMasterDataOrderSearch([FromBody]GetMasterDataOrderSearchRequest request)
        {
            return this._iCustomerOrder.GetMasterDataOrderSearch(request);
        }

        //
        [HttpPost]
        [Route("api/order/searchOrder")]
        [Authorize(Policy = "Member")]
        public SearchOrderResponse SearchOrder([FromBody]SearchOrderRequest request)
        {
            return this._iCustomerOrder.SearchOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderCreateResponse GetMasterDataOrderCreate([FromBody]GetMasterDataOrderCreateRequest request)
        {
            return this._iCustomerOrder.GetMasterDataOrderCreate(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderDetailDialog")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderDetailDialogResponse GetMasterDataOrderDetailDialog([FromBody]GetMasterDataOrderDetailDialogRequest request)
        {
            return this._iCustomerOrder.GetMasterDataOrderDetailDialog(request);
        }

        //
        [HttpPost]
        [Route("api/order/getVendorByProductId")]
        [Authorize(Policy = "Member")]
        public GetVendorByProductIdResponse GetVendorByProductId([FromBody]GetVendorByProductIdRequest request)
        {
            return this._iCustomerOrder.GetVendorByProductId(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderDetailResponse GetMasterDataOrderDetail([FromBody]GetMasterDataOrderDetailRequest request)
        {
            return this._iCustomerOrder.GetMasterDataOrderDetail(request);
        }

        //
        [HttpPost]
        [Route("api/order/deleteOrder")]
        [Authorize(Policy = "Member")]
        public DeleteOrderResponse DeleteOrder([FromBody]DeleteOrderRequest request)
        {
            return this._iCustomerOrder.DeleteOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/getDataDashboardHome")]
        [Authorize(Policy = "Member")]
        public GetDataDashboardHomeResponse GetDataDashboardHome([FromBody]GetDataDashboardHomeRequest request)
        {
            return this._iCustomerOrder.GetDataDashboardHome(request);
        }

        //
        [HttpPost]
        [Route("api/order/checkReceiptOrderHistory")]
        [Authorize(Policy = "Member")]
        public CheckReceiptOrderHistoryResponse CheckReceiptOrderHistory([FromBody]CheckReceiptOrderHistoryRequest request)
        {
            return this._iCustomerOrder.CheckReceiptOrderHistory(request);
        }

        //
        [HttpPost]
        [Route("api/order/checkBeforCreateOrUpdateOrder")]
        [Authorize(Policy = "Member")]
        public CheckBeforCreateOrUpdateOrderResponse CheckBeforCreateOrUpdateOrder([FromBody]CheckBeforCreateOrUpdateOrderRequest request)
        {
            return this._iCustomerOrder.CheckBeforCreateOrUpdateOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/updateStatusOrder")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerOrderResponse UpdateStatusOrder([FromBody]UpdateStatusOrderRequest request)
        {
            return this._iCustomerOrder.UpdateStatusOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/searchProfitAccordingCustomers")]
        [Authorize(Policy = "Member")]
        public ProfitAccordingCustomersResponse SearchProfitAccordingCustomers([FromBody]ProfitAccordingCustomersRequest request)
        {
            return this._iCustomerOrder.SearchProfitAccordingCustomers(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataOrderServiceCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterDataOrderServiceCreateResponse GetMasterDataOrderServiceCreate([FromBody]GetMasterDataOrderServiceCreateRequest request)
        {
            return this._iCustomerOrder.GetMasterDataOrderServiceCreate(request);
        }

        //
        [HttpPost]
        [Route("api/order/createOrderService")]
        [Authorize(Policy = "Member")]
        public CreateOrderServiceResponse CreateOrderService([FromBody]CreateOrderServiceRequest request)
        {
            return this._iCustomerOrder.CreateOrderService(request);
        }

        //
        [HttpPost]
        [Route("api/order/getMasterDataPayOrderService")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPayOrderServiceResponse GetMasterDataPayOrderService([FromBody]GetMasterDataPayOrderServiceRequest request)
        {
            return this._iCustomerOrder.GetMasterDataPayOrderService(request);
        }

        //
        [HttpPost]
        [Route("api/order/getListOrderByLocalPoint")]
        [Authorize(Policy = "Member")]
        public GetListOrderByLocalPointResponse GetListOrderByLocalPoint([FromBody]GetListOrderByLocalPointRequest request)
        {
            return this._iCustomerOrder.GetListOrderByLocalPoint(request);
        }

        //
        [HttpPost]
        [Route("api/order/payOrderByLocalPoint")]
        [Authorize(Policy = "Member")]
        public PayOrderByLocalPointResponse PayOrderByLocalPoint([FromBody]PayOrderByLocalPointRequest request)
        {
            return this._iCustomerOrder.PayOrderByLocalPoint(request);
        }

        //
        [HttpPost]
        [Route("api/order/checkExistsCustomerByPhone")]
        [Authorize(Policy = "Member")]
        public CheckExistsCustomerByPhoneResponse CheckExistsCustomerByPhone([FromBody]CheckExistsCustomerByPhoneRequest request)
        {
            return this._iCustomerOrder.CheckExistsCustomerByPhone(request);
        }

        //
        [HttpPost]
        [Route("api/order/refreshLocalPoint")]
        [Authorize(Policy = "Member")]
        public RefreshLocalPointResponse RefreshLocalPoint([FromBody]RefreshLocalPointRequest request)
        {
            return this._iCustomerOrder.RefreshLocalPoint(request);
        }

        //
        [HttpPost]
        [Route("api/order/getLocalPointByLocalAddress")]
        [Authorize(Policy = "Member")]
        public GetLocalPointByLocalAddressResponse GetLocalPointByLocalAddress([FromBody]GetLocalPointByLocalAddressRequest request)
        {
            return this._iCustomerOrder.GetLocalPointByLocalAddress(request);
        }

        [HttpPost]
        [Route("api/order/getDataSearchTopReVenue")]
        [Authorize(Policy = "Member")]
        public GetDataSearchTopReVenueResponse GetDataSearchTopReVenue([FromBody]GetDataSearchTopReVenueRequest request)
        {
            return this._iCustomerOrder.GetDataSearchTopReVenue(request);
        }

        [HttpPost]
        [Route("api/order/searchTopReVenue")]
        [Authorize(Policy = "Member")]
        public SearchTopReVenueResponse SearchTopReVenue([FromBody]SearchTopReVenueRequest request)
        {
            return this._iCustomerOrder.SearchTopReVenue(request);
        }

        [HttpPost]
        [Route("api/order/getDataSearchRevenueProduct")]
        [Authorize(Policy = "Member")]
        public GetDataSearchRevenueProductResponse GetDataSearchRevenueProduct([FromBody]GetDataSearchRevenueProductRequest request)
        {
            return this._iCustomerOrder.GetDataSearchRevenueProduct(request);
        }

        [HttpPost]
        [Route("api/order/searchRevenueProduct")]
        [Authorize(Policy = "Member")]
        public SearchRevenueProductResponse SearchRevenueProduct([FromBody]SearchRevenueProductRequest request)
        {
            return this._iCustomerOrder.SearchRevenueProduct(request);
        }

        //
        [HttpPost]
        [Route("api/order/getListOrderDetailByOrder")]
        [Authorize(Policy = "Member")]
        public GetListOrderDetailByOrderResponse GetListOrderDetailByOrder([FromBody]GetListOrderDetailByOrderRequest request)
        {
            return this._iCustomerOrder.GetListOrderDetailByOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/getListProductWasOrder")]
        [Authorize(Policy = "Member")]
        public GetListProductWasOrderResponse GetListProductWasOrder([FromBody]GetListProductWasOrderRequest request)
        {
            return this._iCustomerOrder.GetListProductWasOrder(request);
        }

        //
        [HttpPost]
        [Route("api/order/updateCustomerService")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerServiceResponse UpdateCustomerService([FromBody]UpdateCustomerServiceRequest request)
        {
            return this._iCustomerOrder.UpdateCustomerService(request);
        }

        [HttpPost]
        [Route("api/order/getDataProfitByCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataProfitByCustomerResponse GetDataProfitByCustomer([FromBody]GetDataProfitByCustomerRequest request)
        {
            return this._iCustomerOrder.GetDataProfitByCustomer(request);
        }

        [HttpPost]
        [Route("api/order/searchProfitCustomer")]
        [Authorize(Policy = "Member")]
        public SearchProfitCustomerResponse SearchProfitCustomer([FromBody]SearchProfitCustomerRequest request)
        {
            return this._iCustomerOrder.SearchProfitCustomer(request);
        }
    }
}