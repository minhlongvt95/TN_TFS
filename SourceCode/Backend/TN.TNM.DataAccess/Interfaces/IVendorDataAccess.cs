using TN.TNM.DataAccess.Messages.Parameters.Vendor;
using TN.TNM.DataAccess.Messages.Results.Vendor;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IVendorDataAccess
    {
        CreateVendorResult CreateVendor(CreateVendorParameter parameter);
        SearchVendorResult SearchVendor(SearchVendorParameter parameter);
        GetVendorByIdResult GetVendorById(GetVendorByIdParameter parameter);
        GetAllVendorCodeResult GetAllVendorCode(GetAllVendorCodeParameter parameter);
        UpdateVendorByIdResult UpdateVendorById(UpdateVendorByIdParameter parameter);
        QuickCreateVendorResult QuickCreateVendor(QuickCreateVendorParameter parameter);
        CreateVendorOrderResult CreateVendorOrder(CreateVendorOrderParameter parameter);
        SearchVendorOrderResult SearchVendorOrder(SearchVendorOrderParameter parameter);
        GetAllVendorResult GetAllVendor(GetAllVendorParameter parameter);
        GetVendorOrderByIdResult GetVendorOrderById(GetVendorOrderByIdParameter parameter);
        UpdateVendorOrderByIdResult UpdateVendorOrderById(UpdateVendorOrderByIdParameter parameter);
        UpdateActiveVendorResult UpdateActiveVendor(UpdateActiveVendorParameter parameter);
        QuickCreateVendorMasterdataResult QuickCreateVendorMasterdata(QuickCreateVendorMasterdataParameter parameter);
        GetDataCreateVendorResult GetDataCreateVendor(GetDataCreateVendorParameter parameter);
        GetDataSearchVendorResult GetDataSearchVendor(GetDataSearchVendorParameter parameter);
        GetDataEditVendorResult GetDataEditVendor(GetDataEditVendorParameter parameter);
        CreateVendorContactResult CreateVendorContact(CreateVendorContactParameter parameter);
        GetDataCreateVendorOrderResult GetDataCreateVendorOrder(GetDataCreateVendorOrderParameter parameter);
        GetDataAddVendorOrderDetailResult GetDataAddVendorOrderDetail(GetDataAddVendorOrderDetailParameter parameter);
        GetMasterDataSearchVendorOrderResult GetGetMasterDataSearchVendorOrder(GetMasterDataSearchVendorOrderParameter parameter);
        GetDataEditVendorOrderResult GetDataEditVendorOrder(GetDataEditVendorOrderParameter parameter);
        GetDataSearchVendorQuoteResult GetDataSearchVendorQuote(GetDataSearchVendorQuoteParameter parameter);
        CreateVendorQuoteResult CreateVendorQuote(ListVendorQuoteParameter parameter);
        SearchVendorProductPriceResult SearchVendorProductPrice(SearchVendorProductPriceParameter parameter);
        CreateVendorProductPriceResult CreateVendorProductPrice(CreateVendorProductPriceParameter parameter);
        DeleteVendorProductPriceResult DeleteVendorProductPrice(DeleteVendorProductPriceParameter parameter);
        DownloadTemplateVendorProductPriceResult DownloadTemplateVendorProductPrice(DownloadTemplateVendorProductPriceParameter parameter);
        ImportProductVendorPriceResult ImportProductVendorPrice(ImportVendorProductPriceParameter parameter);
        
        GetMasterDataCreateSuggestedSupplierQuoteResult GetMasterDataCreateSuggestedSupplierQuote(GetMasterDataCreateSuggestedSupplierQuoteParameter parameter);
        CreateOrUpdateSuggestedSupplierQuoteResult CreateOrUpdateSuggestedSupplierQuote(CreateOrUpdateSuggestedSupplierQuoteParameter parameter);
        DeleteSuggestedSupplierQuoteRequestResult DeleteSuggestedSupplierQuoteRequest(DeleteSugestedSupplierQuoteRequestParameter parameter);
        ChangeStatusVendorQuoteResult ChangeStatusVendorQuote(ChangeStatusVendorQuoteParameter parameter);

        GetDataAddEditCostVendorOrderResult GetDataAddEditCostVendorOrder(
            GetDataAddEditCostVendorOrderParameter parameter);

        SendEmailVendorQuoteResult SendEmailVendorQuote(SendMailVendorQuoteParameter parameter);
        RemoveVendorOrderResult RemoveVendorOrder(RemoveVendorOrderParameter parameter);
        CancelVendorOrderResult CancelVendorOrder(CancelVendorOrderParameter parameter);
        DraftVendorOrderResult DraftVendorOrder(DraftVendorOrderParameter parameter);

        GetMasterDataVendorOrderReportResult GetMasterDataVendorOrderReport(
            GetMasterDataVendorOrderReportParameter parameter);

        SearchVendorOrderReportResult SearchVendorOrderReport(SearchVendorOrderReportParameter parameter);
        ApprovalOrRejectVendorOrderResult ApprovalOrRejectVendorOrder(ApprovalOrRejectVendorOrderParameter parameter);

        GetQuantityApprovalResult GetQuantityApproval(GetQuantityApprovalParameter paramter);
    }
}
