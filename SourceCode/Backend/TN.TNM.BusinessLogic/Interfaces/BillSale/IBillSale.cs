using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.BillSale;
using TN.TNM.BusinessLogic.Messages.Responses.BillSale;

namespace TN.TNM.BusinessLogic.Interfaces.BillSale
{
    public interface IBillSale
    {
        GetMasterDataBillSaleCreateEditResponse GetMasterDataBillSaleCreateEdit(GetMasterDataBillSaleCreateEditRequest request);
        AddOrEditBillSaleResponse AddOrEditBillSale(AddOrEditBillSaleRequest request);
        SearchBillOfSaleResponse SearchBillOfSale(SearchBillOfSaleRequest request);
        GetMasterBillOfSaleResponse GetMasterBillOfSale(GetMasterBillOfSaleRequest request);
        GetOrderByOrderIdResponse GetOrderByOrderId(GetOrderByOrderIdRequest request);
        UpdateStatusResponse UpdateStatus(UpdateStatusRequest request);
        DeleteBillSaleResponse DeleteBillSale(DeleteBillSaleRequest request);
    }
}
