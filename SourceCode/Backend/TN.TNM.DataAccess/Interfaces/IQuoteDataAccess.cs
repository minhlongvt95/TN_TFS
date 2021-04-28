using TN.TNM.DataAccess.Messages.Parameters.Quote;
using TN.TNM.DataAccess.Messages.Results.Quote;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IQuoteDataAccess
    {
        GetAllQuoteResult GetAllQuote(GetAllQuoteParameter parameter);
        GetTop3QuotesOverdueResult GetTop3QuotesOverdue(GetTop3QuotesOverdueParameter parameter);
        GetTop3WeekQuotesOverdueResult GetTop3WeekQuotesOverdue(GetTop3WeekQuotesOverdueParameter parameter);
        GetTop3PotentialCustomersResult GetTop3PotentialCustomers(GetTop3PotentialCustomersParameter parameter);
        CreateQuoteResult CreateQuote(CreateQuoteParameter parameter);
        UpdateQuoteResult UpdateQuote(UpdateQuoteParameter parameter);
        GetQuoteByIDResult GetQuoteByID(GetQuoteByIDParameter parameter);
        ExportPdfQuotePDFResult ExportPdfQuote(ExportPdfQuoteParameter parameter);
        GetTotalAmountQuoteResult GetTotalAmountQuote(GetTotalAmountQuoteParameter parameter);
        GetDashBoardQuoteResult GetDashBoardQuote(GetDashBoardQuoteParameter parameter);
        UpdateActiveQuoteResult UpdateActiveQuote(UpdateActiveQuoteParameter parameter);
        GetDataQuoteToPieChartResult GetDataQuoteToPieChart(GetDataQuoteToPieChartParameter parameter);
        SearchQuoteResult SearchQuote(SearchQuoteParameter parameter);
        GetDataCreateUpdateQuoteResult GetDataCreateUpdateQuote(GetDataCreateUpdateQuoteParameter parameter);

        GetDataQuoteAddEditProductDialogResult GetDataQuoteAddEditProductDialog(
            GetDataQuoteAddEditProductDialogParameter parameter);

        GetVendorByProductIdResult GetVendorByProductId(GetVendorByProductIdParameter parameter);

        GetDataExportExcelQuoteResult GetDataExportExcelQuote(GetDataExportExcelQuoteParameter parameter);
        GetEmployeeSaleResult GetEmployeeSale(GetEmployeeSaleParameter parameter);
        DownloadTemplateProductResult DownloadTemplateProduct(DownloadTemplateProductParameter parameter);
        CreateCostResult CreateCost(CreateCostParameter parameter);
        GetMasterDataCreateCostResult GetMasterDataCreateCost(GetMasterDataCreateCostParameter parameter);
        UpdateCostResult UpdateCost(UpdateCostParameter parameter);
        UpdateQuoteResult UpdateStatusQuote(GetQuoteByIDParameter parameter);
        SearchQuoteResult SearchQuoteAprroval(SearchQuoteParameter parameter);
        UpdateQuoteResult ApprovalOrRejectQuote(ApprovalOrRejectQuoteParameter parameter);
        UpdateQuoteResult SendEmailCustomerQuote(SendEmailCustomerQuoteParameter parameter);
        GetMasterDataCreateQuoteResult GetMasterDataCreateQuote(GetMasterDataCreateQuoteParameter parameter);
        GetEmployeeByPersonInChargeResult GetEmployeeByPersonInCharge(GetEmployeeByPersonInChargeParameter parameter);
        GetMasterDataUpdateQuoteResult GetMasterDataUpdateQuote(GetMasterDataUpdateQuoteParameter parameter);
    }
}
