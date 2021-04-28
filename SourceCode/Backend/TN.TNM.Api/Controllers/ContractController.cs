using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Contract;
using TN.TNM.BusinessLogic.Messages.Requests.Contract;
using TN.TNM.BusinessLogic.Messages.Responses.Contract;

namespace TN.TNM.Api.Controllers
{
    public class ContractController : Controller
    {
        private IContract _iContract;
        public ContractController(IContract iContract)
        {
            this._iContract = iContract;
        }
        /// <summary>
        /// Get master data contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/getMaterDataContract")]
        [Authorize(Policy = "Member")]
        public GetMasterDataContractResponse GetMaterDataContract([FromBody]GetMasterDataContractRequest request)
        {
            return this._iContract.GetMasterDataContract(request);
        }

        /// <summary>
        /// Get list main contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/getListMainContract")]
        [Authorize(Policy = "Member")]
        public GetListMainContractResponses GetListMainContract([FromBody]GetListMainContractRequests request)
        {
            return this._iContract.GetListMainContract(request);
        }

        /// <summary>
        /// Create or update contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/createOrUpdateContract")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateContractRespone CreateOrUpdateContract([FromForm]CreateOrUpdateContractRequest request)
        {
            return this._iContract.CreateOrUpdateContract(request);
        }

        /// <summary>
        /// Clone contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/createCloneContract")]
        [Authorize(Policy = "Member")]
        public CreateCloneContractResponse CreateCloneContract([FromBody]CreateCloneContractRequest request)
        {
            return this._iContract.CreateCloneContract(request);
        }

        [HttpPost]
        [Route("api/contract/uploadFile")]
        [Authorize(Policy = "Member")]
        public UploadFileResponse UploadFile([FromForm]UploadFileRequest request)
        {
            return this._iContract.UploadFile(request);
        }

        [HttpPost]
        [Route("api/contract/deleteFile")]
        [Authorize(Policy = "Member")]
        public DeleteFileResponse DeleteFile([FromBody]DeleteFileRequest request)
        {
            return this._iContract.DeleteFile(request);
        }

        /// <summary>
        /// Get Master data search update contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/getMasterDataSearchContract")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchContractResponse GetMasterDataSearchContract([FromBody]GetMasterDataSearchContractRequest request)
        {
            return this._iContract.GetMasterDataSearchContract(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/searchContract")]
        [Authorize(Policy = "Member")]
        public SearchContractResponse SearchContract([FromBody]SearchContractRequest request)
        {
            return this._iContract.SearchContract(request);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/changeContractStatus")]
        [Authorize(Policy = "Member")]
        public ChangeContractStatusResponse ChangeContractStatus([FromBody]ChangeContractStatusRequest request)
        {
            return this._iContract.ChangeContractStatus(request);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/getMasterDataDashboardContract")]
        [Authorize(Policy = "Member")]
        public GetMasterDataDashboardContractResponse GetMasterDataDashboardContract([FromBody]GetMasterDataDashboardContractRequest request)
        {
            return this._iContract.GetMasterDataDashboardContract(request);
        }

        [HttpPost]
        [Route("api/contract/deleteContract")]
        [Authorize(Policy = "Member")]
        public DeleteContractResponse DeleteContract([FromBody]DeleteContractRequest request)
        {
            return this._iContract.DeleteContract(request);
        }
    }
}