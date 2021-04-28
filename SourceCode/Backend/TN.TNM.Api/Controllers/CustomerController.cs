using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Customer;
using TN.TNM.BusinessLogic.Messages.Requests.Customer;
using TN.TNM.BusinessLogic.Messages.Responses.Customer;

namespace TN.TNM.Api.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomer _iCustomer;
        public CustomerController(ICustomer iCustomer)
        {
            this._iCustomer = iCustomer;
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/createCustomer")]
        [Authorize(Policy = "Member")]
        public CreateCustomerResponse CreateCustomer([FromBody]CreateCustomerRequest request)
        {
            return this._iCustomer.CreateCustomer(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/searchCustomer")]
        [Authorize(Policy = "Member")]
        public SearchCustomerResponse SearchCustomer([FromBody]SearchCustomerRequest request)
        {
            return this._iCustomer.SearchCustomer(request);
        }

        /// <summary>
        /// Get Customer From Order Create
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getCustomerFromOrderCreate")]
        [Authorize(Policy = "Member")]
        public GetCustomerFromOrderCreateResponse GetCustomerFromOrderCreate([FromBody]GetCustomerFromOrderCreateRequest request)
        {
            return this._iCustomer.GetCustomerFromOrderCreate(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllCustomerServiceLevel")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerServiceLevelResponse GetAllCustomerServiceLevel([FromBody]GetAllCustomerServiceLevelRequest request)
        {
            return this._iCustomer.GetAllCustomerServiceLevel(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getCustomerById")]
        [Authorize(Policy = "Member")]
        public GetCustomerByIdResponse GetCustomerById([FromBody]GetCustomerByIdRequest request)
        {
            return this._iCustomer.GetCustomerById(request);
        }

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/editCustomerById")]
        [Authorize(Policy = "Member")]
        public EditCustomerByIdResponse EditCustomerById([FromBody]EditCustomerByIdRequest request)
        {
            return this._iCustomer.EditCustomerById(request);
        }

        /// <summary>
        /// CustomerList
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllCustomer")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerResponse GetAllCustomer([FromBody]GetAllCustomerRequest request)
        {
            return this._iCustomer.GetAllCustomer(request);
        }

        /// <summary>
        /// CustomerList
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/quickCreateCustomer")]
        [Authorize(Policy = "Member")]
        public QuickCreateCustomerResponse QuickCreateCustomer([FromBody]QuickCreateCustomerRequest request)
        {
            return this._iCustomer.QuickCreateCustomer(request);
        }

        /// <summary>
        /// CustomerCodeList
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllCustomerCode")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerCodeResponse GetAllCustomerCode(GetAllCustomerCodeRequest request)
        {
            return this._iCustomer.GetAllCustomerCode(request);
        }
        /// <summary>
        /// ImportCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/importCustomer")]
        [Authorize(Policy = "Member")]
        public ImportCustomerResponse ImportCustomer(ImportCustomerRequest request)
        {
            return this._iCustomer.ImportCustomer(request);
        }
        /// <summary>
        /// DownloadTemplateCustomer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/downloadTemplateCustomer")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateCustomerResponse DownloadTemplateCustomer([FromBody]DownloadTemplateCustomerRequest request)
        {
            return this._iCustomer.DownloadTemplateCustomer(request);
        }
        /// <summary>
        /// UpdateCustomerDuplicate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/updateCustomerDuplicate")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerDuplicateResponse UpdateCustomerDuplicate([FromBody]UpdateCustomerDuplicateRequest request)
        {
            return this._iCustomer.UpdateCustomerDuplicate(request);
        }
        /// <summary>
        /// getStatisticCustomerForDashboard
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getStatisticCustomerForDashboard")]
        [Authorize(Policy = "Member")]
        public GetStatisticCustomerForDashboardResponse GetStatisticCustomerForDashboard([FromBody]GetStatisticCustomerForDashboardRequest request)
        {
            return this._iCustomer.GetStatisticCustomerForDashboard(request);
        }/// <summary>
        /// getListCustomeSaleToprForDashboard
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getListCustomeSaleToprForDashboard")]
        [Authorize(Policy = "Member")]
        public GetListCustomeSaleToprForDashboardResponse GetListCustomeSaleToprForDashboard([FromBody]GetListCustomeSaleToprForDashboardRequest request)
        {
            return this._iCustomer.GetListCustomeSaleToprForDashboard(request);
        }
        /// <summary>
        /// getStatisticCustomerForDashboard
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/checkDuplicateCustomerPhoneOrEmail")]
        [Authorize(Policy = "Member")]
        public CheckDuplicateCustomerResponse CheckDuplicateCustomerPhoneOrEmail([FromBody]CheckDuplicateCustomerLeadRequest request)
        {
            return this._iCustomer.CheckDuplicateCustomerPhoneOrEmail(request);
        }
        /// <summary>
        /// getStatisticCustomerForDashboard
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/checkDuplicateCustomer")]
        [Authorize(Policy = "Member")]
        public CheckDuplicateCustomerResponse CheckDuplicateCustomer([FromBody]CheckDuplicateCustomerRequest request)
        {
            return this._iCustomer.CheckDuplicateCustomer(request);
        }
        /// <summary>
        /// Check Duplicate Personal Customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/customer/checkDuplicatePersonalCustomer")]
        [Authorize(Policy = "Member")]
        public CheckDuplicatePersonalCustomerResponse CheckDuplicatePersonalCustomer([FromBody]CheckDuplicatePersonalCustomerRequest request)
        {
            return this._iCustomer.CheckDuplicatePersonalCustomer(request);
        }
        /// <summary>
        /// Check Duplicate Personal Customer By Phone Or Email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/customer/checkDuplicatePersonalCustomerByEmailOrPhone")]
        [Authorize(Policy = "Member")]
        public CheckDuplicatePersonalCustomerByEmailOrPhoneResponse CheckDuplicatePersonalCustomerByEmailOrPhone([FromBody]CheckDuplicatePersonalCustomerByEmailOrPhoneRequest request)
        {
            return this._iCustomer.CheckDuplicatePersonalCustomerByEmailOrPhone(request);
        }
        /// <summary>
        /// Get All Customer Additional By CustomerId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllCustomerAdditionalByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetAllCustomerAdditionalByCustomerIdResponse GetAllCustomerAdditionalByCustomerId([FromBody]GetAllCustomerAdditionalByCustomerIdRequest request)
        {
            return this._iCustomer.GetAllCustomerAdditionalByCustomerId(request);
        }

        /// <summary>
        /// Create Customer Additional
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/createCustomerAdditional")]
        [Authorize(Policy = "Member")]
        public CreateCustomerAdditionalResponse CreateCustomerAdditional([FromBody]CreateCustomerAdditionalRequest request)
        {
            return this._iCustomer.CreateCustomerAdditional(request);
        }

        /// <summary>
        /// Delete Customer Additional
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/deleteCustomerAdditional")]
        [Authorize(Policy = "Member")]
        public DeleteCustomerAdditionalResponse DeleteCustomerAdditional([FromBody]DeleteCustomerAdditionalRequest request)
        {
            return this._iCustomer.DeleteCustomerAdditional(request);
        }

        /// <summary>
        /// Edit Customer Additional
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/editCustomerAdditional")]
        [Authorize(Policy = "Member")]
        public EditCustomerAdditionalResponse EditCustomerAdditional([FromBody]EditCustomerAdditionalRequest request)
        {
            return this._iCustomer.EditCustomerAdditional(request);
        }

        /// <summary>
        /// Create List Question
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/createListQuestion")]
        [Authorize(Policy = "Member")]
        public CreateListQuestionResponse CreateListQuestion([FromBody]CreateListQuestionRequest request)
        {
            return this._iCustomer.CreateListQuestion(request);
        }

        /// <summary>
        /// Get List Question Answer By Search
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getListQuestionAnswerBySearch")]
        [Authorize(Policy = "Member")]
        public GetListQuestionAnswerBySearchResponse GetListQuestionAnswerBySearch([FromBody]GetListQuestionAnswerBySearchRequest request)
        {
            return this._iCustomer.GetListQuestionAnswerBySearch(request);
        }

        /// <summary>
        /// Get All History Product By CustomerId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getAllHistoryProductByCustomerId")]
        [Authorize(Policy = "Member")]
        public GetAllHistoryProductByCustomerIdResponse GetAllHistoryProductByCustomerId([FromBody]GetAllHistoryProductByCustomerIdRequest request)
        {
            return this._iCustomer.GetAllHistoryProductByCustomerId(request);
        }
        /// <summary>
        /// Create Customer From Protal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/createCustomerFromProtal")]
        [AllowAnonymous]
        public CreateCustomerFromProtalResponse CreateCustomerFromProtal([FromBody]CreateCustomerFromProtalRequest request)
        {
            return this._iCustomer.CreateCustomerFromProtal(request);
        }

        /// <summary>
        /// Change Customer Status To Delete
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/changeCustomerStatusToDelete")]
        [Authorize(Policy = "Member")]
        public ChangeCustomerStatusToDeleteResponse ChangeCustomerStatusToDelete([FromBody]ChangeCustomerStatusToDeleteRequest request)
        {
            return this._iCustomer.ChangeCustomerStatusToDelete(request);
        }

        //
        /// <summary>
        /// Get DashBoard Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getDashBoardCustomer")]
        [Authorize(Policy = "Member")]
        public GetDashBoardCustomerResponse GetDashBoardCustomer([FromBody]GetDashBoardCustomerRequest request)
        {
            return this._iCustomer.GetDashBoardCustomer(request);
        }

        /// <summary>
        /// Change Customer Status To Delete
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getListCustomer")]
        [Authorize(Policy = "Member")]
        public GetListCustomerResponse GetListCustomer([FromBody]GetListCustomerRequest request)
        {
            return this._iCustomer.GetListCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/createCustomerMasterData")]
        [Authorize(Policy = "Member")]
        public CreateCustomerMasterDataResponse CreateCustomerMasterData([FromBody]CreateCustomerMasterDataRequest request)
        {
            return this._iCustomer.CreateCustomerMasterData(request);
        }

        [HttpPost]
        [Route("api/customer/checkDuplicateCustomerAllType")]
        [Authorize(Policy = "Member")]
        public CheckDuplicateCustomerAllTypeResponse CheckDuplicateCustomerAllType([FromBody]CheckDuplicateCustomerAllTypeRequest request)
        {
            return this._iCustomer.CheckDuplicateCustomerAllType(request);
        }

        //
        /// <summary>
        /// Update Customer By Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/updateCustomerById")]
        [Authorize(Policy = "Member")]
        public UpdateCustomerByIdResponse UpdateCustomerById([FromBody]UpdateCustomerByIdRequest request)
        {
            return this._iCustomer.UpdateCustomerById(request);
        }

        /// <summary>
        /// Get Customer Import Detail
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/getCustomerImportDetai")]
        [Authorize(Policy = "Member")]
        public GetCustomerImportDetailResponse GetCustomerImportDetail([FromBody]GetCustomerImportDetailRequest request)
        {
            return this._iCustomer.GetCustomerImportDetail(request);
        }

        /// <summary>
        /// Import List Customer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/importListCustomer")]
        [Authorize(Policy = "Member")]
        public ImportListCustomerResponse ImportListCustomer([FromBody]ImportListCustomerRequest request)
        {
            return this._iCustomer.ImportListCustomer(request);
        }

        //
        [HttpPost]
        [Route("api/customer/deleteListCustomerAdditional")]
        [Authorize(Policy = "Member")]
        public DeleteListCustomerAdditionalResponse DeleteListCustomerAdditional([FromBody]DeleteListCustomerAdditionalRequest request)
        {
            return this._iCustomer.DeleteListCustomerAdditional(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getHistoryCustomerCare")]
        [Authorize(Policy = "Member")]
        public GetHistoryCustomerCareResponse GetHistoryCustomerCare([FromBody]GetHistoryCustomerCareRequest request)
        {
            return this._iCustomer.GetHistoryCustomerCare(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getDataPreviewCustomerCare")]
        [Authorize(Policy = "Member")]
        public GetDataPreviewCustomerCareResponse GetDataPreviewCustomerCare([FromBody]GetDataPreviewCustomerCareRequest request)
        {
            return this._iCustomer.GetDataPreviewCustomerCare(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getDataCustomerCareFeedBack")]
        [Authorize(Policy = "Member")]
        public GetDataCustomerCareFeedBackResponse GetDataCustomerCareFeedBack([FromBody]GetDataCustomerCareFeedBackRequest request)
        {
            return this._iCustomer.GetDataCustomerCareFeedBack(request);
        }

        //
        [HttpPost]
        [Route("api/customer/saveCustomerCareFeedBack")]
        [Authorize(Policy = "Member")]
        public SaveCustomerCareFeedBackResponse SaveCustomerCareFeedBack([FromBody]SaveCustomerCareFeedBackRequest request)
        {
            return this._iCustomer.SaveCustomerCareFeedBack(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getDataCustomerMeetingById")]
        [Authorize(Policy = "Member")]
        public GetDataCustomerMeetingByIdResponse GetDataCustomerMeetingById([FromBody]GetDataCustomerMeetingByIdRequest request)
        {
            return this._iCustomer.GetDataCustomerMeetingById(request);
        }

        //
        [HttpPost]
        [Route("api/customer/createCustomerMeeting")]
        [Authorize(Policy = "Member")]
        public CreateCustomerMeetingResponse CreateCustomerMeeting([FromBody]CreateCustomerMeetingRequest request)
        {
            return this._iCustomer.CreateCustomerMeeting(request);
        }

        //
        [HttpPost]
        [Route("api/customer/getHistoryCustomerMeeting")]
        [Authorize(Policy = "Member")]
        public GetHistoryCustomerMeetingResponse GetHistoryCustomerMeeting([FromBody]GetHistoryCustomerMeetingRequest request)
        {
            return this._iCustomer.GetHistoryCustomerMeeting(request);
        }

        //
        [HttpPost]
        [Route("api/customer/sendApprovalCustomer")]
        [Authorize(Policy = "Member")]
        public SendApprovalResponse SendApproval([FromBody]SendApprovalRequest request)
        {
            return this._iCustomer.SendApproval(request);
        }

        /// <summary>
        /// search Customer Approval
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/searchCustomerApproval")]
        [Authorize(Policy = "Member")]
        public SearchCustomerResponse GetListCustomerRequestApproval([FromBody]SearchCustomerRequest request)
        {
            return this._iCustomer.GetListCustomerRequestApproval(request);
        }

        /// <summary>
        /// Approval Or RejectCustomer
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/customer/approvalOrRejectCustomer")]
        [Authorize(Policy = "Member")]
        public SendApprovalResponse ApprovalOrRejectCustomer([FromBody]ApprovalOrRejectCustomerRequest request)
        {
            return this._iCustomer.ApprovalOrRejectCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataCreatePotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataCreatePotentialCustomerResponse GetDataCreatePotentialCustomer([FromBody]GetDataCreatePotentialCustomerRequest request)
        {
            return this._iCustomer.GetDataCreatePotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataDetailPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataDetailPotentialCustomerResponse GetDataDetailPotentialCustomer([FromBody]GetDataDetailPotentialCustomerRequest request)
        {
            return this._iCustomer.GetDataDetailPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/updatePotentialCustomer")]
        [Authorize(Policy = "Member")]
        public UpdatePotentialCustomerResponse UpdatePotentialCustomer([FromBody]UpdatePotentialCustomerRequest request)
        {
            return this._iCustomer.UpdatePotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataSearchPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataSearchPotentialCustomerResponse GetDataSearchPotentialCustomer([FromBody]GetDataSearchPotentialCustomerRequest request)
        {
            return this._iCustomer.GetDataSearchPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/searchPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public SearchPotentialCustomerResponse SearchPotentialCustomer([FromBody]SearchPotentialCustomerRequest request)
        {
            return this._iCustomer.SearchPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataDashboardPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataDashboardPotentialCustomerResponse GetDataDashboardPotentialCustomer([FromBody]GetDataDashboardPotentialCustomerRequest request)
        {
            return this._iCustomer.GetDataDashboardPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/convertPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public ConvertPotentialCustomerResponse ConvertPotentialCustomer([FromBody]ConvertPotentialCustomerRequest request)
        {
            return this._iCustomer.ConvertPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/downloadTemplatePotentialCustomer")]
        [Authorize(Policy = "Member")]
        public DownloadTemplatePotentialCustomerResponse DownloadTemplatePotentialCustomer([FromBody]DownloadTemplatePotentialCustomerRequest request)
        {
            return this._iCustomer.DownloadTemplatePotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/getDataImportPotentialCustomer")]
        [Authorize(Policy = "Member")]
        public GetDataImportPotentialCustomerResponse GetDataImportPotentialCustomer([FromBody]GetDataImportPotentialCustomerRequest request)
        {
            return this._iCustomer.GetDataImportPotentialCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/downloadTemplateImportCustomer")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateImportCustomerResponse DownloadTemplateImportCustomer([FromBody]DownloadTemplateImportCustomerRequest request)
        {
            return this._iCustomer.DownloadTemplateImportCustomer(request);
        }

        [HttpPost]
        [Route("api/customer/searchContactCustomer")]
        [Authorize(Policy = "Member")]
        public SearchContactCustomerResponse SearchContactCustomer([FromBody] SearchContactCustomerRequest request)
        {
            return this._iCustomer.SearchContactCustomer(request);
        }

        //
        [HttpPost]
        [Route("api/customer/checkDuplicateInforCustomer")]
        [Authorize(Policy = "Member")]
        public CheckDuplicateInforCustomerResponse CheckDuplicateInforCustomer([FromBody] CheckDuplicateInforCustomerRequest request)
        {
            return this._iCustomer.CheckDuplicateInforCustomer(request);
        }

        //
        [HttpPost]
        [Route("api/customer/changeStatusSupport")]
        [Authorize(Policy = "Member")]
        public ChangeStatusSupportResponse ChangeStatusSupport([FromBody] ChangeStatusSupportRequest request)
        {
            return this._iCustomer.ChangeStatusSupport(request);
        }
    }
}