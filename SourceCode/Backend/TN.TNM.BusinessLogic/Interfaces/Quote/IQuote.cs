using TN.TNM.BusinessLogic.Messages.Requests.Quote;
using TN.TNM.BusinessLogic.Messages.Responses.Quote;

namespace TN.TNM.BusinessLogic.Interfaces.Quote
{
    public interface IQuote
    {
        GetAllQuoteResponse GetAllQuote(GetAllQuoteRequest request);
        GetTop3QuotesOverdueResponse GetTop3QuotesOverdue(GetTop3QuotesOverdueRequest request);
        GetTop3WeekQuotesOverdueResponse GetTop3WeekQuotesOverdue(GetTop3WeekQuotesOverdueRequest request);
        GetTop3PotentialCustomersResponse GetTop3PotentialCustomers(GetTop3PotentialCustomersRequest request);
        CreateQuoteResponse CreateQuote(CreateQuoteRequest request);
        UpdateQuoteResponse UpdateQuote(UpdateQuoteRequest request);
        GetQuoteByIDResponse GetQuoteByID(GetQuoteByIDRequest request);
        ExportPdfQuotePDFResponse ExportPdfQuote(ExportPdfQuoteRequest request);
        GetTotalAmountQuoteResponse GetTotalAmountQuote(GetTotalAmountQuoteRequest request);
        GetDashBoardQuoteResponse GetDashBoardQuote(GetDashBoardQuoteRequest request);
        UpdateActiveQuoteResponse UpdateActiveQuote(UpdateActiveQuoteRequest request);
        GetDataQuoteToPieChartResponse GetDataQuoteToPieChart(GetDataQuoteToPieChartRequest request);
        SearchQuoteResponse SearchQuote(SearchQuoteRequest request);
        GetDataCreateUpdateQuoteResponse GetDataCreateUpdateQuote(GetDataCreateUpdateQuoteRequest request);

        GetDataQuoteAddEditProductDialogResponse GetDataQuoteAddEditProductDialog(
            GetDataQuoteAddEditProductDialogRequest request);

        GetVendorByProductIdResponse GetVendorByProductId(GetVendorByProductIdRequest request);

        GetDataExportExcelQuoteResponse GetDataExportExcelQuote(GetDataExportExcelQuoteRequest request);
        GetEmployeeSaleResponse GetEmployeeSale(GetEmployeeSaleRequest request);
        DownloadTemplateProductResponse DownloadTemplateProduct(DownloadTemplateProductRequest request);
        CreateCostResponse CreateCost(CreateCostRequest request);
        GetMasterDataCreateCostResponse GetMasterDataCreateCost(GetMasterDataCreateCostRequest request);
        UpdateCostResponse UpdateCost(UpdateCostRequest request);
        UpdateQuoteResponse UpdateStatusQuote(GetQuoteByIDRequest request);
        SearchQuoteResponse SearchQuoteAprroval(SearchQuoteRequest request);
        UpdateQuoteResponse ApprovalOrRejectQuote(ApprovalOrRejectQuoteRequest request);
        UpdateQuoteResponse SendEmailCustomerQuote(SendEmailCustomerQuoteRequest request);
        GetMasterDataCreateQuoteResponse GetMasterDataCreateQuote(GetMasterDataCreateQuoteRequest request);
        GetEmployeeByPersonInChargeResponse GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeRequest request);
        GetMasterDataUpdateQuoteResponse GetMasterDataUpdateQuote(GetMasterDataUpdateQuoteRequest request);
    }
}
