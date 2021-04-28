using TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice;
using TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Interfaces.ReceiptInvoice
{
    public interface IReceiptInvoice
    {
        CreateReceiptInvoiceResponse CreateReceiptInvoice(CreateReceiptInvoiceRequest request);
        EditReceiptInvoiceResponse EditReceiptInvoice(EditReceiptInvoiceRequest request);
        GetReceiptInvoiceByIdResponse GetReceiptInvoiceById(GetReceiptInvoiceByIdRequest request);
        SearchReceiptInvoiceResponse SearchReceiptInvoice(SearchReceiptInvoiceRequest request);
        SearchCashBookReceiptInvoiceResponse SearchCashBookReceiptInvoice(SearchCashBookReceiptInvoiceRequest request);
        CreateBankReceiptInvoiceResponse CreateBankReceiptInvoice(CreateBankReceiptInvoiceRequest request);
        SearchBankReceiptInvoiceResponse SearchBankReceiptInvoice(SearchBankReceiptInvoiceRequest request);
        GetBankReceiptInvoiceByIdResponse GetBankReceiptInvoiceById(GetBankReceiptInvoiceByIdRequest request);
        ExportReceiptinvoiceResponse ExportPdfReceiptInvoice(ExportReceiptinvoiceRequest request);
        ExportBankReceiptInvoiceResponse ExportBankReceiptInvoice(ExportBankReceiptInvoiceRequest request);
        SearchBankBookReceiptResponse SearchBankBookReceipt(SearchBankBookReceiptRequest request);
        GetOrderByCustomerIdResponse GetOrderByCustomerId(GetOrderByCustomerIdRequest request);
        GetMaterDataSearchBankReceiptInvoiceResponse GetMaterDataSearchBankReceiptInvoice(GetMasterDataSearchBankReceiptInvoiceRequest request);
        GetMasterDataReceiptInvoiceResponse GetMasterDataReceiptInvoice(GetMasterDataReceiptInvoiceRequest request);
        GetMasterDataSearchReceiptInvoiceResponse GetGetMasterDataSearchReceiptInvoice(GetMasterDataSearchReceiptInvoiceRequest request);
    } 
}
