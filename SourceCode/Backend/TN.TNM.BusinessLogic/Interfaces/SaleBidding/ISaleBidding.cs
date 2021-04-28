using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.SaleBidding;
using TN.TNM.BusinessLogic.Messages.Responses.SaleBidding;

namespace TN.TNM.BusinessLogic.Interfaces.SaleBidding
{
    public interface ISaleBidding
    {
        GetMasterDataSaleBiddingDashboardResponse GetMasterDataSaleBiddingDashboard(GetMasterDataSaleBiddingDashboardRequest request);
        GetMasterDataCreateSaleBiddingResponse GetMasterDataCreateSaleBidding(GetMasterDataCreateSaleBiddingRequest request);
        GetMasterDataSearchSaleBiddingResponse GetMasterDataSearchSaleBidding(GetMasterDataSearchSaleBiddingRequest request);
        SearchSaleBiddingResponse SearchBidding(SearchSaleBiddingRequest request);
        CreateSaleBiddingResponse CreateSaleBidding(CreateSaleBiddingRequest request);
        GetMasterDataSaleBiddingAddEditProductDialogResponse GetMasterDataSaleBiddingAddEditProductDialog(
                                                                                    GetMasterDataSaleBiddingAddEditProductDialogRequest request);
        GetVendorByProductIdReponse GetVendorByProductId(GetVendorByProductIdRequest request);
        DownloadTemplateProductResponse DownloadTemplateProduct(DownloadTemplateProductRequest request);
        GetMasterDataSaleBiddingDetailResponse GetMasterDataSaleBiddingDetail(GetMasterDataSaleBiddingDetailRequest request);
        EditSaleBiddingResponse EditSaleBidding(EditSaleBiddingRequest request);
        UpdateStatusSaleBiddingResponse UpdateStatusSaleBidding(UpdateStatusSaleBiddingRequest request);
        GetMasterDataSaleBiddingApprovedResponse GetMasterDataSaleBiddingApproved(GetMasterDataSaleBiddingApprovedRequest request);
        SearchSaleBiddingApprovedResponse SearchSaleBiddingApproved(SearchSaleBiddingApprovedRequest request);
        GetVendorMappingResponse GetVendorMapping(GetVendorMappingRequest request);
        GetCustomerByEmployeeIdResponse GetCustomerByEmployeeId(GetCustomerByEmployeeIdRequest request);
        SendEmailEmployeeResponse SendEmailEmployee(SendEmailEmployeeRequest request);
        GetPersonInChargeByCustomerIdResponse GetPersonInChargeByCustomerId(GetPersonInChargeByCustomerIdRequest request);
    }
}
