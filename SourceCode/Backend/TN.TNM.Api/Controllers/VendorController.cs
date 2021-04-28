using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Vendor;
using TN.TNM.BusinessLogic.Messages.Requests.Vendor;
using TN.TNM.BusinessLogic.Messages.Responses.Vendor;

namespace TN.TNM.Api.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendor iVendor;
        public VendorController(IVendor _iVendor)
        {
            this.iVendor = _iVendor;
        }

        /// <summary>
        /// Create new Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vendor/createVendor")]
        [Authorize(Policy = "Member")]
        public CreateVendorResponse CreateVendor([FromBody]CreateVendorRequest request)
        {
            return iVendor.CreateVendor(request);
        }

        /// <summary>
        /// Search Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>List of Vendor</returns>
        [HttpPost]
        [Route("api/vendor/searchVendor")]
        [Authorize(Policy = "Member")]
        public SearchVendorResponse SearchVendor([FromBody]SearchVendorRequest request)
        {
            return iVendor.SearchVendor(request);
        }

        /// <summary>
        /// Get Vendor by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/getVendorById")]
        [Authorize(Policy = "Member")]
        public GetVendorByIdResponse GetVendorById([FromBody]GetVendorByIdRequest request)
        {
            return iVendor.GetVendorById(request);
        }

        /// <summary>
        /// Get Vendor by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/getAllVendorCode")]
        [Authorize(Policy = "Member")]
        public GetAllVendorCodeResponse GetAllVendorCode([FromBody]GetAllVendorCodeRequest request)
        {
            return iVendor.GetAllVendorCode(request);
        }

        /// <summary>
        /// Update Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/updateVendorById")]
        [Authorize(Policy = "Member")]
        public UpdateVendorByIdResponse UpdateVendorById([FromBody]UpdateVendorByIdRequest request)
        {
            return iVendor.UpdateVendorById(request);
        }

        /// <summary>
        /// Quick Create Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/quickCreateVendor")]
        [Authorize(Policy = "Member")]
        public QuickCreateVendorResponse QuickCreateVendor([FromBody]QuickCreateVendorRequest request)
        {
            return iVendor.QuickCreateVendor(request);
        }

        /// <summary>
        /// Create Vendor Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/createVendorOrder")]
        [Authorize(Policy = "Member")]
        public CreateVendorOrderResponse CreateVendorOrder([FromBody]CreateVendorOrderRequest request)
        {
            return iVendor.CreateVendorOrder(request);
        }

        /// <summary>
        /// Create Vendor Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/searchVendorOrder")]
        [Authorize(Policy = "Member")]
        public SearchVendorOrderResponse SearchVendorOrder([FromBody]SearchVendorOrderRequest request)
        {
            return iVendor.SearchVendorOrder(request);
        }

        /// <summary>
        /// GetAllVendor
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vendor/getAllVendor")]
        [Authorize(Policy = "Member")]
        public GetAllVendorResponse GetAllVendor([FromBody]GetAllVendorRequest request)
        {
            return iVendor.GetAllVendor(request);
        }

        /// <summary>
        /// Get vendor order by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vendor/getVendorOrderById")]
        [Authorize(Policy = "Member")]
        public GetVendorOrderByIdResponse GetVendorOrderById([FromBody]GetVendorOrderByIdRequest request)
        {
            return iVendor.GetVendorOrderById(request);
        }

        /// <summary>
        /// Update vendor order by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vendor/updateVendorOrderById")]
        [Authorize(Policy = "Member")]
        public UpdateVendorOrderByIdResponse UpdateVendorOrderById([FromBody]UpdateVendorOrderByIdRequest request)
        {
            return iVendor.UpdateVendorOrderById(request);
        }

        /// <summary>
        /// Update Active Vendor
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>Get Vendor</returns>
        [HttpPost]
        [Route("api/vendor/updateActiveVendor")]
        [Authorize(Policy = "Member")]
        public UpdateActiveVendorResponse UpdateActiveVendor([FromBody]UpdateActiveVendorRequest request)
        {
            return iVendor.UpdateActiveVendor(request);
        }

        /// <summary>
        /// QuickCreateVendorMasterdata
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns>QuickCreateVendorMasterdata</returns>
        [HttpPost]
        [Route("api/vendor/quickCreateVendorMasterdata")]
        [Authorize(Policy = "Member")]
        public QuickCreateVendorMasterdataResponse QuickCreateVendorMasterdata([FromBody]QuickCreateVendorMasterdataRequest request)
        {
            return iVendor.QuickCreateVendorMasterdata(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataCreateVendor")]
        [Authorize(Policy = "Member")]
        public GetDataCreateVendorResponse GetDataCreateVendor([FromBody]GetDataCreateVendorRequest request)
        {
            return iVendor.GetDataCreateVendor(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataSearchVendor")]
        [Authorize(Policy = "Member")]
        public GetDataSearchVendorResponse GetDataSearchVendor([FromBody]GetDataSearchVendorRequest request)
        {
            return iVendor.GetDataSearchVendor(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataEditVendor")]
        [Authorize(Policy = "Member")]
        public GetDataEditVendorResponse GetDataEditVendor([FromBody]GetDataEditVendorRequest request)
        {
            return iVendor.GetDataEditVendor(request);
        }

        [HttpPost]
        [Route("api/vendor/createVendorContact")]
        [Authorize(Policy = "Member")]
        public CreateVendorContactResponse CreateVendorContact([FromBody]CreateVendorContactRequest request)
        {
            return iVendor.CreateVendorContact(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataCreateVendorOrder")]
        [Authorize(Policy = "Member")]
        public GetDataCreateVendorOrderResponse GetDataCreateVendorOrder([FromBody]GetDataCreateVendorOrderRequest request)
        {
            return iVendor.GetDataCreateVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataAddVendorOrderDetail")]
        [Authorize(Policy = "Member")]
        public GetDataAddVendorOrderDetailResponse GetDataAddVendorOrderDetail([FromBody]GetDataAddVendorOrderDetailRequest request)
        {
            return iVendor.GetDataAddVendorOrderDetail(request);
        }

        [HttpPost]
        [Route("api/vendor/getMasterDataSearchVendorOrder")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchVendorOrderResponse GetMasterDataSearchVendorOrder([FromBody]GetMasterDataSearchVendorOrderRequest request)
        {
            return iVendor.GetMasterDataSearchVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataEditVendorOrder")]
        [Authorize(Policy = "Member")]
        public GetDataEditVendorOrderResponse GetDataEditVendorOrder([FromBody]GetDataEditVendorOrderRequest request)
        {
            return iVendor.GetDataEditVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/getDataSearchVendorQuote")]
        [Authorize(Policy = "Member")]
        public GetDataSearchVendorQuoteResponse GetDataSearchVendorQuote([FromBody]GetDataSearchVendorQuoteRequest request)
        {
            return iVendor.GetDataSearchVendorQuote(request);
        }

        [HttpPost]
        [Route("api/vendor/createVendorQuote")]
        [Authorize(Policy = "Member")]
        public CreateVendorQuoteResponse CreateVendorQuote([FromBody]ListVendorQuoteRequest request)
        {
            return iVendor.CreateVendorQuote(request);
        }

        [HttpPost]
        [Route("api/vendor/searchVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public SearchVendorProductPriceResponse SearchVendorProductPrice([FromBody]SearchVendorProductPriceRequest request)
        {
            return iVendor.SearchVendorProductPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/createVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public CreateVendorProductPriceResponse CreateVendorProductPrice([FromBody]CreateVendorProductPriceRequest request)
        {
            return iVendor.CreateVendorProductPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/deleteVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public DeleteProductVendorPriceResponse DeleteVendorProductPrice([FromBody]DeleteVendorProductPriceRequest request)
        {
            return iVendor.DeleteProductVendorPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/downloadTemplateVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateVendorProductPriceResponse DownloadTemplateVendorProductPrice([FromBody]DownloadTemplateVendorProductPriceRequest request)
        {
            return iVendor.DownloadTemplateVendorProductPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/importVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public ImportVendorProductPriceResponse ImportVendorProductPrice([FromBody]ImportVendorProductPriceRequest request)
        {
            return iVendor.ImportVendorProductPrice(request);
        }

        [HttpPost]
        [Route("api/vendor/getMasterDataCreateSuggestedSupplierQuote")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateSuggestedSupplierQuoteResponse GetMasterDataCreateSuggestedSupplierQuote([FromBody]GetMasterDataCreateSuggestedSupplierQuoteRequest request)
        {
            return iVendor.GetMasterDataCreateSuggestedSupplierQuote(request);
        }

        [HttpPost]
        [Route("api/vendor/createOrUpdateSuggestedSupplierQuote")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateSuggestedSupplierQuoteResponse CreateOrUpdateSuggestedSupplierQuote([FromBody]CreateOrUpdateSuggestedSupplierQuoteRequest request)
        {
            return iVendor.CreateOrUpdateSuggestedSupplierQuote(request);
        }

        [HttpPost]
        [Route("api/vendor/deleteSuggestedSupplierQuoteRequest")]
        [Authorize(Policy = "Member")]
        public DeleteSuggestedSupplierQuoteRequestResponse DeleteSuggestedSupplierQuoteRequest([FromBody]DeleteSuggestedSupplierQuoteRequestRequest request)
        {
            return iVendor.DeleteSuggestedSupplierQuoteRequest(request);
        }

        [HttpPost]
        [Route("api/vendor/changeStatusVendorQuote")]
        [Authorize(Policy = "Member")]
        public ChangeStatusVendorQuoteResponse ChangeStatusVendorQuote([FromBody]ChangeStatusVendorQuoteRequest request)
        {
            return iVendor.ChangeStatusVendorQuote(request);
        }
        //
        [HttpPost]
        [Route("api/vendor/getDataAddEditCostVendorOrder")]
        [Authorize(Policy = "Member")]
        public GetDataAddEditCostVendorOrderResponse GetDataAddEditCostVendorOrder([FromBody]GetDataAddEditCostVendorOrderRequest request)
        {
            return iVendor.GetDataAddEditCostVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/sendEmailVendorQuote")]
        [Authorize(Policy = "Member")]
        public SendEmailVendorQuoteResponse SendEmailVendorQuote([FromBody]SendEmailVendorQuoteRequest request)
        {
            return iVendor.SendEmailVendorQuote(request);
        }

        
        [HttpPost]
        [Route("api/vendor/removeVendorOrder")]
        [Authorize(Policy = "Member")]
        public RemoveVendorOrderResponse RemoveVendorOrder([FromBody]RemoveVendorOrderRequest request)
        {
            return iVendor.RemoveVendorOrder(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/cancelVendorOrder")]
        [Authorize(Policy = "Member")]
        public CancelVendorOrderResponse CancelVendorOrder([FromBody]CancelVendorOrderRequest request)
        {
            return iVendor.CancelVendorOrder(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/draftVendorOrder")]
        [Authorize(Policy = "Member")]
        public DraftVendorOrderResponse DraftVendorOrder([FromBody]DraftVendorOrderRequest request)
        {
            return iVendor.DraftVendorOrder(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/getMasterDataVendorOrderReport")]
        [Authorize(Policy = "Member")]
        public GetMasterDataVendorOrderReportResponse GetMasterDataVendorOrderReport([FromBody]GetMasterDataVendorOrderReportRequest request)
        {
            return iVendor.GetMasterDataVendorOrderReport(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/searchVendorOrderReport")]
        [Authorize(Policy = "Member")]
        public SearchVendorOrderReportResponse SearchVendorOrderReport([FromBody]SearchVendorOrderReportRequest request)
        {
            return iVendor.SearchVendorOrderReport(request);
        }

        //
        [HttpPost]
        [Route("api/vendor/approvalOrRejectVendorOrder")]
        [Authorize(Policy = "Member")]
        public ApprovalOrRejectVendorOrderResponse ApprovalOrRejectVendorOrder(
            [FromBody]ApprovalOrRejectVendorOrderRequest request)
        {
            return iVendor.ApprovalOrRejectVendorOrder(request);
        }

        [HttpPost]
        [Route("api/vendor/getQuantityApproval")]
        [Authorize(Policy = "Member")]
        public GetQuantityApprovalResponse GetQuantityApproval([FromBody] GetQuantityApprovalRequest request)
        {
            return iVendor.GetQuantityApproval(request);
        }
    }
}