using TN.TNM.BusinessLogic.Messages.Requests.Vendor;
using TN.TNM.BusinessLogic.Messages.Responses.Vendor;

namespace TN.TNM.BusinessLogic.Interfaces.Vendor
{
    public interface IVendor
    {
        CreateVendorResponse CreateVendor(CreateVendorRequest request);
        SearchVendorResponse SearchVendor(SearchVendorRequest request);
        GetVendorByIdResponse GetVendorById(GetVendorByIdRequest request);
        GetAllVendorCodeResponse GetAllVendorCode(GetAllVendorCodeRequest request);
        UpdateVendorByIdResponse UpdateVendorById(UpdateVendorByIdRequest request);
        QuickCreateVendorResponse QuickCreateVendor(QuickCreateVendorRequest request);
        CreateVendorOrderResponse CreateVendorOrder(CreateVendorOrderRequest request);
        SearchVendorOrderResponse SearchVendorOrder(SearchVendorOrderRequest request);
        GetAllVendorResponse GetAllVendor(GetAllVendorRequest request);
        GetVendorOrderByIdResponse GetVendorOrderById(GetVendorOrderByIdRequest request);
        UpdateVendorOrderByIdResponse UpdateVendorOrderById(UpdateVendorOrderByIdRequest request);
        UpdateActiveVendorResponse UpdateActiveVendor(UpdateActiveVendorRequest request);
        QuickCreateVendorMasterdataResponse QuickCreateVendorMasterdata(QuickCreateVendorMasterdataRequest request);
        GetDataCreateVendorResponse GetDataCreateVendor(GetDataCreateVendorRequest request);
        GetDataSearchVendorResponse GetDataSearchVendor(GetDataSearchVendorRequest request);
        GetDataEditVendorResponse GetDataEditVendor(GetDataEditVendorRequest request);
        CreateVendorContactResponse CreateVendorContact(CreateVendorContactRequest request);
        GetDataCreateVendorOrderResponse GetDataCreateVendorOrder(GetDataCreateVendorOrderRequest request);
        GetDataAddVendorOrderDetailResponse GetDataAddVendorOrderDetail(GetDataAddVendorOrderDetailRequest request);
        GetMasterDataSearchVendorOrderResponse GetMasterDataSearchVendorOrder(GetMasterDataSearchVendorOrderRequest request);
        GetDataEditVendorOrderResponse GetDataEditVendorOrder(GetDataEditVendorOrderRequest request);

        GetDataSearchVendorQuoteResponse GetDataSearchVendorQuote(GetDataSearchVendorQuoteRequest request);
        CreateVendorQuoteResponse CreateVendorQuote(ListVendorQuoteRequest request);
        SearchVendorProductPriceResponse SearchVendorProductPrice(SearchVendorProductPriceRequest request);
        CreateVendorProductPriceResponse CreateVendorProductPrice(CreateVendorProductPriceRequest request);
        DeleteProductVendorPriceResponse DeleteProductVendorPrice(DeleteVendorProductPriceRequest request);
        DownloadTemplateVendorProductPriceResponse DownloadTemplateVendorProductPrice(DownloadTemplateVendorProductPriceRequest request);
        ImportVendorProductPriceResponse ImportVendorProductPrice(ImportVendorProductPriceRequest request);
        
        GetMasterDataCreateSuggestedSupplierQuoteResponse GetMasterDataCreateSuggestedSupplierQuote(GetMasterDataCreateSuggestedSupplierQuoteRequest request);
        CreateOrUpdateSuggestedSupplierQuoteResponse CreateOrUpdateSuggestedSupplierQuote(CreateOrUpdateSuggestedSupplierQuoteRequest request);
        DeleteSuggestedSupplierQuoteRequestResponse DeleteSuggestedSupplierQuoteRequest(DeleteSuggestedSupplierQuoteRequestRequest request);
        ChangeStatusVendorQuoteResponse ChangeStatusVendorQuote(ChangeStatusVendorQuoteRequest request);

        GetDataAddEditCostVendorOrderResponse GetDataAddEditCostVendorOrder(
            GetDataAddEditCostVendorOrderRequest request);
        SendEmailVendorQuoteResponse SendEmailVendorQuote(SendEmailVendorQuoteRequest request);
        RemoveVendorOrderResponse RemoveVendorOrder(RemoveVendorOrderRequest request);
        CancelVendorOrderResponse CancelVendorOrder(CancelVendorOrderRequest request);
        DraftVendorOrderResponse DraftVendorOrder(DraftVendorOrderRequest request);

        GetMasterDataVendorOrderReportResponse GetMasterDataVendorOrderReport(
            GetMasterDataVendorOrderReportRequest request);

        SearchVendorOrderReportResponse SearchVendorOrderReport(SearchVendorOrderReportRequest request);
        ApprovalOrRejectVendorOrderResponse ApprovalOrRejectVendorOrder(ApprovalOrRejectVendorOrderRequest request);
        GetQuantityApprovalResponse GetQuantityApproval(GetQuantityApprovalRequest request);
    }
}
