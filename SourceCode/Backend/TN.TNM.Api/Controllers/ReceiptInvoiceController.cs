using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.ReceiptInvoice;
using TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice;
using TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice;

namespace TN.TNM.Api.Controllers
{
    public class ReceiptInvoiceController
    {
        private readonly IReceiptInvoice _iReceiptInvoice;
        public ReceiptInvoiceController(IReceiptInvoice iReceiptInvoice)
        {
            this._iReceiptInvoice = iReceiptInvoice;
        }

        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/create")]
        [Authorize(Policy = "Member")]
        public CreateReceiptInvoiceResponse CreateReceiptInvoice([FromBody]CreateReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.CreateReceiptInvoice(request);
        }
        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/edit")]
        [Authorize(Policy = "Member")]
        public EditReceiptInvoiceResponse EditReceiptInvoice([FromBody]EditReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.EditReceiptInvoice(request);
        }
        /// <summary>
        /// GetReceiptInvoiceById
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/getReceiptInvoiceById")]
        [Authorize(Policy = "Member")]
        public GetReceiptInvoiceByIdResponse GetReceiptInvoiceById([FromBody]GetReceiptInvoiceByIdRequest request)
        {
            return this._iReceiptInvoice.GetReceiptInvoiceById(request);
        }

        /// <summary>
        /// Search Receipt Invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/createBankReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public CreateBankReceiptInvoiceResponse CreateBankReceiptInvoice([FromBody]CreateBankReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.CreateBankReceiptInvoice(request);
        }

        /// <summary>
        /// Search Receipt Invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/searchReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public SearchReceiptInvoiceResponse SearchReceiptInvoice([FromBody]SearchReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.SearchReceiptInvoice(request);
        }

        /// <summary>
        /// Search Bank Receipt Invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/searchBankReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public SearchBankReceiptInvoiceResponse SearchBankReceiptInvoice([FromBody]SearchBankReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.SearchBankReceiptInvoice(request);
        }

        /// <summary>
        /// Get Bank Receipt Invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/getBankReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public GetBankReceiptInvoiceByIdResponse GetBankReceiptInvoiceById([FromBody]GetBankReceiptInvoiceByIdRequest request)
        {
            return this._iReceiptInvoice.GetBankReceiptInvoiceById(request);
        }

        /// <summary>
        /// exportPdfReceiptInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/exportPdfReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public ExportReceiptinvoiceResponse ExportReceiptinvoice([FromBody]ExportReceiptinvoiceRequest request)
        {
            return this._iReceiptInvoice.ExportPdfReceiptInvoice(request);
        }

        /// <summary>
        /// exportPdfReceiptInvoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/exportBankReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public ExportBankReceiptInvoiceResponse ExportBankReceiptInvoice([FromBody]ExportBankReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.ExportBankReceiptInvoice(request);
        }

        /// <summary>
        /// Search Bank Book Receipt
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/searchBankBookReceipt")]
        [Authorize(Policy = "Member")]
        public SearchBankBookReceiptResponse SearchBankBookReceipt([FromBody]SearchBankBookReceiptRequest request)
        {
            return this._iReceiptInvoice.SearchBankBookReceipt(request);
        }

        /// <summary>
        /// Search Receipt Invoice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/searchCashBookReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public SearchCashBookReceiptInvoiceResponse SearchCashBookReceiptInvoice([FromBody]SearchCashBookReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.SearchCashBookReceiptInvoice(request);
        }

        //GetOrderByCustomerId
        /// <summary>
        /// Get Order By CustomerId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/getOrderByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetOrderByCustomerIdResponse GetOrderByCustomerId([FromBody]GetOrderByCustomerIdRequest request)
        {
            return this._iReceiptInvoice.GetOrderByCustomerId(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/getMasterDateSearchBankBookReceipt")]
        [Authorize(Policy = "Member")]
        public GetMaterDataSearchBankReceiptInvoiceResponse GetMaterDataSearchBankReceiptInvoice([FromBody]GetMasterDataSearchBankReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.GetMaterDataSearchBankReceiptInvoice(request);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/getMasterDataBookReceipt")]
        [Authorize(Policy = "Member")]
        public GetMasterDataReceiptInvoiceResponse GetMaterDataReceiptInvoice([FromBody]GetMasterDataReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.GetMasterDataReceiptInvoice(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/receiptInvoice/GetMasterDataSearchReceiptInvoice")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchReceiptInvoiceResponse GetMasterDataSearchReceiptInvoice([FromBody]GetMasterDataSearchReceiptInvoiceRequest request)
        {
            return this._iReceiptInvoice.GetGetMasterDataSearchReceiptInvoice(request);
        }
    }
}
