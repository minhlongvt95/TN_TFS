using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;
using TN.TNM.DataAccess.Messages.Results.ReceiptInvoice;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IReceiptInvoiceDataAccess
    {
        CreateReceiptInvoiceResult CreateReceiptInvoice(CreateReceiptInvoiceParameter parameter);
        EditReceiptInvoiceResult EditReceiptInvoice(EditReceiptInvoiceParameter parameter);
        GetReceiptInvoiceByIdResult GetReceiptInvoiceById(GetReceiptInvoiceByIdParameter parameter);
        SearchReceiptInvoiceResult SearchReceiptInvoice(SearchReceiptInvoiceParameter parameter);
        CreateBankReceiptInvoiceResult CreateBankReceiptInvoice(CreateBankReceiptInvoiceParameter parameter);
        SearchBankReceiptInvoiceResult SearchBankReceiptInvoice(SearchBankReceiptInvoiceParameter parameter);
        GetBankReceiptInvoiceByIdResult GetBankReceiptInvoiceById(GetBankReceiptInvoiceByIdParameter parameter);
        ExportReceiptinvoiceResult ExportPdfReceiptInvoice(ExportReceiptInvoiceParameter parameter);
        ExportBankReceiptInvoiceResult ExportBankReceiptInvoice(ExportBankReceiptInvoiceParameter parameter);
        SearchBankBookReceiptResult SearchBankBookReceipt(SearchBankBookReceiptParameter parameter);
        SearchCashBookReceiptInvoiceResult SearchCashBookReceiptInvoice(SearchCashBookReceiptInvoiceParameter parameter);
        GetOrderByCustomerIdResult GetOrderByCustomerId(GetOrderByCustomerIdParameter parameter);
        GetMasterDateSearchBankReceiptInvoiceResult GetMasterDateSearchBankReceiptInvoice(GetMasterDateSearchBankReceiptInvoiceParameter parameter);
        GetMasterDataReceiptInvoiceResult GetMasterDataReceiptInvoice(GetMasterDataReceiptInvoiceParameter parameter);
        GetMasterDataSearchReceiptInvoiceResult GetGetMasterDataSearchReceiptInvoice(GetMasterDataSearchReceiptInvoiceParameter parameter);
    }
}
