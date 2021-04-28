using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;
using TN.TNM.DataAccess.Messages.Results.BillSale;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IBillSaleDataAccess
    {
        GetMasterDataBillSaleCreateEditResult GetMasterDataBillSaleCreateEdit(GetMasterDataBillSaleCreateEditParameter parameter);
        AddOrEditBillSaleResult AddOrEditBillSale(AddOrEditBillSaleParameter parameter);
        GetOrderByOrderIdResult GetOrderByOrderId(GetOrderByOrderIdParameter parameter);
        SearchBillOfSaleResult SearchBillOfSale(SearchBillOfSaleParameter parameter);
        GetMasterBillOfSaleResult GetMasterBillOfSale(GetMasterBillOfSaleParameter parameter);
        UpdateStatusResult UpdateStatus(UpdateStatusParameter parameter);
        DeleteBillSaleResult DeleteBillSale(DeleteBillSaleParameter parameter);
    }
}
