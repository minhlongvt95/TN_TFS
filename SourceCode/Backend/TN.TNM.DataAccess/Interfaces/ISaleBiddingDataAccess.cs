using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;
using TN.TNM.DataAccess.Messages.Results.SaleBidding;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ISaleBiddingDataAccess
    {
        GetMasterDataCreateSaleBiddingResult GetMasterDataCreateSaleBidding(GetMasterDataCreateSaleBiddingParameter parameter);
        GetMasterDataSaleBiddingDashboardResult GetMasterDataSaleBiddingDashBoard(GetMasterDataSaleBiddingDashboardParameter parameter);
        CreateSaleBiddingResult CreateSaleBidding(CreateSaleBiddingParameter parameter);
        GetMasterDataSearchSaleBiddingResult GetMasterDataSearchSaleBidding(GetMasterDataSearchSaleBiddingParamter parameter);
        SearchSaleBiddingResult SearchSaleBidding(SearchSaleBiddingParameter parameter);
        GetMasterDataSaleBiddingAddEditProductDialogResult GetMasterDataSaleBiddingAddEditProductDialog(
                                                                      GetMasterDataSaleBiddingAddEditProductDialogParameter parameter);
        GetVendorByProductIdResult GetVendorByProductId(GetVendorByProductIdParameter parameter);
        DownloadTemplateProductResult DownloadTemplateProduct(DownloadTemplateProductParameter parameter);
        GetMasterDataSaleBiddingDetailResult GetMasterDataSaleBiddingDetail(GetMasterDataSaleBiddingDetailParameter parameter);
        EditSaleBiddingResult EditSaleBidding(EditSaleBiddingParameter parameter);
        UpdateStatusSaleBiddingResult UpdateStatusSaleBidding(UpdateStatusSaleBiddingParameter parameter);
        GetMasterDataSaleBiddingApprovedResult GetMasterDataSaleBiddingApproved(GetMasterDataSaleBiddingApprovedParameter parameter);
        SearchSaleBiddingApprovedResult SearchSaleBiddingApproved(SearchSaleBiddingApprovedParameter parameter);
        GetVendorMappingResult GetVendorMapping(GetVendorMappingParameter parameter);
        GetCustomerByEmployeeIdResult GetCustomerByEmployeeId(GetCustomerByEmployeeIdParameter parameter);
        SendEmailEmployeeResult SendEmailEmployee(SendEmailEmployeeParameter parameter);
        GetPersonInChargeByCustomerIdResult GetPersonInChargeByCustomerId(GetPersonInChargeByCustomerIdParameter parameter);
    }
}
