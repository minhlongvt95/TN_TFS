using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.SaleBidding;
using TN.TNM.BusinessLogic.Messages.Requests.SaleBidding;
using TN.TNM.BusinessLogic.Messages.Responses.SaleBidding;

namespace TN.TNM.Api.Controllers
{
    public class SaleBiddingController : Controller
    {
        private readonly ISaleBidding _iSaleBidding;
        public SaleBiddingController(ISaleBidding iSaleBidding)
        {
            this._iSaleBidding = iSaleBidding;
        }

        /// <summary>
        ///  Get Master Data Create SaleBidding
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/saleBidding/createSaleBidding")]
        [Authorize(Policy = "Member")]
        public CreateSaleBiddingResponse CreateSaleBidding([FromForm]CreateSaleBiddingRequest request)
        {
            return this._iSaleBidding.CreateSaleBidding(request);
        }

        /// <summary>
        ///  Get Master Data Create SaleBidding
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/saleBidding/getMasterDataCreateSaleBidding")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateSaleBiddingResponse GetMasterDataCreateSaleBidding([FromBody]GetMasterDataCreateSaleBiddingRequest request)
        {
            return this._iSaleBidding.GetMasterDataCreateSaleBidding(request);
        }

        [HttpPost]
        [Route("api/saleBidding/getMasterDateSaleBiddingDashboard")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSaleBiddingDashboardResponse GetMasterDateSaleBiddingDashboard([FromBody]GetMasterDataSaleBiddingDashboardRequest request)
        {
            return this._iSaleBidding.GetMasterDataSaleBiddingDashboard(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/saleBidding/getMasterDataSearchSaleBidding")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchSaleBiddingResponse GetMasterDataSearchSaleBidding([FromBody]GetMasterDataSearchSaleBiddingRequest request)
        {
            return this._iSaleBidding.GetMasterDataSearchSaleBidding(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/saleBidding/searchSaleBidding")]
        [Authorize(Policy = "Member")]
        public SearchSaleBiddingResponse SearchSaleBidding([FromBody]SearchSaleBiddingRequest request)
        {
            return this._iSaleBidding.SearchBidding(request);
        }

        /// <summary>
        /// Get Master Data SaleBidding Add or Edit Product Dialog
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/saleBidding/getMasterDataSaleBiddingAddEditProductDialog")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSaleBiddingAddEditProductDialogResponse GetMasterDataSaleBiddingAddEditProductDialog
                                                                    ([FromBody]GetMasterDataSaleBiddingAddEditProductDialogRequest request)
        {
            return this._iSaleBidding.GetMasterDataSaleBiddingAddEditProductDialog(request);
        }

        /// <summary>
        /// Get Vendor By ProductId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/saleBidding/getVendorByProductId")]
        [Authorize(Policy = "Member")]
        public GetVendorByProductIdReponse GetVendorByProductId([FromBody]GetVendorByProductIdRequest request)
        {
            return this._iSaleBidding.GetVendorByProductId(request);
        }

        /// <summary>
        /// Get Vendor By ProductId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/saleBidding/downloadTemplateProduct")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateProductResponse DownloadTemplateProduct([FromBody]DownloadTemplateProductRequest request)
        {
            return this._iSaleBidding.DownloadTemplateProduct(request);
        }

        [HttpPost]
        [Route("api/saleBidding/getMasterDataSaleBiddingDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSaleBiddingDetailResponse GetMasterDataSaleBiddingDetail([FromBody]GetMasterDataSaleBiddingDetailRequest request)
        {
            return this._iSaleBidding.GetMasterDataSaleBiddingDetail(request);
        }

        [HttpPost]
        [Route("api/saleBidding/editSaleBidding")]
        [Authorize(Policy = "Member")]
        public EditSaleBiddingResponse EditSaleBidding([FromForm]EditSaleBiddingRequest request)
        {
            return this._iSaleBidding.EditSaleBidding(request);
        }

        [HttpPost]
        [Route("api/saleBidding/updateStatusSaleBidding")]
        [Authorize(Policy = "Member")]
        public UpdateStatusSaleBiddingResponse UpdateStatusSaleBidding([FromBody]UpdateStatusSaleBiddingRequest request)
        {
            return this._iSaleBidding.UpdateStatusSaleBidding(request);
        }

        [HttpPost]
        [Route("api/saleBidding/getMasterDataSaleBiddingApproved")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSaleBiddingApprovedResponse GetMasterDataSaleBiddingApproved([FromBody]GetMasterDataSaleBiddingApprovedRequest request)
        {
            return this._iSaleBidding.GetMasterDataSaleBiddingApproved(request);
        }

        [HttpPost]
        [Route("api/saleBidding/searchSaleBiddingApproved")]
        [Authorize(Policy = "Member")]
        public SearchSaleBiddingApprovedResponse SearchSaleBiddingApproved([FromBody]SearchSaleBiddingApprovedRequest request)
        {
            return this._iSaleBidding.SearchSaleBiddingApproved(request);
        }

        [HttpPost]
        [Route("api/saleBidding/getVendorMapping")]
        [Authorize(Policy = "Member")]
        public GetVendorMappingResponse GetVendorMapping([FromBody]GetVendorMappingRequest request)
        {
            return _iSaleBidding.GetVendorMapping(request);
        }

        [HttpPost]
        [Route("api/saleBidding/getCustomerByEmployeeId")]
        [Authorize(Policy = "Member")]
        public GetCustomerByEmployeeIdResponse GetCustomerByEmployeeId([FromBody]GetCustomerByEmployeeIdRequest request)
        {
            return _iSaleBidding.GetCustomerByEmployeeId(request);
        }

        [HttpPost]
        [Route("api/saleBidding/sendEmailEmployee")]
        [Authorize(Policy = "Member")]
        public SendEmailEmployeeResponse SendEmailEmployee([FromBody]SendEmailEmployeeRequest request)
        {
            return _iSaleBidding.SendEmailEmployee(request);
        }

        [HttpPost]
        [Route("api/saleBidding/GetPersonInChargeByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetPersonInChargeByCustomerIdResponse GetPersonInChargeByCustomerId([FromBody]GetPersonInChargeByCustomerIdRequest request)
        {
            return _iSaleBidding.GetPersonInChargeByCustomerId(request);
        }
    }
}