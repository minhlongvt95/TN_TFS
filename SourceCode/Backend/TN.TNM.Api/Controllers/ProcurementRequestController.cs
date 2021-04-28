using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.ProcurementRequest;
using TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest;
using TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest;

namespace TN.TNM.Api.Controllers
{
    public class ProcurementRequestController : Controller
    {
        private readonly IProcurementRequest iProcurementRequest;

        public ProcurementRequestController(IProcurementRequest _iProcurementRequest)
        {
            this.iProcurementRequest = _iProcurementRequest;
        }

        /// <summary>
        /// Controller tạo hóa đơn mua hàng
        /// </summary>
        /// <param name="request">Thông tin hóa đơn mua hàng</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/procurement/createProcurementRequest")]
        [Authorize(Policy = "Member")]
        public CreateProcurementRequestResponse CreateProcurementRequest(CreateProcurementRequestRequest request)
        {
            return iProcurementRequest.CreateProcurementRequest(request);
        }
        /// <summary>
        /// Controller tim kiem hóa đơn mua hàng
        /// </summary>
        /// <param name="request">Thông tin hóa đơn mua hàng</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/procurementRequest/search")]
        [Authorize(Policy = "Member")]
        public SearchProcurementRequestResponse SearchProcurementRequest([FromBody]SearchProcurementRequestRequest request)
        {
            return iProcurementRequest.SearchProcurementRequest(request);
        }

        [HttpPost]
        [Route("api/procurementRequest/searchVendorProductPrice")]
        [Authorize(Policy = "Member")]
        public SearchVendorProductPriceResponse SearchVendorProductPrice([FromBody]SearchVendorProductPriceRequest request)
        {
            return iProcurementRequest.SearchVendorProductPrice(request);
        }

        /// <summary>
        /// Controller lấy dự toán
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/procurement/getAllProcurementPlan")]
        [Authorize(Policy = "Member")]
        public GetAllProcurementPlanResponse GetAllProcurementPlan([FromBody]GetAllProcurementPlanRequest request)
        {
            return iProcurementRequest.GetAllProcurementPlan(request);
        }
        /// <summary>
        /// Controller hien thi
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/procurement/getProcurementRequestById")]
        [Authorize(Policy = "Member")]
        public GetProcurementRequestByIdResponse GetProcurementRequestById([FromBody]GetProcurementRequestByIdRequest request)
        {
            return iProcurementRequest.GetProcurementRequestById(request);
        }
        /// <summary>
        /// Controller edit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/procurement/editProcurementRequest")]
        [Authorize(Policy = "Member")]
        public EditProcurementRequestResponse EditProcurementRequest(EditProcurementRequestRequest request)
        {
            return iProcurementRequest.EditProcurementRequest(request);
        }

        [HttpPost]
        [Route("api/procurement/getDataCreateProcurementRequest")]
        [Authorize(Policy = "Member")]
        public GetDataCreateProcurementRequestResponse GetDataCreateProcurementRequest([FromBody] GetDataCreateProcurementRequestRequest request)
        {
            return iProcurementRequest.GetDataCreateProcurementRequest(request);
        }

        [HttpPost]
        [Route("api/procurement/getDataCreateProcurementRequestItem")]
        [Authorize(Policy = "Member")]
        public GetDataCreateProcurementRequestItemResponse GetDataCreateProcurementRequestItem([FromBody] GetDataCreateProcurementRequestItemRequest request)
        {
            return iProcurementRequest.GetDataCreateProcurementRequestItem(request);
        }

        [HttpPost]
        [Route("api/procurement/getDataEditProcurementRequest")]
        [Authorize(Policy = "Member")]
        public GetDataEditProcurementRequestResponse GetDataEditProcurementRequest([FromBody] GetDataEditProcurementRequestRequest request)
        {
            return iProcurementRequest.GetDataEditProcurementRequest(request);
        }

        [HttpPost]
        [Route("api/procurement/getMasterDataSearchProcurementRequest")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchProcurementRequestResponse GetMasterDataSearchProcurementRequest([FromBody] GetMasterDataSearchProcurementRequest request)
        {
            return iProcurementRequest.GetMasterDataSearchProcurementRequest(request);
        }

        [HttpPost]
        [Route("api/procurement/approvalOrReject")]
        [Authorize(Policy = "Member")]
        public CreateProcurementRequestResponse ApprovalOrReject([FromBody] GetDataEditProcurementRequestRequest request)
        {
            return iProcurementRequest.ApprovalOrReject(request);
        }

        [HttpPost]
        [Route("api/procurement/changeStatus")]
        [Authorize(Policy = "Member")]
        public CreateProcurementRequestResponse ChangeStatus([FromBody] GetDataEditProcurementRequestRequest request)
        {
            return iProcurementRequest.ChangeStatus(request);
        }

        /// <summary>
        /// Controller tim kiem hóa đơn mua hàng
        /// </summary>
        /// <param name="request">Thông tin hóa đơn mua hàng</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/procurementRequest/searchReport")]
        [Authorize(Policy = "Member")]
        public SearchProcurementRequestResponse SearchProcurementRequestReport([FromBody]SearchProcurementRequestRequest request)
        {
            return iProcurementRequest.SearchProcurementRequestReport(request);
        }
    }
}