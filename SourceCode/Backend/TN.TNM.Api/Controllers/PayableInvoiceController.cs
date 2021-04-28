using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.PayableInvoice;
using TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice;
using TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice;

namespace TN.TNM.Api.Controllers
{
    public class PayableInvoiceController
    {
        private readonly IPayableInvoice _iPayableInvoice;
        public PayableInvoiceController(IPayableInvoice iPayableInvoice)
        {
            this._iPayableInvoice = iPayableInvoice;
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/create")]
        [Authorize(Policy = "Member")]
        public CreatePayableInvoiceRespone CreatePayableInvoice([FromBody]CreatePayableInvoiceRequest request)
        {
            return this._iPayableInvoice.CreatePayableInvoice(request);
        }
        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/edit")]
        [Authorize(Policy = "Member")]
        public EditPayableInvoiceRespone EditPayableInvoice([FromBody]EditPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.EditPayableInvoice(request);
        }
        /// <summary>
        /// Get PayableInvoiceById
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/getPayableInvoiceById")]
        [Authorize(Policy = "Member")]
        public GetPayableInvoiceByIdResponse GetPayableInvoiceById([FromBody]GetPayableInvoiceByIdRequest request)
        {
            return this._iPayableInvoice.GetPayableInvoiceById(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/searchPayableInvoice")]
        [Authorize(Policy = "Member")]
        public SearchPayableInvoiceResponse SearchPayableInvoice([FromBody]SearchPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.SearchPayableInvoice(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/createBankPayableInvoice")]
        [Authorize(Policy = "Member")]
        public CreateBankPayableInvoiceResponse CreateBankPayableInvoice([FromBody]CreateBankPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.CreateBankPayableInvoice(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/searchBankPayableInvoice")]
        [Authorize(Policy = "Member")]
        public SearchBankPayableInvoiceResponse SearchBankPayableInvoice([FromBody]SearchBankPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.SearchBankPayableInvoice(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/getBankPayableInvoiceById")]
        [Authorize(Policy = "Member")]
        public GetBankPayableInvoiceByIdResponse GetBankPayableInvoiceById([FromBody]GetBankPayableInvoiceByIdRequest request)
        {
            return this._iPayableInvoice.GetBankPayableInvoiceById(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/exportBankPayableInvoice")]
        [Authorize(Policy = "Member")]
        public ExportBankPayableInvoiceResponse ExportBankPayableInvoice([FromBody]ExportBankPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.ExportBankPayableInvoice(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/exportPayableInvoice")]
        [Authorize(Policy = "Member")]
        public ExportPayableInvoiceResponse ExportPayableInvoice([FromBody]ExportPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.ExportPayableInvoice(request);
        }

        /// <summary>
        /// Search Bank Book Payable Invoice
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/searchBankBookPayableInvoice")]
        [Authorize(Policy = "Member")]
        public SearchBankBookPayableInvoiceResponse SearchBankBookPayableInvoice([FromBody]SearchBankBookPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.SearchBankBookPayableInvoice(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/searchCashBookPayableInvoice")]
        [Authorize(Policy = "Member")]
        public SearchCashBookPayableInvoiceResponse SearchCashBookPayableInvoice([FromBody]SearchCashBookPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.SearchCashBookPayableInvoice(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/getMasterDataPayableInvoice")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPayableInvoiceResponse GetMasterDataPayableInvoice ([FromBody]GetMasterDataPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.GetMasterDataPayableInvoice(request);
        }

        /// <summary>
        /// Get All Order
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/payableInvoice/getMasterDataPayableInvoiceSearch")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPayableInvoiceSearchResponse GetMasterDataPayableInvoiceSearch([FromBody]GetMasterDataPayableInvoiceSearchRequest request)
        {
            return this._iPayableInvoice.GetMasterDataPayableInvoiceSearch(request);
        }

       [HttpPost]
       [Route("api/payableInvoice/getMasterDataBankPayableInvoice")]
       [Authorize(Policy ="Member")]
       public GetMasterDataBankPayableInvoiceResponse GetMasterDataBankPayableInvoice([FromBody]GetMasterDataBankPayableInvoiceRequest request)
       {
            return this._iPayableInvoice.GetMasterDataBankPayableInvoice(request);
       }
        
       [HttpPost]
       [Route("api/payableInvoice/getMasterDataSearchBankPayableInvoice")]
       [Authorize(Policy ="Member")]
        public GetMasterDataBankSearchPayableInvoiceResponse GetMasterDataSearchBankPayableInvoice([FromBody]GetMasterDataSearchBankPayableInvoiceRequest request)
        {
            return this._iPayableInvoice.GetMasterDataBankSearchPayableInvoice(request);
        }
    }
}
