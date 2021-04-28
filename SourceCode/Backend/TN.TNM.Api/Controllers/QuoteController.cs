using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Quote;
using TN.TNM.BusinessLogic.Messages.Requests.Quote;
using TN.TNM.BusinessLogic.Messages.Responses.Quote;

namespace TN.TNM.Api.Controllers
{
    public class QuoteController : ControllerBase
    {
        private readonly IQuote _iQuote;
        public QuoteController(IQuote iQuote)
        {
            this._iQuote = iQuote;
        }
        /// <summary>
        /// Create Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/createQuote")]
        [Authorize(Policy = "Member")]
        public CreateQuoteResponse CreateQuote([FromBody]CreateQuoteRequest request)
        {
            return this._iQuote.CreateQuote(request);
        }
        /// <summary>
        /// Get All Quote 
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getAllQuote")]
        [Authorize(Policy = "Member")]
        public GetAllQuoteResponse GetAllQuote([FromBody]GetAllQuoteRequest request)
        {
            return this._iQuote.GetAllQuote(request);
        }

        /// <summary>
        /// Get Top3 Quotes Overdue 
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getTop3QuotesOverdue")]
        [Authorize(Policy = "Member")]
        public GetTop3QuotesOverdueResponse GetTop3QuotesOverdue([FromBody]GetTop3QuotesOverdueRequest request)
        {
            return this._iQuote.GetTop3QuotesOverdue(request);
        }

        /// <summary>
        /// Get Top3 Week Quotes Overdue 
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getTop3WeekQuotesOverdue")]
        [Authorize(Policy = "Member")]
        public GetTop3WeekQuotesOverdueResponse GetTop3WeekQuotesOverdue([FromBody]GetTop3WeekQuotesOverdueRequest request)
        {
            return this._iQuote.GetTop3WeekQuotesOverdue(request);
        }

        /// <summary>
        /// Get Top3 Week Quotes Overdue 
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getTop3PotentialCustomers")]
        [Authorize(Policy = "Member")]
        public GetTop3PotentialCustomersResponse GetTop3PotentialCustomers([FromBody]GetTop3PotentialCustomersRequest request)
        {
            return this._iQuote.GetTop3PotentialCustomers(request);
        }

        /// <summary>
        /// update Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/updateQuote")]
        [Authorize(Policy = "Member")]
        public UpdateQuoteResponse UpdateQuote([FromBody]UpdateQuoteRequest request)
        {
            return this._iQuote.UpdateQuote(request);
        }
        /// <summary>
        /// get Quote ByID
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getQuoteByID")]
        [Authorize(Policy = "Member")]
        public GetQuoteByIDResponse GetQuoteByID([FromBody]GetQuoteByIDRequest request)
        {
            return this._iQuote.GetQuoteByID(request);
        }

        /// <summary>
        /// Get Total Amount Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getTotalAmountQuote")]
        [Authorize(Policy = "Member")]
        public GetTotalAmountQuoteResponse GetTotalAmountQuote([FromBody]GetTotalAmountQuoteRequest request)
        {
            return this._iQuote.GetTotalAmountQuote(request);
        }
        /// <summary>
        /// Get DashBoard Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getDashBoardQuote")]
        [Authorize(Policy = "Member")]
        public GetDashBoardQuoteResponse GetDashBoardQuote([FromBody]GetDashBoardQuoteRequest request)
        {
            return this._iQuote.GetDashBoardQuote(request);
        }

        /// <summary>
        /// update active Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/updateActiveQuote")]
        [Authorize(Policy = "Member")]
        public UpdateActiveQuoteResponse UpdateActiveQuote([FromBody]UpdateActiveQuoteRequest request)
        {
            return this._iQuote.UpdateActiveQuote(request);
        }

        /// <summary>
        /// GetDataQuoteToPieChart
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getDataQuoteToPieChart")]
        [Authorize(Policy = "Member")]
        public GetDataQuoteToPieChartResponse GetDataQuoteToPieChart([FromBody]GetDataQuoteToPieChartRequest request)
        {
            return this._iQuote.GetDataQuoteToPieChart(request);
        }

        /// <summary>
        /// Search Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/searchQuote")]
        [Authorize(Policy = "Member")]
        public SearchQuoteResponse SearchQuote([FromBody]SearchQuoteRequest request)
        {
            return this._iQuote.SearchQuote(request);
        }


        /// <summary>
        /// Get Data Create/Update Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getDataCreateUpdateQuote")]
        [Authorize(Policy = "Member")]
        public GetDataCreateUpdateQuoteResponse GetDataCreateUpdateQuote([FromBody]GetDataCreateUpdateQuoteRequest request)
        {
            return this._iQuote.GetDataCreateUpdateQuote(request);
        }


        /// <summary>
        /// Get Data Quote Add Edit Product Dialog
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getDataQuoteAddEditProductDialog")]
        [Authorize(Policy = "Member")]
        public GetDataQuoteAddEditProductDialogResponse GetDataQuoteAddEditProductDialog([FromBody]GetDataQuoteAddEditProductDialogRequest request)
        {
            return this._iQuote.GetDataQuoteAddEditProductDialog(request);
        }

        //
        /// <summary>
        /// Get Vendor By ProductId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getVendorByProductId")]
        [Authorize(Policy = "Member")]
        public GetVendorByProductIdResponse GetVendorByProductId([FromBody]GetVendorByProductIdRequest request)
        {
            return this._iQuote.GetVendorByProductId(request);
        }

        [HttpPost]
        [Route("api/quote/getDataExportExcelQuouter")]
        [Authorize(Policy ="Member")]
        public GetDataExportExcelQuoteResponse GetDataExportExcelQuote([FromBody]GetDataExportExcelQuoteRequest request)
        {
            return this._iQuote.GetDataExportExcelQuote(request);
        }

        [HttpPost]
        [Route("api/quote/getEmployeeSale")]
        [Authorize(Policy = "Member")]
        public GetEmployeeSaleResponse GetEmployeeSale([FromBody]GetEmployeeSaleRequest request)
        {
            return this._iQuote.GetEmployeeSale(request);
        }

        [HttpPost]
        [Route("api/quote/downloadTemplateProduct")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateProductResponse DownloadTemplateProduct([FromBody]DownloadTemplateProductRequest request)
        {
             return this._iQuote.DownloadTemplateProduct(request);
        }

        [HttpPost]
        [Route("api/quote/createCost")]
        [Authorize(Policy = "Member")]
        public CreateCostResponse CreateCost([FromBody]CreateCostRequest request)
        {
            return _iQuote.CreateCost(request);
        }

        /// <summary>
        /// get master data create cost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/getMasterDataCreateCost")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateCostResponse GetMasterDataCreateCost([FromBody]GetMasterDataCreateCostRequest request)
        {
            return this._iQuote.GetMasterDataCreateCost(request);
        }

        /// <summary>
        /// UpdateCost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/updateCost")]
        [Authorize(Policy = "Member")]
        public UpdateCostResponse UpdateCost([FromBody]UpdateCostRequest request)
        {
            return this._iQuote.UpdateCost(request);
        }

        /// <summary>
        /// UpdateCost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/updateStatusQuote")]
        [Authorize(Policy = "Member")]
        public UpdateQuoteResponse UpdateStatusQuote([FromBody]GetQuoteByIDRequest request)
        {
            return this._iQuote.UpdateStatusQuote(request);
        }

        /// <summary>
        /// Search Quote
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/searchQuoteApproval")]
        [Authorize(Policy = "Member")]
        public SearchQuoteResponse SearchQuoteAprroval([FromBody]SearchQuoteRequest request)
        {
            return this._iQuote.SearchQuoteAprroval(request);
        }

        /// <summary>
        /// UpdateCost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/approvalOrRejectQuote")]
        [Authorize(Policy = "Member")]
        public UpdateQuoteResponse ApprovalOrRejectQuote([FromBody]ApprovalOrRejectQuoteRequest request)
        {
            return this._iQuote.ApprovalOrRejectQuote(request);
        }

        /// <summary>
        /// UpdateCost
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/quote/sendEmailCustomerQuote")]
        [Authorize(Policy = "Member")]
        public UpdateQuoteResponse SendEmailCustomerQuote([FromBody]SendEmailCustomerQuoteRequest request)
        {
            return this._iQuote.SendEmailCustomerQuote(request);
        }

        //
        [HttpPost]
        [Route("api/quote/getMasterDataCreateQuote")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateQuoteResponse GetMasterDataCreateQuote([FromBody]GetMasterDataCreateQuoteRequest request)
        {
            return this._iQuote.GetMasterDataCreateQuote(request);
        }

        //
        [HttpPost]
        [Route("api/quote/getEmployeeByPersonInCharge")]
        [Authorize(Policy = "Member")]
        public GetEmployeeByPersonInChargeResponse GetEmployeeByPersonInCharge([FromBody]GetEmployeeByPersonInChargeRequest request)
        {
            return this._iQuote.GetEmployeeByPersonInCharge(request);
        }

        //
        [HttpPost]
        [Route("api/quote/getMasterDataUpdateQuote")]
        [Authorize(Policy = "Member")]
        public GetMasterDataUpdateQuoteResponse GetMasterDataUpdateQuote([FromBody]GetMasterDataUpdateQuoteRequest request)
        {
            return this._iQuote.GetMasterDataUpdateQuote(request);
        }
    }
}