using TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest;
using TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Interfaces.ProcurementRequest
{
    public interface IProcurementRequest
    {
        /// <summary>
        /// Tạo hóa đơn mua hàng
        /// </summary>
        /// <param name="request">Thông tin hóa đơn mua hàng</param>
        /// <returns></returns>
        CreateProcurementRequestResponse CreateProcurementRequest(CreateProcurementRequestRequest request);
        /// <summary>
        /// tim kiem du taon
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchProcurementRequestResponse SearchProcurementRequest(SearchProcurementRequestRequest request);

        /// <summary>
        /// Lấy tất cả dự toán
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAllProcurementPlanResponse GetAllProcurementPlan(GetAllProcurementPlanRequest request);
        /// <summary>
        /// hien thi hoa don
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetProcurementRequestByIdResponse GetProcurementRequestById(GetProcurementRequestByIdRequest request);
        /// <summary>
        /// edit hao don
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        EditProcurementRequestResponse EditProcurementRequest(EditProcurementRequestRequest request);
        GetDataCreateProcurementRequestResponse GetDataCreateProcurementRequest(GetDataCreateProcurementRequestRequest request);
        GetDataCreateProcurementRequestItemResponse GetDataCreateProcurementRequestItem(GetDataCreateProcurementRequestItemRequest request);
        GetDataEditProcurementRequestResponse GetDataEditProcurementRequest(GetDataEditProcurementRequestRequest request);
        GetMasterDataSearchProcurementRequestResponse GetMasterDataSearchProcurementRequest(GetMasterDataSearchProcurementRequest request);
        CreateProcurementRequestResponse ApprovalOrReject(GetDataEditProcurementRequestRequest request);
        CreateProcurementRequestResponse ChangeStatus(GetDataEditProcurementRequestRequest request);
        SearchProcurementRequestResponse SearchProcurementRequestReport(SearchProcurementRequestRequest request);
        SearchVendorProductPriceResponse SearchVendorProductPrice(SearchVendorProductPriceRequest request);
    }
}
