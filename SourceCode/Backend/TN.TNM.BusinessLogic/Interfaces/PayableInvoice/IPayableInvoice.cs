using TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice;
using TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice;

namespace TN.TNM.BusinessLogic.Interfaces.PayableInvoice
{
    public interface IPayableInvoice
    {
        CreatePayableInvoiceRespone CreatePayableInvoice(CreatePayableInvoiceRequest request);
        EditPayableInvoiceRespone EditPayableInvoice(EditPayableInvoiceRequest request);
        GetPayableInvoiceByIdResponse GetPayableInvoiceById(GetPayableInvoiceByIdRequest request);
        SearchPayableInvoiceResponse SearchPayableInvoice(SearchPayableInvoiceRequest request);
        SearchCashBookPayableInvoiceResponse SearchCashBookPayableInvoice(SearchCashBookPayableInvoiceRequest request);
        CreateBankPayableInvoiceResponse CreateBankPayableInvoice(CreateBankPayableInvoiceRequest request);
        SearchBankPayableInvoiceResponse SearchBankPayableInvoice(SearchBankPayableInvoiceRequest request);
        GetBankPayableInvoiceByIdResponse GetBankPayableInvoiceById(GetBankPayableInvoiceByIdRequest request);
        ExportBankPayableInvoiceResponse ExportBankPayableInvoice(ExportBankPayableInvoiceRequest request);
        ExportPayableInvoiceResponse ExportPayableInvoice(ExportPayableInvoiceRequest request);
        SearchBankBookPayableInvoiceResponse SearchBankBookPayableInvoice(SearchBankBookPayableInvoiceRequest request);
        GetMasterDataPayableInvoiceResponse GetMasterDataPayableInvoice(GetMasterDataPayableInvoiceRequest request);
        GetMasterDataBankPayableInvoiceResponse GetMasterDataBankPayableInvoice(GetMasterDataBankPayableInvoiceRequest request);
        GetMasterDataPayableInvoiceSearchResponse GetMasterDataPayableInvoiceSearch(GetMasterDataPayableInvoiceSearchRequest request);
        GetMasterDataBankSearchPayableInvoiceResponse GetMasterDataBankSearchPayableInvoice(GetMasterDataSearchBankPayableInvoiceRequest request);


    }
}
