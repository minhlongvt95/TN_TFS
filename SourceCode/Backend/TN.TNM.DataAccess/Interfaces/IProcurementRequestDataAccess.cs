using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;
using TN.TNM.DataAccess.Messages.Results.ProcurementRequest;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IProcurementRequestDataAccess
    {
        /// <summary>
        /// Tạo hóa đơn mua hàng
        /// </summary>
        /// <param name="parameter">Thông tin của hóa đơn</param>
        /// <returns></returns>
        CreateProcurementRequestResult CreateProcurementRequest(CreateProcurementRequestParameter parameter);
        /// <summary>
        /// Tim kiem hoa don mua hang
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        SearchProcurementRequestResult SearchProcurementRequest(SearchProcurementRequestParameter parameter);

        /// <summary>
        /// Lấy tất cả dự toán
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetAllProcurementPlanResult GetAllProcurementPlan(GetAllProcurementPlanParameter parameter);
        /// <summary>
        /// hien thi 1 du toan
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        GetProcurementRequestByIdResult GetProcurementRequestById(GetProcurementRequestByIdParameter parameter);
        /// <summary>
        /// Sua du toan
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        EditProcurementRequestResult EditProcurementRequest(EditProcurementRequestParameter parameter);
        GetDataCreateProcurementRequestResult GetDataCreateProcurementRequest(GetDataCreateProcurementRequestParameter parameter);
        GetDataCreateProcurementRequestItemResult GetDataCreateProcurementRequestItem(GetDataCreateProcurementRequestItemParameter parameter);
        GetDataEditProcurementRequestResult GetDataEditProcurementRequest(GetDataEditProcurementRequestParameter parameter);
        CreateProcurementRequestResult ApprovalOrReject(GetDataEditProcurementRequestParameter parameter);
        GetMasterDataSearchProcurementRequestResult GetMasterDataSearchProcurementRequest(GetMasterDataSearchProcurementRequestParameter parameter);
        CreateProcurementRequestResult ChangeStatus(GetDataEditProcurementRequestParameter parameter);
        SearchProcurementRequestResult SearchProcurementRequestReport(SearchProcurementRequestParameter parameter);
        SearchVendorProductPriceResult SearchVendorProductPrice(SearchVendorProductPriceParameter parameter);
    }
}
